// This code is a part of samples package for Apitron PDF Rasterizer .NET
// © 2014 Apitron LTD 

using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.View;

namespace AndroidSampleWithViewPager
{
    using System.IO;

    using Android.Graphics;
    using Android.Provider;
	using Android.Views;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

	[Activity(Label = "AndroidSampleWithViewPager", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		ViewPager pager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

			// initial setup
			pager = this.FindViewById<ViewPager> (Resource.Id.pager);

			Button loadBtn = (Button)FindViewById (Resource.Id.loadBtn);

			loadBtn.Click += button_Click;
		}
	

        private void button_Click(object sender, EventArgs e)
        {
			// document stream
			MemoryStream ms = new MemoryStream ();
            
			// load the linked sample pdf file, returned stream is not seekable so load to memory stream
            using (Stream stream = Assets.Open("development guide.pdf"))
			{
                stream.CopyTo(ms);
			}

			ms.Position = 0;

            // create focument from the stream and request first page
            Document doc = new Document(ms);

			// Create the adapter based on loaded document,
			// this simple implementation uses disk-caching to store rendered pages and reduce memory usage.
            // ProgressiveRendering property gets or sets the value that indicates whether
            // the rendering thread should continously update the view while the page is being rendered.            
			pager.Adapter = new SimplePagerAdapter (this.BaseContext, doc){ProgressiveRendering = true};

		    // Set the current page
		    pager.SetCurrentItem (0, false);
        }
    }
}

