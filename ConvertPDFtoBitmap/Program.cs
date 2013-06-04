namespace ConvertPDFtoBitmap
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\testfile.pdf", FileMode.Open))
            {
                // this object represents a PDF document
                Document document = new Document(fs);             

                // process and save pages one by one
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];

                    // we use original page's width and height for image as well as default rendering settings
                    using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, new RenderingSettings()))
                    {
                        bitmap.Save(string.Format("{0}.png", i), ImageFormat.Png);
                    }
                }

                // preview first rendered page
                Process.Start("0.png");
            }
        }
    }
}
