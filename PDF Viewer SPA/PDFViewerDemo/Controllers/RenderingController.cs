using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Web;
using System.Web.Http;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using Apitron.PDF.Rasterizer.ErrorHandling;

namespace PDFViewerDemo.Controllers
{
    public class RenderingController : ApiController
    {
        private static readonly string filePath;

        static RenderingController()
        {
            filePath = HttpContext.Current.Server.MapPath("~/App_Data/Apitron_Pdf_Kit_in_Action.pdf");
        }

        /// <summary>
        /// Gets the rendered page as encoded image.
        /// </summary>
        /// <param name="id">Page index, zero-based.</param>
        /// <returns>String containing encoded image, or null if the call fails.</returns>
        public string GetRenderedPage(int id)
        {
            using (Document doc = new Document(File.OpenRead(filePath)))
            {
                if (id >= 0 && doc.Pages.Count > id)
                {
                    Bitmap bm = doc.Pages[id].Render(new Resolution(72, 72), new RenderingSettings());

                    if (bm != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bm.Save(ms, ImageFormat.Png);

                            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns document info.
        /// </summary>
        /// <returns>Valid document info object, or null if the call fails.</returns>
        public DocumentInfo GetFileInfo()
        {
            try
            {
                using (Document doc = new Document(File.OpenRead(filePath)))
                {
                    return new DocumentInfo() {PageCount = doc.Pages.Count};
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
