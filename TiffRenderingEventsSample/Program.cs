using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using System;
using System.IO;


namespace TiffRenderingEventsSample
{
    class Program
    {
        #region Fields
        private const string inputFileName = @"..\..\..\Documents\testfile.pdf";
        static RotationAngle currentRotate = RotationAngle.Rotate0;
        static Resolution currentResolution = new Resolution(72, 72); 
        #endregion

        static void Main(string[] args)
        {
            using (FileStream stream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
            {
                using (Document document = new Document(stream))
                {
                    TiffRenderingSettings settings = new TiffRenderingSettings();
                    settings.Compression = TiffCompressionMethod.LZW;
					
                    #region Before Rendering Page
                    
                    settings.BeforeRenderPage += Settings_BeforeRenderPage;                    
                    ProcessPdfDocument(document, settings, "out_beforeevent.tiff");
                    System.Diagnostics.Process.Start("out_beforeevent.tiff");

                    #endregion

                    #region Standard convert to bitonal

                    settings.Compression = TiffCompressionMethod.CCIT4;
                    ProcessPdfDocument(document, settings, "out_standardbitonal.tiff");
                    System.Diagnostics.Process.Start("out_standardbitonal.tiff");

                    #endregion

                    #region Custom convert to bitonal

                    ConvertToBitonalDelegate convertDelegate = ConvertToBitonal;
                    settings.ConvertToBitonal = convertDelegate;
                    ProcessPdfDocument(document, settings, "out_cuctombitonal.tiff");
                    System.Diagnostics.Process.Start("out_cuctombitonal.tiff");

                    #endregion

                    #region After Rendering Page
                    settings.Compression = TiffCompressionMethod.LZW;
                    settings.AfterRenderPage += Settings_AfterRenderPage;
                    ProcessPdfDocument(document, settings, "out_afterrendering.tiff");
                    System.Diagnostics.Process.Start("out_afterrendering.tiff");

                    #endregion
                }
            }
        }

        private static void Settings_AfterRenderPage(AfterRenderPageEventArgs args)
        {
            // BGRA
            byte[] result = args.ImageData;
            int scanLineLength = args.Width * 4;

            // Remove Blue channel
            for (int y = 0; y < args.Height; y++)
            {
                int startPoint = y * scanLineLength;
                int stopPoint = startPoint + scanLineLength;
                for (int x = startPoint; x < stopPoint; x += 4)
                {
                    // Reset blue channel - i.e White bacome Yellow
                    result[x] = 0;                    
                }
            }
        }

        private static void Settings_BeforeRenderPage(BeforeRenderPageEventArgs args)
        {
            // Change rotation every time
            switch (currentRotate)
            {
                case RotationAngle.Rotate0:
                    currentRotate = RotationAngle.Rotate90;
                    break;
                case RotationAngle.Rotate90:
                    currentRotate = RotationAngle.Rotate180;
                    break;
                case RotationAngle.Rotate180:
                    currentRotate = RotationAngle.Rotate270;
                    break;
                case RotationAngle.Rotate270:
                    currentRotate = RotationAngle.Rotate0;
                    break;
            }
            args.RenderingSettings.RotationAngle = currentRotate;

            // Change resolution every time
            if (currentResolution.DpiX == 72)
            {
                currentResolution = new Resolution(300, 300);
            }
            else
            {
                currentResolution = new Resolution(72, 72);
                args.DesiredWidth = args.DesiredWidth * 5;
                args.DesiredHeight = args.DesiredHeight * 5;
            }            
            args.Resolution = currentResolution;

            // Set crop box
            args.CropBox = new Rectangle(400, 600, args.DesiredWidth - 400, args.DesiredHeight - 600);
        }

        private static byte[] ConvertToBitonal(int width, int height, byte[] imageData, out int resultingWidth, out int resultingHeight)
        {
            resultingWidth = width;
            resultingHeight = height;

            // pixels format BGRA8888
            byte[] pixels = imageData;

            int scanlLineLength = ((width & 0x07) != 0 ? (width >> 3) + 1 : width >> 3);
            byte[] scanLine = new byte[scanlLineLength];
            int index = 0;
            for (int y = 0; y < height; ++y)
            {
                // Reset blue channel - i.e.White will become Yellow
                ConvertRowToBitonal(pixels, y, scanLine, width);
                Array.Copy(scanLine, 0, pixels, index, scanlLineLength);
                index += scanlLineLength;
            }
            Array.Resize(ref pixels, index);

            return pixels;
        }

        private static void ConvertRowToBitonal(byte[] pixels, int y, byte[] strip, int pixelWidth)
        {
            int offset = pixelWidth * 4 * y;

            byte buffer = 0;

            for (int i = 0; i < strip.Length; i++)
            {
                for (int j = 7; j >= 0 && pixelWidth > 0; --j, offset += 4, pixelWidth--)
                {
                    // Use only blue channel (pixels format BGRA8888)
                    buffer |= (byte)(((pixels[offset + 1] & 0x01) ^ 0x01) << j);
                }

                strip[i] = buffer;

                buffer = 0;
            }
        }

        private static void ProcessPdfDocument(Document document, TiffRenderingSettings settings, string outputFileName)
        {
            using (FileStream outputStream = File.Create(outputFileName))
            {
                document.SaveToTiff(outputStream, settings);
            }
        }
    }
}
