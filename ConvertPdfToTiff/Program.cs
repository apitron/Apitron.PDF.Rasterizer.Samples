namespace ConvertPdfToTiff
{
    using System.IO;
    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // open and load the file
            using (FileStream fs = new FileStream(@"..\..\..\..\development guide.pdf", FileMode.Open), fsOut = File.Create("out.tiff"))
			using (Document document = new Document(fs)) // this object represents a PDF document
            {
                // save to tiff using CCIT4 compression, black and white tiff.
                // set the DPI to 144.0 for this sample, so twice more than default PDF dpi setting.
                TiffRenderingSettings tiffRenderingSettings = new TiffRenderingSettings(TiffCompressionMethod.CCIT4, 144, 144);

                // set the white color brightness level 
                tiffRenderingSettings.WhiteColorTolerance = 0.9f;
                
                document.SaveToTiff(fsOut, tiffRenderingSettings);

                System.Diagnostics.Process.Start("out.tiff");
            }
        }
    }
}
