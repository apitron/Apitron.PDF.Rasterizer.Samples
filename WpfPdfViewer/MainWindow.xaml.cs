using System;
using Apitron.PDF.Kit;
using Apitron.PDF.Kit.FixedLayout;
using Apitron.PDF.Kit.FixedLayout.Resources;
using Apitron.PDF.Kit.FixedLayout.Resources.Fonts;
using Apitron.PDF.Kit.FixedLayout.Resources.XObjects;
using Apitron.PDF.Kit.Interactive.Annotations;
using Apitron.PDF.Kit.Styles;
using Apitron.PDF.Kit.Styles.Appearance;
using Apitron.WpfPdfViewer.Annotations;

namespace Apitron.WpfPdfViewer
{
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;
    using Apitron.PDF.Rasterizer.Navigation;
    using Microsoft.Win32;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Input;
    using Apitron.WpfPdfViewer.ViewModels;
    using Page = Apitron.PDF.Rasterizer.Page;
    using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private delegate void SetImageSourceDelegate(
            byte[] source, IList<Link> links, int width, int height, Image destination);

        public DocumentViewModel document = null;

        private Task task;

        private int GlobalScale = 1;

        private Rectangle destinationRectangle;

        private ResourceManager resourceManager;

        #endregion

        #region Ctors

        public MainWindow()
        {
            this.InitializeComponent();
            this.document = new DocumentViewModel();
            this.DataContext = this.document;
            this.document.PropertyChanged += this.DocumentOnPropertyChanged;

            Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image star =
                new Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image("star", "Resources//star.png", true);
            Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image smile =
                new Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image("smile", "Resources//smile.png", true);

            this.resourceManager = new ResourceManager();
            resourceManager.RegisterResource(star);
            resourceManager.RegisterResource(smile);
        }

        #endregion

        #region Event Handlers

        private void OnOpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PDF Files (*.pdf, *.PDF)|*.pdf;*.PDF";
            bool? dialogResult = dialog.ShowDialog(this);
            if (dialogResult.Value)
            {
                using (
                    FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    InitDocument(stream);
                }
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnBookmarkSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Bookmark newValue = (Bookmark) e.NewValue;
            if (newValue != null)
            {
                this.document.Document.Navigator.GoToBookmark(newValue);
                this.destinationRectangle =
                    newValue.GetDestinationRectangle((int) (this.document.Page.Width*this.GlobalScale),
                        (int) (this.document.Page.Height*this.GlobalScale), null);
            }
        }


        private void OnNavigationButtonClick(object sender, RoutedEventArgs e)
        {
            Button source = (Button) e.Source;
            Document doc = this.document.Document;
            DocumentNavigator navigator = doc == null ? null : doc.Navigator;
            if (doc == null || navigator == null)
            {
                return;
            }
            switch ((string) source.CommandParameter)
            {
                case "Next":
                    navigator.MoveForward();
                    break;
                case "Prev":
                    navigator.MoveBackward();
                    break;
                case "First":
                    navigator.Move(0, Origin.Begin);
                    break;
                case "Last":
                    navigator.Move(0, Origin.End);
                    break;
                default:
                    return;
            }
            this.destinationRectangle = null;
        }


        private void DocumentOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
        }

        private void OnAboutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(this,
                "This sample shows how to preview PDF and add annotations providing visual feedback and ability to save the result.",
                "WPF PDF Viewer sample by Apitron");
        }

        protected bool IsFullyOrPartiallyVisible(FrameworkElement child, FrameworkElement scrollViewer)
        {
            var childTransform = child.TransformToAncestor(scrollViewer);
            var childRectangle = childTransform.TransformBounds(new Rect(new Point(0, 0), child.RenderSize));
            var ownerRectangle = new Rect(new Point(0, 0), scrollViewer.RenderSize);
            return ownerRectangle.IntersectsWith(childRectangle);
        }

        #endregion

        #region PDF manipulations and annotations related code

        private void AddAnnotation(object sender, RoutedEventArgs e)
        {
            // add annotation based on button's tag value.
            Button button = (Button) sender;
            string actionName = button.Tag as string;
            if (this.document != null && this.document.NativePage != null && this.document.Page != null)
            {
                if (actionName == "star")
                {
                    Annotation starAnnotation = new RubberStampAnnotation(new Boundary(10, 400, 50, 440),
                        AnnotationFlags.Default);
                    FixedContent fixedContent = new FixedContent("ap01", new Boundary(0, 0, 40, 40));
                    fixedContent.Content.AppendImage("star", 0, 0, 40, 40);
                    starAnnotation.Appearance.Normal = fixedContent;
                    this.document.NativePage.Annotations.Add(starAnnotation);

                    this.document.PageViewModel.Annotations.Add(new StarAnnotationViewModel(starAnnotation,
                        this.document.Page));
                }
                else if (actionName == "smile")
                {
                    Annotation starAnnotation = new RubberStampAnnotation(new Boundary(60, 400, 100, 440),
                        AnnotationFlags.Default);
                    FixedContent fixedContent = new FixedContent("ap01", new Boundary(0, 0, 40, 40));
                    fixedContent.Content.AppendImage("smile", 0, 0, 40, 40);
                    starAnnotation.Appearance.Normal = fixedContent;
                    this.document.NativePage.Annotations.Add(starAnnotation);

                    this.document.PageViewModel.Annotations.Add(new SmileAnnotationViewModel(starAnnotation,
                        this.document.Page));
                }
                else if (actionName == "text")
                {
                    FreeTextAnnotation textAnnotation = new FreeTextAnnotation(new Boundary(60, 400, 260, 500));

                    string sampleText = "sample text";

                    textAnnotation.Appearance.Normal = AnnotationsHelper.CreateNormalAppearance(sampleText, 200, 100);

                    // set properties affecting default appearance to be used as fallback
                    textAnnotation.FontSize = 12;
                    textAnnotation.BorderEffect = new AnnotationBorderEffect(AnnotationBorderEffectStyle.NoEffect, 0);
                    textAnnotation.Contents = sampleText;
                    // text and border color
                    textAnnotation.TextColor = RgbColors.White.Components;
                    // set  background here if needed
                    textAnnotation.Color = RgbColors.Green.Components;

                    this.document.NativePage.Annotations.Add(textAnnotation);

                    this.document.PageViewModel.Annotations.Add(new TextAnnotationViewModel(textAnnotation,
                        this.document.Page));
                }
            }
        }

        private void SaveChanges_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.document != null && this.document.NativePage != null && this.document.Page != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "PDF Files (*.pdf, *.PDF)|*.pdf;*.PDF";
                bool? dialogResult = dialog.ShowDialog(this);
                if (dialogResult.Value)
                {
                    // save the document
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        this.document.FixedDocument.Save(stream);
                    }

                    // ...and reload it
                    int pageIndex = this.document.Pages.IndexOf(this.document.Page);
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read,
                        FileShare.Read))
                    {
                        InitDocument(stream);
                    }

                    this.document.Document.Navigator.Move(pageIndex, Origin.Current);
                }
            }
        }

        /// <summary>
        /// Initialize the view model. Set both fixed document and document, the first is used for manipuluations, 
        /// while the second for rendering pages.
        /// </summary>
        /// <param name="stream"></param>
        private void InitDocument(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int) stream.Length);
            FixedDocument kitDocument = new FixedDocument(new MemoryStream(buffer), this.resourceManager);
            this.document.FixedDocument = kitDocument;

            Document document = new Document(new MemoryStream(buffer));
            this.document.Document = document;
        }

        #endregion
    }
}