using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.ErrorHandling;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Page = Windows.UI.Xaml.Controls.Page;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowsStoreAppSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        /// <summary>
        /// Handles rendering button click event
        /// </summary>
        private async void btnRender_Click(object sender, RoutedEventArgs e)
        {
            // get the assets folder for the app
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");

            // get the file included in app assets
            StorageFile file = await folder.GetFileAsync("testfile.pdf");

            // open the file and render first page
            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                Document doc = new Document(stream);

                Apitron.PDF.Rasterizer.Page page = doc.Pages[0];

                ErrorLogger logger = new ErrorLogger();
                
                WriteableBitmap bm = page.Render((int) page.Width, (int) page.Height, new RenderingSettings(), logger);                

                myImage.Source = bm;                
            }
        }
    }
}
