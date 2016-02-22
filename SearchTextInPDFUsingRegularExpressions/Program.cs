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
using Apitron.PDF.Rasterizer.Search;

namespace SearchTextInPDFUsingRegularExpressions
{
    class Program
    {
        // global rendering settings
        static RenderingSettings renderingSettings = new RenderingSettings();
        // hightlight brush for search results
        static Brush hightlightBrush = new SolidBrush(Color.FromArgb(100,255,255,0));

        static void Main(string[] args)
        {
            // the source file to search the text into
            string inputFilePath = "../../data/Apitron_Pdf_Kit_in_Action.pdf";            

            // open pdf document for search and rendering
            // we'll use 2 different streams here
            using (Stream searchStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read),
                documentStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {                
                // create search object from PDF data stream
                using (SearchIndex searchIndex = new SearchIndex(searchStream))
                {
                    // open document to be used for rendering
                    using (Document doc = new Document(documentStream))
                    {
                        searchIndex.Search((handlerArgs =>
                        {
                            // if we have results
                            if (handlerArgs.ResultItems.Count != 0)
                            {
                                // create resulting image filename
                                string outputFileName = string.Format("{0}_{1}.png",
                                    Path.GetFileNameWithoutExtension(inputFilePath), handlerArgs.PageIndex);

                                // render found result and start system image viewer
                                Page page = doc.Pages[handlerArgs.PageIndex];
                                using (Image bitmap = page.Render(new Resolution(96, 96), renderingSettings))
                                {
                                    foreach (SearchResultItem searchResultItem in handlerArgs.ResultItems)
                                    {
                                        HighlightSearchResult(bitmap, searchResultItem, page);
                                    }

                                    bitmap.Save(outputFileName);
                                }

                                Process.Start(outputFileName);
                            }

                        }),
                        // find everything that matches [WORD][whitespaces]Kit pattern
                        new Regex("\\w+\\s+Kit"));                         
                    } 
                }
            }
        }
       
        /// <summary>
        ///  Highlights the search result.
        /// </summary>
        /// <param name="bitmap"> The bitmap. </param>
        /// <param name="searchResultItem"> The search result item. </param>
        /// <param name="page"> The page. </param>
        private static void HighlightSearchResult(Image bitmap, SearchResultItem searchResultItem, Page page)
        {
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                double[] rectangle;
                SearchResultRegion region = page.TransformRegion(searchResultItem.Region, bitmap.Width, bitmap.Height, renderingSettings);
                foreach (double[] item in region.Blocks)
                {
                    rectangle = item;
                    PointF[] points = new PointF[rectangle.Length / 2];
                    for (int i = 0; i < 4; i++)
                    {
                        points[i] = new PointF((float)rectangle[i * 2], (float)rectangle[(i * 2) + 1]);
                    }
                    gr.FillPolygon(hightlightBrush, points);
                }
            }
        }
    }    
}
