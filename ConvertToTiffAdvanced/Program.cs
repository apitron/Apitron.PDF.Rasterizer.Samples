using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;

namespace ConvertToTiffAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Stream inputStream = File.OpenRead("../../data/document.pdf"),  
                outputStream = File.Create("out.tiff"))
            {
                using (Document doc = new Document(inputStream))
                {
                    doc.SaveToTiff(outputStream, new TiffRenderingSettings()
                    {
                        // set our conversion delegate
                        ConvertToBitonal = MyConvertToBitonal
                    });
                }
            }

            Process.Start("out.tiff");
        }

        private static byte[] MyConvertToBitonal(int width, int height, byte[] imageData, out int resultingWidth,
            out int resultingHeight)
        {
            // we will scale the resulting image, 
            // each source pixel will correspond to 64 black and white pixels
            int scaleFactor = 8;
            resultingWidth = width * scaleFactor;
            resultingHeight = height * scaleFactor;

            // create resulting data buffer
            byte[] resultingImage = new byte[width * resultingHeight];
            
            // source stride width in bytes
            double luminance;

            // black pixel goes first, lighter pixels follow
            byte[] pixel0 = new byte[] {0x7e, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7e };
            byte[] pixel1 = new byte[] {0x18, 0x3c, 0x7e, 0xFF, 0xFF, 0x7e, 0x3c, 0x18 };
            byte[] pixel2 = new byte[] {0, 0x18, 0x3c, 0x7e, 0x7e, 0x3c, 0x18, 0 };
            byte[] pixel3 = new byte[] {0, 0, 0x18, 0x3c, 0x3c, 0x18, 0, 0 };
            byte[] pixel4 = new byte[] {0, 0, 0, 0x18, 0x18, 0, 0, 0 };

            
            // iterate over the source pixels and modify bitonal image
            // according to own algorithm
            for (int y = 0, offsetY=0, stride=width*4; y < height; ++y, offsetY+=stride)
            {   
                // base offset for final pixel(s)
                int resultingPixelOffsetBase = y*width*scaleFactor;

                for (int x = 0, offsetX=0; x < width; ++x, offsetX+=4)
                {
                    // read pixel data
                    byte b = imageData[offsetY+offsetX];
                    byte g = imageData[offsetY+offsetX+1];
                    byte r = imageData[offsetY+offsetX+2];

                    // calculate the luminance
                    luminance = (0.2126*r + 0.7152*g + 0.0722*b);

                    // based on the luminance we'll select which pixel to use
                    // from the darkest one to the lightest
                    if (luminance < 50)
                    {
                        SetPixel(resultingImage, resultingPixelOffsetBase+x, pixel0, width);
                    }
                    else if (luminance<100)
                    {
                        SetPixel(resultingImage, resultingPixelOffsetBase+x, pixel1, width);
                    }
                    else if(luminance <150)
                    {
                        SetPixel(resultingImage, resultingPixelOffsetBase+x, pixel2, width);
                    }
                    else if (luminance<200)
                    {
                        SetPixel(resultingImage, resultingPixelOffsetBase+x, pixel3, width);
                    }
                    else if (luminance < 250)
                    {
                        SetPixel(resultingImage, resultingPixelOffsetBase+x, pixel4, width);
                    }
                }
            }

            return resultingImage;
        }

        /// Copies pixel data describing resulting pixel shape to the 
        /// resulting image
        private static void SetPixel(byte[] resultingImage, int resultingPixelOffset, byte[] pixelData, int strideInBytes)
        {
            for (int i = 0; i < pixelData.Length; ++i)
            {
                resultingImage[resultingPixelOffset + strideInBytes*i] = pixelData[i];
            }
        }
    }
}
