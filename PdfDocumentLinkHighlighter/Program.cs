using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Navigation;
using Apitron.PDF.Rasterizer.Search;
using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

namespace PdfDocumentLinkHighlighter
{
    class Program
    {
        static void Main(string[] args)
        {
            // prepare graphics objects
            Bitmap renderedPage = null;
            Brush highlightBrush = new SolidBrush(Color.FromArgb(126, 255, 255, 0));

            //  store rendering settings
            Resolution renderingResolution = new Resolution(144, 144);
            RenderingSettings renderingSettings = new RenderingSettings();
            Page firstPage = null;

            // a list of rects to highlight
            IList<RectangleF> highlightRects = new List<RectangleF>();

            // open PDF document
            using (FileStream fs = new FileStream("../../files/test.pdf", FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite))
            {                
                using (Document doc = new Document(fs))
                {
                    firstPage = doc.Pages[0];

                    renderedPage = firstPage.Render(renderingResolution, renderingSettings);

                    // parse links and store highlight rects
                    foreach (Link link in firstPage.Links)
                    {
                        if (link.IsUriLink)
                        {
                            Apitron.PDF.Rasterizer.Rectangle locationRect =
                                link.GetLocationRectangle(renderingResolution, renderingSettings);

                            highlightRects.Add(TransformToGDIRect(locationRect, renderedPage.Height));                          

                            Console.WriteLine(link.DestinationUri);
                        }
                    }
                }
            }

            // search text in the same document using regular expression matching URLs
            using (SearchIndex search = new SearchIndex(new FileStream("../../files/test.pdf", FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite)))
            {
                search.Search(handlerArgs =>
                {
                    // first page only
                    if (handlerArgs.PageIndex > 0)
                    {
                        handlerArgs.CancelSearch = true;
                        return;
                    }

                    // add highlight rects by processing found items
                    foreach (SearchResultItem item in handlerArgs.ResultItems)
                    {
                        SearchResultRegion searchResultRegion = firstPage.TransformRegion(item.Region, renderingResolution,
                            renderingSettings);
                        
                        foreach (double[] block in searchResultRegion.Blocks)
                        {
                            float xMin = float.MaxValue;
                            float yMin = float.MaxValue;
                            float xMax = float.MinValue;
                            float yMax = float.MinValue;
                            for (int i = 0; i < block.Length;)
                            {
                                xMin = (float)Math.Min(xMin, block[i]);
                                xMax = (float)Math.Max(xMax, block[i++]);

                                yMin = (float)Math.Min(yMin, block[i]);
                                yMax = (float)Math.Max(yMax, block[i++]);
                            }

                            highlightRects.Add(new RectangleF(xMin, yMin, xMax-xMin, yMax-yMin));
                        }

                        Console.WriteLine(item.Title);
                    }
                },
                new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"));
            }

            // render hightllight rects
            HighlightRects(renderedPage, highlightRects, highlightBrush);

            renderedPage.Save("renderedPage.png");
            Process.Start("renderedPage.png");

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
        
        /// <summary>
        /// Highlights a list of rects.
        /// </summary>
        private static void HighlightRects(Bitmap renderedPage, IList<RectangleF> highlightRects, Brush highlightBrush)
        {
            using (Graphics g = Graphics.FromImage(renderedPage))
            {
                foreach (RectangleF rect in highlightRects)
                {
                    g.FillRectangle(highlightBrush, rect);
                }
            }
        }

        /// <summary>
        /// Transforms PDF rect to GDI rect.
        /// </summary>
        /// <param name="locationRect">Rect to transform.</param>
        /// <param name="height">The height of the page.</param>
        /// <returns>Transformed GDI rect.</returns>
        private static RectangleF TransformToGDIRect(Rectangle locationRect, double height)
        {
            return new RectangleF((float)locationRect.Left, (float)(height - locationRect.Top),
                (float)locationRect.Width, (float)locationRect.Height);
        }
    }
}
