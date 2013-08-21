namespace RenderingSettingsUsage
{
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
                // this objects represents a PDF document
                Document document = new Document(fs);

                // intialize settings object
                RenderingSettings settings = new RenderingSettings();

                // i want a special background for my pages
                settings.BackgroundColor = (uint) Color.LawnGreen.ToArgb();

                // annotaions objects like notes, will be drawn
                settings.DrawAnotations = true;

                // images will be drawn as well
                settings.DrawImages = true;

                // text on
                settings.DrawText = true;

                // i want page to be turned 90 degrees clockwise
                settings.RotationAngle = RotationAngle.Rotate90;

                // i want page content to fit undistorted, so let's preserve an aspect ratio
                settings.ScaleMode = ScaleMode.PreserveAspectRatio;

                // process and save pages one by one
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];

                    // we use original page's width and height for image as well as default rendering settings
                    using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, settings))
                    {
                        bitmap.Save(string.Format("{0}.png", i), ImageFormat.Png);
                    }
                }
            }
        }
    }
}
