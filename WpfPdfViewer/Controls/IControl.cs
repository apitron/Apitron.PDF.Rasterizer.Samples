namespace Apitron.WpfPdfViewer.Controls
{
    using Apitron.WpfPdfViewer.ViewModels;

    /// <summary>
    /// This interface represents an abstract control
    /// </summary>
    public interface IControl<TView> where TView : IViewModel
    {
        /// <summary>
        /// Gets the data context.
        /// </summary>
        TView ViewModel { get; }
    }
}