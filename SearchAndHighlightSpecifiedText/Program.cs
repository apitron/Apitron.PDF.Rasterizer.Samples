using System.Diagnostics;
using System.Drawing;
using System.IO;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.Search;

namespace SearchAndHighlightSpecifiedText
{
    class Program
    {
        /// <summary>
        ///   The brush to highlight a text.
        /// </summary>
        private static readonly SolidBrush markBrush = new SolidBrush(Color.FromArgb(100, Color.Yellow));

        private static readonly RenderingSettings renderingSettings = new RenderingSettings();
        private static Document document;

        static void Main(string[] args)
        {
            string pathToDocument = @"..\..\..\Documents\testfile.pdf";

            // create index from PDF file
            using (Stream pdfDocumentStreamToSearch = new FileStream(pathToDocument, FileMode.Open, FileAccess.Read))
            {
                SearchIndex searchIndex = new SearchIndex(pdfDocumentStreamToSearch);

                // create document used for rendering
                using (Stream pdfDocumentStreamToRasterize = new FileStream(pathToDocument, FileMode.Open, FileAccess.Read))
                {
                    document = new Document(pdfDocumentStreamToRasterize);

                    // search text in PDF document and render pages containg results
                    searchIndex.Search(SearchHandler, "testing one");
                }
            }
        }

        /// <summary>
        ///   Drawing the pages with highlighted text.
        /// </summary>
        /// <param name="handlerArgs"> The handler args. </param>
        private static void SearchHandler(SearchHandlerArgs handlerArgs)
        {
            if (handlerArgs.ResultItems.Count != 0)
            {
                string outputFileName = $"{handlerArgs.PageIndex}.png";

                Page page = document.Pages[handlerArgs.PageIndex];
                using (Image bm = page.Render((int) page.Width*2, (int) page.Height*2, renderingSettings))
                {
                    foreach (SearchResultItem searchResultItem in handlerArgs.ResultItems)
                    {
                        HighlightSearchResult(bm, searchResultItem, page);
                    }

                    bm.Save(outputFileName);
                }

                Process.Start(outputFileName);
            }

            // Search cancellation condition, now we stop if we have more than 3 results found,
            // or all pages are searched
            if (handlerArgs.ResultItems.Count > 3)
            {
                handlerArgs.CancelSearch = true;
            }
        }

        /// <summary>
        ///   Highlights the search result.
        /// </summary>
        /// <param name="bm"> The bitmap. </param>
        /// <param name="searchResultItem"> The search result item. </param>
        /// <param name="page"> The page. </param>
        private static void HighlightSearchResult(Image bm, SearchResultItem searchResultItem, Page page)
        {
            using (Graphics gr = Graphics.FromImage(bm))
            {
                double[] rectangle;
                SearchResultRegion region = page.TransformRegion(searchResultItem.Region, bm.Width, bm.Height, renderingSettings);
                foreach (double[] item in region.Blocks)
                {
                    rectangle = item;
                    PointF[] points = new PointF[rectangle.Length/2];
                    for (int i = 0; i < 4; i++)
                    {
                        points[i] = new PointF((float) rectangle[i*2], (float) rectangle[(i*2) + 1]);
                    }
                    gr.FillPolygon(markBrush, points);
                }
            }
        }
    }
}
