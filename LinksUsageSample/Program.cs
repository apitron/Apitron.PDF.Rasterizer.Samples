using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LinksUsageSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\LinksUsageSample.pdf", FileMode.Open))
            {
                // this object represents a PDF document
                using(Document document = new Document(fs))
                {
                    // get the first page and read the links from it
                    Page currentPage = document.Pages[0];

                    RenderingSettings settings = new RenderingSettings();

                    // render first page with all links hightlighted
                    SavePageAsBitmapWithHightlightedLinks(currentPage, settings);

                    // enumerate through the page links collection and render linked pages
                    for( int i = 0; i < currentPage.Links.Count; i++ )
                    {
                        // try to navigate using given link and save linked page as bitmap
                        if (document.Navigator.GoToLink(currentPage.Links[i]))
                        {
                            // we use default PDF dpi settings for image as well as default rendering settings
                            using (Bitmap bitmap = document.Navigator.CurrentPage.Render(new Resolution(72,72), settings))
                            {
                                bitmap.Save($"{i}.png", ImageFormat.Png);
                            }
                        }
                    }

                    // preview first rendered page
                    Process.Start("Page_with_highlighted_links.png");
                }
            }
        }

        private static void SavePageAsBitmapWithHightlightedLinks(Page page, RenderingSettings settings)
        {
            int pageWidth = (int)page.Width;
            int pageHeight = (int)page.Height;

            // render page
            using (Bitmap bitmap = page.Render(pageWidth,pageHeight, settings))
            {
                // create graphics object that we will use for drawing of the highlighting rects
                using (Graphics g = Graphics.FromImage(bitmap))
                {                   
                    using(Brush highlightBrush = new SolidBrush(Color.FromArgb(0x5FFFFF00)))
                    {
                        foreach (Link link in page.Links)
                        {
                            Apitron.PDF.Rasterizer.Rectangle linkLocation = link.GetLocationRectangle(pageWidth,pageHeight,settings);

                            // PDF coordinate system has Y axis inverted in comparison to GDI, so transform the Y coordinate of the rect here
                            // because link object coordinates will be returned using PDF coordinate system.
                            g.FillRectangle(highlightBrush, new RectangleF((float) linkLocation.Left, pageHeight-(float) linkLocation.Top,(float) linkLocation.Width,(float) linkLocation.Height));
                        }
                    }
                }

                bitmap.Save("Page_with_highlighted_links.png", ImageFormat.Png);
            }
        }
    }
}
