namespace PartialPageRendering
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;
    using Rectangle = Apitron.PDF.Rasterizer.Rectangle;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\Documents\testfile.pdf", FileMode.Open))
			using (Document document = new Document(fs)) // this object represents a PDF document
            {
                // process and save pages one by one
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];
                    
                    RenderingSettings settings = new RenderingSettings();
                    settings.BackgroundColor = (uint)Color.BlueViolet.ToArgb();

                    // we'd like to convert half of original PDF page
                    Rectangle rectangle = new Rectangle(0, currentPage.Width/2, (int)currentPage.Width, (int)currentPage.Height);

                    // we use original page's width and height for image
                    using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, rectangle, settings))
                    {
                        bitmap.Save($"{i}.png", ImageFormat.Png);
                    }
                }

                // preview first rendered page
                Process.Start("0.png");
            }
        }
    }
}
