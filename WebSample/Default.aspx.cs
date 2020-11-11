using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;

namespace WebSample
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (FileStream stream = new FileStream(Server.MapPath("~\\App_Code\\Documents\\testfile.pdf"), FileMode.Open,FileAccess.Read))
            {
                Document document = new Document(stream);
                using (MemoryStream ms = new MemoryStream())
                {
                    //get the first page and render it into JPEG
                    Page page = document.Pages[0];
                    Bitmap bmp = page.Render((int) (page.Width), (int) (page.Height), new RenderingSettings());
                    bmp.Save(ms, ImageFormat.Jpeg);
                    
                    Response.ContentType = "image/jpeg";
                    Response.AddHeader("content-disposition", "inline; filename=MyDocument.jpeg");
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
        }
    }
}