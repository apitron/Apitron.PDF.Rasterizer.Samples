using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;

namespace Apitron.Pdf.Rasterizer.WebServiceSample
{
    /// <summary>
    /// Summary description for RasterizationServiceasmx
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class RasterizationService : System.Web.Services.WebService
    {
        /// <summary>
        /// Gets an rendered page from specified PDF file. 
        /// </summary>
        /// <param name="pageIndex">Page index to be rendered.</param>
        /// <param name="filePath">File location.</param>
        [WebMethod]
        public void GetPageImage(int pageIndex, string filePath)
        {
            filePath = HttpContext.Current.Server.MapPath(filePath);
            if (System.IO.File.Exists(filePath))
            {
                using (Stream stream = new FileStream(filePath, FileMode.Open))
                {
                    Document document = new Document(stream);
                    Bitmap result = document.Pages[pageIndex].Render(new Resolution(96, 96), new RenderingSettings());

                    MemoryStream memoryStream = new MemoryStream();
                    result.Save(memoryStream, ImageFormat.Jpeg);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    this.Context.Response.ContentType = "image/jpeg";
                    this.Context.Response.BinaryWrite(memoryStream.GetBuffer());
                    this.Context.Response.Flush();
                }
            }
        }
    }
}
