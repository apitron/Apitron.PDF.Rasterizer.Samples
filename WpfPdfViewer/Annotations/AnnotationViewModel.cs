using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apitron.PDF.Kit.FixedLayout;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.WpfPdfViewer.Controls;
using Page = Apitron.PDF.Rasterizer.Page;

namespace Apitron.WpfPdfViewer.Annotations
{
    public class AnnotationViewModel : IAnnotationViewModel, INotifyPropertyChanged
    {
        private Rect rect;
        private Annotation annotation;
        private double baseHeight;
        private ApitronCommand mouseDown;
        private ApitronCommand mouseUp;
        private ApitronCommand mouseMove;
        private Point mousePoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationViewModel"/> class.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public AnnotationViewModel(Annotation annotation, Page page)
        {
            this.annotation = annotation;
            baseHeight = page.Height;

            this.rect = new Rect(annotation.Boundary.Left, baseHeight - annotation.Boundary.Top,
                annotation.Boundary.Width, annotation.Boundary.Height);

            mouseDown = new ApitronCommand((o)=> true, MouseDownHandler);
            mouseUp = new ApitronCommand((o)=> true, MouseUpHandler);
            mouseMove = new ApitronCommand((o)=> true, MouseMoveHandler);
        }

        #region Implementation of IAnnotationView

        /// <summary>
        /// Gets the rect.
        /// </summary>
        /// <value>
        /// The rect.
        /// </value>
        public Rect Rect
        {
            get { return rect; }
        }

        public Annotation Annotation
        {
            get { return this.annotation; }
        }

        #endregion

        public void Translate(double dx, double dy)
        {
            annotation.Boundary = new Boundary(annotation.Boundary.Left+dx,annotation.Boundary.Bottom-dy,annotation.Boundary.Right+dx,annotation.Boundary.Top-dy);

            this.rect = new Rect(annotation.Boundary.Left, baseHeight - annotation.Boundary.Top,
               annotation.Boundary.Width, annotation.Boundary.Height);

            OnPropertyChanged("Rect");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand MouseDown
        {
            get
            {
                return mouseDown;
            }
        }

        public ICommand MouseUp
        {
            get { return mouseUp; }
        }

        public ICommand MouseMove
        {
            get
            {
                return mouseMove;
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        private void MouseDownHandler(object e)
        {
            
            Control control = (Control) e;
            PdfPage page = FindParent<PdfPage>(control);

            page.Cursor = Cursors.Hand;

            mousePoint = Mouse.GetPosition(page);
            control.CaptureMouse();
        }

        private void MouseUpHandler(object e)
        {
            Control control = (Control)e;
            PdfPage page = FindParent<PdfPage>(control);

            page.Cursor = Cursors.Arrow;

            control.ReleaseMouseCapture();
            control.Focus();
        }

        private void MouseMoveHandler(object e)
        {
            Control control = (Control)e;

            if (control.IsMouseCaptured)
            {
                PdfPage page = FindParent<PdfPage>(control);

                Point newPoint = Mouse.GetPosition(page);

                Translate(newPoint.X - mousePoint.X, newPoint.Y - mousePoint.Y);

                mousePoint = newPoint;
            }
        }
    }
}