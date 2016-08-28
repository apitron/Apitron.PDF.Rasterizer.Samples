using System.Windows;

namespace Apitron.WpfPdfViewer.Annotations
{
    /// <summary>
    /// This interface represents an abstract annotation.
    /// </summary>
    public interface IAnnotationViewModel
    {
        /// <summary>
        /// Gets the rect.
        /// </summary>
        /// <value>
        /// The rect.
        /// </value>
        Rect Rect { get; }
    }
}