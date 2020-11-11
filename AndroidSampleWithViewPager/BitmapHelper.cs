// This code is a part of samples package for Apitron PDF Rasterizer .NET
// © 2014 Apitron LTD 

using System;
using System.Runtime.InteropServices;
using Android.Graphics;

namespace AndroidSampleWithViewPager
{
    /// <summary>
    /// A bitmap helper class for various bitmap operations.
    /// </summary>
    static class BitmapHelper
    {
        /// <summary>
        /// Creates the mutable bitmap with ARGB32 pixel format.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bgColor">Color of the bg.</param>
        /// <returns>Created bitmap.</returns>
        public static Bitmap CreateMutableBitmapARGB32(int width, int height, uint bgColor)
        {
            // pixel data
            uint pixelDataSize = (uint) (width*height*4);

            byte[] imageData = new byte[pixelDataSize+54];

            // BITMAPFILEHEADER
            // "BM" +2
            imageData[0] = 0x42;
            imageData[1] = 0x4D;

            // bitmapsize +4; pixeldata size + bitmap file header(14 bytes) + bitmapinfo header(40 bytes)
            Array.Copy(BitConverter.GetBytes(pixelDataSize+54), 0, imageData, 2, 4);

            // reserved +4
            imageData[6] = 0;
            imageData[7] = 0;
            imageData[8] = 0;
            imageData[9] = 0;

            // pixel offset +4
            imageData[10] = 54;
            imageData[11] = 0;
            imageData[12] = 0;
            imageData[13] = 0;

            
            // BITMAPINFOHEADER
            // header size offset +4
            imageData[14] = 40;
            imageData[15] = 0;
            imageData[16] = 0;
            imageData[17] = 0;

            // bitmapWidth +4
            Array.Copy(BitConverter.GetBytes(width), 0, imageData, 18, 4);

            // bitmapHeight +4
            Array.Copy(BitConverter.GetBytes(height), 0, imageData, 22, 4);

            // planes +2
            imageData[26] = 1;
            imageData[27] = 0;

            // bitcount +2
            imageData[28] = 32;
            imageData[29] = 0;

            // compression +4, BI_RGB
            imageData[30] = 0;
            imageData[31] = 0;
            imageData[32] = 0;
            imageData[33] = 0;

            // pixel data size +4
            Array.Copy(BitConverter.GetBytes(pixelDataSize), 0, imageData, 34, 4);

            // dpiX  +4
            Array.Copy(BitConverter.GetBytes(2835), 0, imageData, 38, 4);

            // dpiY +4
            Array.Copy(BitConverter.GetBytes(2835), 0, imageData, 42, 4);

            // clrUsed +4
            imageData[46] = 0;
            imageData[47] = 0;
            imageData[48] = 0;
            imageData[49] = 0;

            // clr important +4
            imageData[50] = 0;
            imageData[51] = 0;
            imageData[52] = 0;
            imageData[53] = 0;

            FillBitmapWithColor(imageData, 54, width * 4, bgColor);

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InMutable = true;
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            
            Bitmap result = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);
            result.HasAlpha = true;

            return result;
        }

        /// <summary>
        /// Initializes the bitmap 
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="stride">The stride.</param>
        /// <param name="bgColor">Color of the bg.</param>
        private static  void FillBitmapWithColor(byte[] data, int offset, int stride, uint bgColor)
        {
            byte a = (byte) ((bgColor >> 24) & 0xFF);
            byte r = (byte) ((bgColor >> 16) & 0xFF);
            byte g = (byte) ((bgColor >> 8) & 0xFF);
            byte b = (byte)(bgColor & 0xFF);

            unchecked
            {
                for (int i = offset; i < offset+stride; i+=4)
                {
                    data[i] = b;
                    data[i+1] = g;
                    data[i+2] = r;
                    data[i+3] = a;
                }
            }

            for (int i = offset+stride; i < data.Length; i += stride)
            {
                Array.Copy(data, offset, data, i, stride);
            }
        }


        /// <summary>
        /// Copies the pixel data to bitmap, data assumed to be in ARGB format so it will be trasformed to the internal Android ABGR format that bitmaps use while copying.
        /// </summary>
        /// <param name="targetBitmap">The target bitmap.</param>
        /// <param name="data">The data.</param>
        /// <param name="stride">The stride, bitmap width in pixels.</param>
        public static void CopyPixelDataToBitmap(Bitmap targetBitmap, int[] data, int stride)
        {
            if (targetBitmap != null)
            {
                IntPtr ptr = targetBitmap.LockPixels();

                unchecked
                {
                    int value;

                    int[] strideArray = new int[stride];

                    for (int i = 0; i < data.Length; i += stride)
                    {
                        for (int j = 0, k = i; j < stride; ++j, ++k)
                        {
                            value = data[k];

                            strideArray[j] =
                                (int)
                                ((0xFF000000) | ((value & 0xFF) << 16) | (value & 0x0000FF00) | ((value >> 16) & 0xFF));
                        }

                        Marshal.Copy(strideArray, 0, ptr + (i << 2), stride);
                    }
                }
                targetBitmap.UnlockPixels();
            }
        }
    }
}