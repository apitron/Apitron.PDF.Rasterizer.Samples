using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Apitron.WpfPdfViewer.Annotations;

namespace Apitron.WpfPdfViewer.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Apitron.WpfPdfViewer.ViewModels;
   
    /// <summary>
    /// Interaction logic for PdfPage.xaml
    /// </summary>
    public partial class PdfPage : UserControl, IControl<PageViewModel>
    {
        public PdfPage()
        {
            this.InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (this.ViewModel != null && this.ViewModel.Page != null)
            {
                this.ViewModel.Invalidate();
            }
        }

        public void MakeIndicatorVisible()
        {
            loadingIndicator.Visibility = Visibility.Visible;
        }

        #region Implementation of IControl

        /// <summary>
        /// Gets the data context.
        /// </summary>
        public PageViewModel ViewModel
        {
            get { return this.DataContext as PageViewModel; }
            set { this.DataContext = value; }
        }

        #endregion
            
    }
}