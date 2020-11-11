using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;

namespace ConvertPdfUsingResolution
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\development guide.pdf", FileMode.Open))
			using (Document document = new Document(fs)) // this object represents a PDF document
            {
                // process and save pages one by one
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];

                    // we use 144 dpi resolution and default rendering settings
                    using (Bitmap bitmap = currentPage.Render(new Resolution(144, 144), new RenderingSettings()))
                    {
                        bitmap.Save($"{i}.bmp", ImageFormat.Bmp);
                    }
                }

                // preview first rendered page
                Process.Start("0.bmp");
            }
        }
    }
}
