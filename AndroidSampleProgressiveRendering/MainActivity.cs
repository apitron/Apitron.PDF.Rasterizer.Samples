using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using Java.Nio;

namespace AndroidSampleProgressiveRendering
{
    using System.IO;

    using Android.Graphics;
    using Android.Provider;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

    [Activity(Label = "AndroidSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		ImageView myImageView;
		int[] imageData;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			myImageView = FindViewById<ImageView>(Resource.Id.myImageView);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += new EventHandler(button_Click);
        }

        private void button_Click(object sender, EventArgs e)
        {
			// start rendering task
			ThreadPool.QueueUserWorkItem((wcb)=>
				{
		            // load the linked sample pdf file
		            using (Stream stream = Assets.Open("development guide.pdf"), ms = new MemoryStream())
		            {
		                stream.CopyTo(ms);

		                ms.Position = 0;

		                // create focument from the stream and request first page
						using(Document doc = new Document(ms))
						{
			                Page page = doc.Pages[1];

							int width = (int)page.Width;
							int height = (int)page.Height;

							imageData = new int[width * height];

							// create the bitmap
							Bitmap  bm = Bitmap.CreateBitmap(width, height,Bitmap.Config.Argb8888);

							// make it mutable
							bm = bm.Copy(Bitmap.Config.Argb8888,true);

							myImageView.Post(()=> myImageView.SetImageBitmap (bm));

							ManualResetEvent renderingFinishedEvent = new ManualResetEvent (false);

							// start the updating thread
							ThreadPool.QueueUserWorkItem ((cb) => 
								{
									bool rendered = false;

									do
									{
										// check the rendering state
										rendered = renderingFinishedEvent.WaitOne(0);

										// write "in progress" image
										bm.SetPixels(imageData,0,width,0,0,width,height);

										// redraw
										myImageView.PostInvalidate();

										// sleep if not rendered and then continue
										if(!rendered)
										{
											Thread.Sleep(2000);
										}
									}
									while(!rendered);
							    }
							);

							// start the rendering
							page.RenderAsInts(width, height,imageData, new RenderingSettings ());

							// make sure that update thread will have updating finished
							renderingFinishedEvent.Set ();
						}
					}		
				}
 			  );
				
            }
        }
    
}

