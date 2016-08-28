using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.PDF.Rasterizer;

namespace Apitron.WpfPdfViewer.Annotations
{
    public class ImageBasedAnnotationViewModel : AnnotationViewModel
    {
        private BitmapImage img;

        public ImageSource ImageSource
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ImageFileName))
                {
                    if (img == null)
                    {
                        FileStream fileStream = new FileStream(this.ImageFileName, FileMode.Open, FileAccess.Read,
                            FileShare.Read);
                        {
                            img = new System.Windows.Media.Imaging.BitmapImage();
                            img.BeginInit();
                            img.StreamSource = fileStream;
                            img.EndInit();
                        }
                    }
                    return img;
                }
                return null;
            }
        }

        protected virtual string ImageFileName
        {
            get { return String.Empty; }
        }

        public ImageBasedAnnotationViewModel(Annotation annotation, Page page) : base(annotation, page)
        {
        }
    }
}