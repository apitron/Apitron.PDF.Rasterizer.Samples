using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.WpfPdfViewer.Controls;
using Page = Apitron.PDF.Rasterizer.Page;

namespace Apitron.WpfPdfViewer.Annotations
{
    public class TextAnnotationViewModel : AnnotationViewModel
    {
        private ApitronCommand textChanged;

        public string Text
        {
            get
            {
                return Annotation.Contents;
            }
            set
            {
                Annotation.Appearance.Normal = Annotations.AnnotationsHelper.CreateNormalAppearance(value,
                    Annotation.Boundary.Width, Annotation.Boundary.Height);

                Annotation.Contents = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationViewModel"/> class.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public TextAnnotationViewModel(Annotation annotation, Page page) : base(annotation, page)
        {
        }
    }
}