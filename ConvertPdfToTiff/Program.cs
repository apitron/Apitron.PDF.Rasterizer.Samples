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
            using (FileStream fs = new FileStream(@"..\..\..\Documents\testfile.pdf", FileMode.Open), fsOut = File.Create("out.tiff"))
            {
                // this objects represents a PDF document
                Document document = new Document(fs);

                // save to tiff using CCIT4 compression, black and white tiff.
                // set the DPI to 144.0 for this sample, so twice more than default PDF dpi setting.
                TiffRenderingSettings tiffRenderingSettings = new TiffRenderingSettings(TiffCompressionMethod.LZW, 144, 144);

                document.SaveToTiff(fsOut, tiffRenderingSettings);

                System.Diagnostics.Process.Start("out.tiff");
            }
        }
    }
}
