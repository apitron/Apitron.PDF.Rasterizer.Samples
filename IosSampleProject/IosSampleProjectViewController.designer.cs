// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace IosSampleProject
{
	[Register ("IosSampleProjectViewController")]
	partial class IosSampleProjectViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton btnRenderFile { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnRenderFile != null) {
				btnRenderFile.Dispose ();
				btnRenderFile = null;
			}
		}
	}
}
