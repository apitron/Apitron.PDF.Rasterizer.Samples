using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.ErrorHandling;
using Apitron.PDF.Rasterizer.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace UniversalAppSamplePDFViewerControlForWindows10.Controls
{
    public sealed partial class PDFViewerControl : UserControl
    {
        #region fields

        private PDFViewerControlViewModel viewModel;

        #endregion

        #region ctor

        public PDFViewerControl()
        {
            this.InitializeComponent();

            viewModel = new PDFViewerControlViewModel();
            viewModel.Navigated += NavigationHandler;

            this.DataContext = viewModel;
        }

        #endregion

        #region Opening the file

        /// <summary>
        /// Handles open file button click event.
        /// </summary>
        private async void OpenFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // show file open dialog
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".pdf");

                StorageFile file = await picker.PickSingleFileAsync();

                if (file != null)
                {                    
                    // load new file into the model
                    viewModel.LoadFile(await file.OpenStreamForReadAsync());
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }
        
        #endregion

        #region Navigation

        private void NavigationHandler(object sender, NavigatedEventArgs eventArgs)
        {
            RenderPage(eventArgs.NewPage);                                    
        }

        private void RenderPage(Apitron.PDF.Rasterizer.Page page)
        {
            if (page != null)
            {
                ErrorLogger logger = new ErrorLogger();

                WriteableBitmap bm = page.Render(new Resolution(96, 96), new RenderingSettings(), logger);

                myImage.Source = bm;                
            }
        }

        private void ShowPreviousPage(object sender, RoutedEventArgs e)
        {
            viewModel.MoveBackward();
        }

        private void ShowNextPage(object sender, RoutedEventArgs e)
        {
            viewModel.MoveForward();
        }

        #endregion

        #region Zooming

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            viewModel.ZoomOut();

            scrollViewer.ZoomToFactor((float)viewModel.Zoom);
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            viewModel.ZoomIn();

            scrollViewer.ZoomToFactor((float)viewModel.Zoom);
        }

        #endregion
    }
}
