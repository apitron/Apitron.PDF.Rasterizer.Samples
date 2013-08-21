using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.ErrorHandling;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhoneAppSample.Resources;
using Windows.Storage;

namespace WindowsPhoneAppSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles Render button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnRenderClicked(object sender, RoutedEventArgs e)
        {           
            // get the assets folder for the app
            StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");

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