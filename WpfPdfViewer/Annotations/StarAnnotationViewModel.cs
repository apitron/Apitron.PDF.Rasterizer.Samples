using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.PDF.Rasterizer;

namespace Apitron.WpfPdfViewer.Annotations
{
    public class StarAnnotationViewModel : ImageBasedAnnotationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationViewModel"/> class.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public StarAnnotationViewModel(Annotation annotation, Page page) : base(annotation, page)
        {
        }

        #region Overrides of AnnotationViewModel

        protected override string ImageFileName
        {
            get { return "Resources/star.png"; }
        }

        #endregion
    }
}