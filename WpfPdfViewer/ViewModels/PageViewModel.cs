using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.WpfPdfViewer.Annotations;

namespace Apitron.WpfPdfViewer.ViewModels
{
    /// <summary>
    /// This class represents a pdf page view model.
    /// </summary>
    public class PageViewModel : IViewModel
    {
        #region Fields

        private Page page;
        private readonly PDF.Kit.FixedLayout.Page kitPage;

        private Task task;

        private BitmapSource source;

        CancellationTokenSource tokenSource;

        private bool rendered;

        static int quality = 1;

        private byte[] image;

        private ObservableCollection<IAnnotationViewModel> annotations;
        private bool drawText;
        private bool drawImages;
        private bool drawPaths;
        private bool drawAnnotations;
        private bool drawAnnotationsText;


        #endregion

        #region Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PageViewModel(Page page, PDF.Kit.FixedLayout.Page kitPage)
        {
            this.page = page;
            this.kitPage = kitPage;
            this.OnPropertyChanged("Width");
            this.OnPropertyChanged("Height");

            if (this.kitPage != null && this.page != null)
            {
                this.annotations = new ObservableCollection<IAnnotationViewModel>();

//                foreach (Annotation annotation in this.kitPage.Annotations)
//                {
//                    this.annotations.Add(new AnnotationViewModel(annotation, this.page));
//                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width
        {
            get { return this.page != null ? (int) this.page.Width*quality : 0; }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height
        {
            get { return this.page != null ? (int) this.page.Height*quality : 0; }
        }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public Page Page
        {
            get { return this.page; }
        }

        public ObservableCollection<IAnnotationViewModel> Annotations
        {
            get { return this.annotations; }
        }

        /// <summary>
        /// Gets the image source.
        /// </summary>
        public BitmapSource ImageSource
        {
            get
            {
                if (!this.rendered || task == null)
                {
                    return null;
                }

                return this.source;
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Implementation of IViewModel

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        public void Invalidate()
        {
            if (this.rendered)
            {
                return;
            }
            this.rendered = true;

            this.OnPropertyChanged("Width");
            this.OnPropertyChanged("Height");
            this.OnPropertyChanged("ImageSource");

            int desiredWidth = Width;
            int desiredHeight = Height;
            if (this.tokenSource != null)
            {
                this.tokenSource.Cancel();
            }
            this.tokenSource = new CancellationTokenSource();
            this.tokenSource.Token.ThrowIfCancellationRequested();
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            this.image = new byte[desiredWidth*desiredHeight*4];

            RenderingSettings renderingSettings = new RenderingSettings();
            renderingSettings.DrawText = this.DrawText;
            renderingSettings.DrawImages = this.DrawImages;
            renderingSettings.DrawPaths = this.DrawPaths;
            renderingSettings.DrawAnotations = this.DrawAnnotations;
            renderingSettings.AnnotationRenderingSettings = new RenderingSettings();
            renderingSettings.AnnotationRenderingSettings.DrawText = this.DrawAnnotationsText;
            // Uncomment to not draw annotations
            // renderingSettings.DrawAnotations = false;

            Task renderingTask = new Task(() => this.page.RenderAsBytes(desiredWidth, desiredHeight, this.image, renderingSettings));

            this.task = Task.Factory.StartNew(
                () =>
                {
                    renderingTask.Start();
                    while (!(renderingTask.IsCompleted || renderingTask.IsCanceled || renderingTask.IsFaulted))
                    {
                        if (tokenSource.IsCancellationRequested)
                        {
                            renderingSettings.CancelRendering();
                            this.rendered = false;
                            this.task = null;
                            this.source = null;
                            this.tokenSource = null;
                            this.image = null;
                            break;
                        }
                        renderingTask.Wait(1000);
                        dispatcher.Invoke(DispatcherPriority.DataBind, new Action(() =>
                        {
                            source = BitmapSource.Create(this.Width, this.Height, 72, 72, PixelFormats.Bgra32, null,
                                image, 4*this.Width);
                            this.OnPropertyChanged("ImageSource");
                        }));
                    }
                    this.image = null;
                    dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (this.source != null)
                        {
                            this.source.Freeze();
                        }
                    }));
                    GC.Collect();
                }, this.tokenSource.Token);
        }

        public bool DrawText
        {
            get
            {
                return drawText;
            }
            set
            {
                drawText = value;
                OnPropertyChanged(nameof(DrawText));
            }
        }
        public bool DrawPaths
        {
            get
            {
                return drawPaths;
            }
            set
            {
                drawPaths = value;
                OnPropertyChanged(nameof(DrawPaths));
            }
        }

        public bool DrawImages
        {
            get
            {
                return drawImages;
            }
            set
            {
                drawImages = value;
                OnPropertyChanged(nameof(DrawImages));
            }
        }

        public bool DrawAnnotations
        {
            get
            {
                return drawAnnotations;
            }
            set
            {
                drawAnnotations = value;
                OnPropertyChanged(nameof(DrawAnnotations));
            }
        }

        public bool DrawAnnotationsText
        {
            get
            {
                return drawAnnotationsText;
            }
            set
            {
                drawAnnotationsText = value;
                OnPropertyChanged(nameof(DrawAnnotationsText));
            }
        }

        /// <summary>
        /// Freezes this instance.
        /// </summary>
        public void Freeze()
        {
            if (this.task != null && this.tokenSource != null)
            {
                if (!task.IsCompleted)
                {
                    this.tokenSource.Cancel();
                    GC.Collect();
                }
            }
        }

        #endregion
    }
}