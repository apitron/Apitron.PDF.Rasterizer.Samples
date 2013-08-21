using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += new EventHandler(button_Click);
        }

        private void button_Click(object sender, EventArgs e)
        {
            // load the linked sample pdf file
            using (Stream stream = Assets.Open("testfile.pdf"), ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                ms.Position = 0;

                // create focument from the stream and request first page
                Document doc = new Document(ms);

                Page page = doc.Pages[0];

                // render the page using default settings
                int[] bitmapData = page.RenderAsInts((int)page.Width, (int)page.Height, new RenderingSettings());

                // convert given data to an android bitmap
                Bitmap bm = Bitmap.CreateBitmap(bitmapData, (int)page.Width, (int)page.Height, Bitmap.Config.Argb8888);

                // save image to device's gallery
                string imageUri = MediaStore.Images.Media.InsertImage(this.ContentResolver, bm, "MyImage", "An image created using Apitron.PDF.Rasterizer");

                // try to open the newly created image
                StartActivity( new Intent( Intent.ActionView,  Android.Net.Uri.Parse(imageUri)) );
            }
        }
    }
}

