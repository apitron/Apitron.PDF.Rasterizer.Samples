#region

using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using Microsoft.Win32;
using WpfPdfViewer.ViewModels;
using Page = Apitron.PDF.Rasterizer.Page;

#endregion

namespace WpfPdfViewer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Windows.Shapes;

    using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private delegate void SetImageSourceDelegate( byte[] source, IList<Link> links, int width, int height );

        public DocumentViewModel document = null;

        private Task task;

        private int GlobalScale = 2;

        private Rectangle destinationRectangle;

        #endregion

        #region Ctors

        public MainWindow()
        {
            InitializeComponent();
            this.document = new DocumentViewModel();
            this.DataContext = this.document;
            this.document.PropertyChanged += DocumentOnPropertyChanged;
        }

        #endregion

        #region Private Members

        private void UpdatePageView()
        {
            Page currentPage = this.document.Page;
            int desiredWidth = (int)currentPage.Width * GlobalScale;
            int desiredHeight = (int)currentPage.Height * GlobalScale;
            byte[] image = currentPage.RenderAsBytes( desiredWidth, desiredHeight, new RenderingSettings() );
            IList<Link> links = currentPage.Links;

            SetImageSourceDelegate del = this.SetImageSource;
            this.Dispatcher.Invoke( del, image, links, desiredWidth, desiredHeight );
        }

        private void SetImageSource( byte[] image, IList<Link> links, int width, int height )
        {
            BitmapSource source = BitmapSource.Create( width, height, 72, 72, PixelFormats.Bgr32, null, image, 4 * width );
            this.PageImage.Source = source;
            this.PageImage.Width = width;
            this.PageImage.Height = height;

            this.PageCanvas.Width = width;
            this.PageCanvas.Height = height;

            for (int i = 1; i < this.PageCanvas.Children.Count; i++)
            {
                Button rectangle = (Button)this.PageCanvas.Children[i];
                rectangle.Click -= this.OnLinkClick;
            }

            this.PageCanvas.Children.RemoveRange( 1, this.PageCanvas.Children.Count );

            foreach (Link link in links)
            {
                Apitron.PDF.Rasterizer.Rectangle location = link.GetLocationRectangle( width, height, null );
                Button rectangle = new Button();
                rectangle.Opacity = 0;
                rectangle.Cursor = Cursors.Hand;
                rectangle.Width = location.Right - location.Left;
                rectangle.Height = location.Top - location.Bottom;
                Canvas.SetLeft( rectangle, location.Left );
                Canvas.SetBottom( rectangle, location.Bottom );
                rectangle.Click += this.OnLinkClick;
                rectangle.DataContext = link;
                this.PageCanvas.Children.Add( rectangle );
            }

            this.UpdateImageZoom();
            this.UpdateViewLocation( this.destinationRectangle );
        }

        private void UpdateImageZoom()
        {
            Slider slider = this.ZoomSlider;
            Canvas imageContainer = this.PageCanvas;
            if (imageContainer != null && slider != null && this.document != null)
            {
                double scale = slider.Value;
                imageContainer.LayoutTransform = new ScaleTransform( scale, scale );
            }
        }

        /// <summary>
        /// Updates the view location.
        /// </summary>
        /// <param name="destinationInfo">The destination info.</param>
        private void UpdateViewLocation( Apitron.PDF.Rasterizer.Rectangle destinationInfo )
        {
            if (destinationInfo == null)
            {
                this.PageScroller.ScrollToTop();
                return;
            }
            double value = this.ZoomSlider.Value;
            double scale = value;

            double horizontalScale = this.PageScroller.ViewportWidth / this.PageImage.ActualWidth;
            double verticalScale = this.PageScroller.ViewportHeight / this.PageImage.ActualHeight;

            if (destinationInfo.Bottom != 0 && destinationInfo.Right != this.PageImage.ActualWidth)
            {
                double expectedHScale = this.PageScroller.ViewportWidth / destinationInfo.Height;
                double expectidVScale = this.PageScroller.ViewportHeight / destinationInfo.Width;
                horizontalScale = expectedHScale;
                verticalScale = expectidVScale;

                scale = Math.Min( verticalScale, horizontalScale );
                this.ZoomSlider.Value = scale;
            }

            this.PageScroller.ScrollToHorizontalOffset( destinationInfo.Left * scale );
            this.PageScroller.ScrollToVerticalOffset( ( this.PageImage.ActualHeight - destinationInfo.Top ) * scale );
        }

        #endregion

        #region Event Handlers

        private void OnOpenFileClick( object sender, RoutedEventArgs e )
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDF Files (*.pdf, *.PDF)|*.pdf;*.PDF";
            bool? dialogResult = dialog.ShowDialog( this );
            if (dialogResult.Value)
            {
                Document document = new Document( new FileStream( dialog.FileName, FileMode.Open, FileAccess.Read ) );
                ( this.document ).Document = document;
            }
        }

        private void OnExitClick( object sender, RoutedEventArgs e )
        {
            this.Close();
        }

        private void OnBookmarkSelectionChanged( object sender, RoutedPropertyChangedEventArgs<object> e )
        {
            Bookmark newValue = (Bookmark)e.NewValue;
            if (newValue != null)
            {
                document.Document.Navigator.GoToBookmark( newValue );
                this.destinationRectangle = newValue.GetDestinationRectangle( (int)( this.document.Page.Width * this.GlobalScale ), (int)( this.document.Page.Height * this.GlobalScale ), null );
            }
        }

        private void OnLinkClick( object sender, RoutedEventArgs routedEventArgs )
        {
            Link link = (Link)( (Button)sender ).DataContext;
            this.document.Document.Navigator.GoToLink( link );

            this.destinationRectangle = link.GetDestinationRectangle( (int)( this.document.Page.Width * this.GlobalScale ), (int)( this.document.Page.Height * this.GlobalScale ), null );
        }


        private void OnNavigationButtonClick( object sender, RoutedEventArgs e )
        {
            Button source = (Button)e.Source;
            Document doc = document.Document;
            DocumentNavigator navigator = doc == null ? null : doc.Navigator;
            if (doc == null || navigator == null)
            {
                return;
            }
            switch ((string)source.CommandParameter)
            {
                case "Next":
                    navigator.MoveForward();
                    break;
                case "Prev":
                    navigator.MoveBackward();
                    break;
                case "First":
                    navigator.Move( 0, Origin.Begin );
                    break;
                case "Last":
                    navigator.Move( 0, Origin.End );
                    break;
                default:
                    return;
            }
            this.destinationRectangle = null;
        }

        private void OnZoomChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            this.UpdateImageZoom();
        }


        private void DocumentOnPropertyChanged( object sender, PropertyChangedEventArgs propertyChangedEventArgs )
        {
            if (propertyChangedEventArgs.PropertyName == "Page")
            {
                task = new Task( UpdatePageView );
                task.Start();
            }
        }

        #endregion
    }
}
