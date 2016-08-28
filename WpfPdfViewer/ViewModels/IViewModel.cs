namespace Apitron.WpfPdfViewer.ViewModels
{
    using System.ComponentModel;

    /// <summary>
    /// This interface represents an abstract view model.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        void Invalidate();

        /// <summary>
        /// Freezes this instance.
        /// </summary>
        void Freeze();
    }
}