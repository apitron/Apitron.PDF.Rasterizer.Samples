using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.UI.Xaml.Media;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Navigation;

namespace UniversalAppSamplePDFViewerControlForWindows10.Controls
{
    /// <summary>
    /// A model used by the <see cref="PDFViewerControl"/>.
    /// </summary>
    public class PDFViewerControlViewModel:INotifyPropertyChanged
    {
        #region fields

        private Document currentDocument;
        private double [] possibleZoomValues;
        private int zoomIndex;

        #endregion

        #region properties

        /// <summary>
        /// Indicates whether the control can navigate forward.
        /// </summary>
        public bool CanMoveForward
        {
            get
            {
                if (currentDocument != null)
                {
                    return currentDocument.Navigator.CurrentIndex < (currentDocument.Pages.Count - 1);
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether the control can navigate backward.
        /// </summary>
        public bool CanMoveBackward
        {
            get
            {
                if (currentDocument != null)
                {
                    return currentDocument.Navigator.CurrentIndex > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether the control can zoom in.
        /// </summary>
        public bool CanZoomIn
        {
            get
            {
                if (currentDocument != null)
                {
                    return zoomIndex + 1 < possibleZoomValues.Length;
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates whether the control can zoom out.
        /// </summary>
        public bool CanZoomOut
        {
            get
            {
                if (currentDocument != null)
                {
                    return zoomIndex > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets current zoom factor.
        /// </summary>
        public double Zoom
        {
            get
            {
                return possibleZoomValues[zoomIndex];
            }
        }

        /// <summary>
        /// Gets current page index or -1 if the document is not opened.
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                if (currentDocument != null)
                {
                    return currentDocument.Navigator.CurrentIndex;
                }

                return -1;
            }
        }

        /// <summary>
        /// Getd doc's page count or 0 if no document is opened.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (currentDocument != null)
                {
                    return currentDocument.Pages.Count;
                }

                return 0;
            }
        }

        #endregion

        #region events

        /// <summary>
        /// An event raised when navigation occurs initiated by the <see cref="MoveForward"/> and <see cref="MoveBackward"/>.
        /// This event is also being fired when new document is loaded using <see cref="LoadFile"/>.
        /// </summary>
        public NavigatedDelegate Navigated;
        /// <summary>
        /// Standard event that comes from <see cref="INotifyPropertyChanged"/> implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ctor

        public PDFViewerControlViewModel()
        {
            possibleZoomValues = new double[] {0.25,0.5,0.75,1,1.25,1.5,1.75,2};
            zoomIndex = 3;
        }

        #endregion

        #region Opening an closing the file


        /// <summary>
        /// Loads PDF file from the stream into the model.
        /// </summary>
        /// <param name="documentStream">Stream, containing PDF data, can be null.</param>
        public void LoadFile(Stream documentStream)
        {
            try
            {           
                if (documentStream != null)
                {
                    currentDocument = new Document(documentStream);

                    // subscribe to navigation events and go to first page
                    currentDocument.Navigator.Navigated += Navigator_Navigated;
                    currentDocument.Navigator.Move(0, Origin.Begin);
                }
                else
                {
                    Dispose();
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                Dispose();                
            }

            OnPropertyChanged("Zoom");
            OnPropertyChanged("CanZoomIn");
            OnPropertyChanged("CanZoomOut");
            OnPropertyChanged("PageCount");
        }

        /// <summary>
        /// Close currently opened document.
        /// </summary>
        private void Dispose()
        {
            if (currentDocument != null)
            {
                // sign off from the navigation event
                currentDocument.Navigator.Navigated -= Navigator_Navigated;

                currentDocument.Dispose();
                currentDocument = null;

                zoomIndex = 3;
            }
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Pass the navigation event further.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventargs"></param>
        private void Navigator_Navigated(object sender, NavigatedEventArgs eventargs)
        {            
            if (Navigated != null)
            {
                Navigated(sender, eventargs);
            }

            OnPropertyChanged("CanMoveForward");
            OnPropertyChanged("CanMoveBackward");
            OnPropertyChanged("CurrentPageIndex");
        }

        /// <summary>
        /// Navigates the model backward.
        /// </summary>
        public void MoveBackward()
        {
            if (currentDocument != null)
            {
                currentDocument.Navigator.MoveBackward();
            }
        }

        /// <summary>
        /// Navigates the model forward.
        /// </summary>
        public void MoveForward()
        {
            if (currentDocument != null)
            {
                currentDocument.Navigator.MoveForward();
            }
        }

        #endregion

        #region Zooming

        /// <summary>
        /// Zooms in.
        /// </summary>
        public void ZoomIn()
        {
            if (CanZoomIn)
            {
                ++zoomIndex;

                OnPropertyChanged("CanZoomIn");
                OnPropertyChanged("CanZoomOut");
                OnPropertyChanged("Zoom");
            }
        }

        /// <summary>
        /// Zooms out.
        /// </summary>
        public void ZoomOut()
        {
            if (CanZoomOut)
            {
                --zoomIndex;

                OnPropertyChanged("CanZoomIn");
                OnPropertyChanged("CanZoomOut");
                OnPropertyChanged("Zoom");
            }
        }

        #endregion
    }
}
