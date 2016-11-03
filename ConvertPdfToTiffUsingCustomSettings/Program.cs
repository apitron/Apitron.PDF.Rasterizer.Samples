namespace ConvertPdfToTiffUsingCustomSettings
{
    using System;
    using System.IO;
    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

    internal class Program
    {
        const int DPI = 144;

        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\map.pdf", FileMode.Open), fsOut = File.Create("out.tiff"))
            {
                // this objects represents a PDF document
                using (Document document = new Document(fs))
                {
                    // save to tiff using CCIT4 compression, black and white tiff.
                    // set the DPI to 144.0 for this sample, so twice more than default PDF dpi setting.
                    TiffRenderingSettings tiffRenderingSettings = new TiffRenderingSettings(TiffCompressionMethod.LZW,
                        DPI, DPI);
                    tiffRenderingSettings.RenderMode = RenderMode.HighQuality;
                    tiffRenderingSettings.ScaleMode = ScaleMode.PreserveAspectRatio;
                    tiffRenderingSettings.Compression = TiffCompressionMethod.LZW;

                    // subscribe to before rendering event and adjust rendering settings 
                    // for each page as you want 
                    tiffRenderingSettings.BeforeRenderPage += TiffRenderingSettings_BeforeRenderPage;

                    document.SaveToTiff(fsOut, tiffRenderingSettings);
                }

                System.Diagnostics.Process.Start("out.tiff");
            }
        }
         
        private static void TiffRenderingSettings_BeforeRenderPage(BeforeRenderPageEventArgs args)
        {
            // skip all pages expect first two
            if (args.PageIndex > 1)
            {
                args.IsPageSkipped = true;
                return;
            }

            // force the portrait mode for all pages if needed
            if (args.DesiredHeight < args.DesiredWidth)
            {
                args.RenderingSettings.RotationAngle = RotationAngle.Rotate270;
            }

           // crop from the all sides by cutting out existing margins
           double margin = 40;
           args.CropBox = new Rectangle(margin, margin, args.DesiredWidth - margin, args.DesiredHeight - margin);
        }
    }
}
