using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Thread = Java.Lang.Thread;

namespace AndroidSample
{
    using System.IO;

    using Android.Graphics;
    using Android.Provider;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

    [Activity(Label = "AndroidSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        ImageView pageView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += new EventHandler(button_Click);

            pageView = FindViewById<ImageView>(Resource.Id.myImageView);
            pageView.SetImageResource(Android.Resource.Color.Transparent);
        }

        private void button_Click(object sender, EventArgs e)
        {
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Rendering page. Please wait...");
            progress.SetCancelable(false);
            progress.Show();

            new Thread(() =>
            {
                RunOnUiThread(() =>
                {
                    // clear theview
                    pageView.SetImageResource(Android.Resource.Color.Transparent);
                });

                // load the linked sample pdf file
                using (Stream stream = Assets.Open("testfile.pdf"), ms = new MemoryStream())
                {
                    try
                    {
                        stream.CopyTo(ms);

                        ms.Position = 0;

                        // create focument from the stream and request first page
                        using (Document doc = new Document(ms,
                            new EngineSettings() {MemoryAllocationMode = MemoryAllocationMode.ResourcesLowMemory}))
                        {

                            Page page = doc.Pages[0];

                            // render the page using default settings
                            int[] bitmapData = page.RenderAsInts((int) page.Width, (int) page.Height,
                                new RenderingSettings());

                            // convert given data to an android bitmap
                            Bitmap bm = Bitmap.CreateBitmap(bitmapData, (int) page.Width, (int) page.Height,
                                Bitmap.Config.Argb8888);

                            RunOnUiThread(() =>
                            {
                                pageView.SetImageBitmap(bm);
                            });
                        }

                        //                // save image to device's gallery
                        //                string imageUri = MediaStore.Images.Media.InsertImage(this.ContentResolver, bm, "MyImage", "An image created using Apitron.PDF.Rasterizer");
                        //
                        //                // try to open the newly created image
                        //                StartActivity( new Intent( Intent.ActionView,  Android.Net.Uri.Parse(imageUri)) );
                    }
                    finally
                    {
                        RunOnUiThread(() =>
                        {
                            progress.Hide();
                        });
                    }
                }
            }).Start();
        }
    }
}

