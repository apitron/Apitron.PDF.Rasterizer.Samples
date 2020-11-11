using System;
using CoreGraphics;
using Foundation;
using UIKit;
using Apitron.PDF.Rasterizer;
using System.IO;
using Apitron.PDF.Rasterizer.Configuration;
using CoreGraphics;

namespace IosSampleProject
{
	public partial class IosSampleProjectViewController : UIViewController
	{
		public IosSampleProjectViewController () : base ("IosSampleProjectViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.

			// sign up for a touch up event for our button
			btnRenderFile.TouchUpInside += BtnClickHandler;
		}

		// handles the touch up event for our button
		private void BtnClickHandler(object sender,EventArgs args)
		{
			// open a PDF file from app bundle
			using(Stream stream  = File.OpenRead(Path.Combine(NSBundle.MainBundle.BundlePath,"testfile.pdf")))
			{
				// construct the document
				Document doc = new Document (stream);
			
				// we are going to render first page from the document
				Page page = doc.Pages [0];

				int w = (int)page.Width;
				int h = (int)page.Height;
		
				// render the page to a raw bitmap data represented by byte array
				byte[] imageData = ConvertBGRAtoRGBA(page.RenderAsBytes (w,h, new RenderingSettings (), null));

				// create CGDataProvider which will serve CGImage creation
				CGDataProvider dataProvider = new CGDataProvider (imageData, 0, imageData.Length);

				// create core graphics image using data provider created above, note that
				// we use CGImageAlphaInfo.Last(ARGB) pixel format
				CGImage cgImage = new CGImage(w,h,8,32,w*4,CGColorSpace.CreateDeviceRGB(),CGImageAlphaInfo.Last,dataProvider,null,false, CGColorRenderingIntent.Default);

				// create UIImage and save it to gallery
				UIImage finalImage = new UIImage (cgImage);

				//finalImage.SaveToPhotosAlbum ((img, error) =>{ var o = img as UIImagge; Console.WriteLine("error:" + error);});
				finalImage.SaveToPhotosAlbum (null);

				// show notification message
				UIAlertView view = new UIAlertView ("Message",string.Format("Image has been saved to photo gallery"),null,"Ok",null);

				view.Show ();
			}
		}

		/// <summary>
		/// Converts the BGRA data to RGBA.
		/// </summary>
		/// <returns>Same byte array but with RGBA color dara.</returns>
		/// <param name="bgraData">Raw bitmap data in BGRA8888 format .</param>
		byte[] ConvertBGRAtoRGBA(byte[] bgraData)
		{
			// implemented simple conversion, swap 2 bytes.
			byte tmp;

			for(int i=0,k=2;i<bgraData.Length;i+=4,k+=4)
			{
				tmp = bgraData [i];
				bgraData [i] = bgraData [k];
				bgraData [k] = tmp;
			}

			return bgraData;
		}

		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

