using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Labwork1
{
    public static class PixelDrawing
    {
        const int dpiX = 96, dpiY = 96;

        public static WriteableBitmap GetBitmap(int width, int height, List<Pixel> pixels)
        {
            WriteableBitmap writeableBitmap = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormats.Bgr32, null);
            long pBackBuffer = 0;
            long backBufferStride = 0;

            try
            {
                writeableBitmap.Lock();
                pBackBuffer = (long)writeableBitmap.BackBuffer;
                backBufferStride = writeableBitmap.BackBufferStride;

                //Random rand = new Random();
                //for (int x = 0; x < writeableBitmap.PixelWidth; x++)
                //{
                //    for (int y = 0; y < writeableBitmap.PixelHeight; y++)
                //    {
                //        int alpha = 255;
                //        int red = 255;
                //        int green = 255;
                //        int blue = 255;

                //        // Set the pixel value.                    
                //        byte[] colorData = { (byte)blue, (byte)green, (byte)red, (byte)alpha }; // B G R

                //        Int32Rect rect = new Int32Rect(x, y, 1, 1);
                //        int stride = (writeableBitmap.PixelWidth * writeableBitmap.Format.BitsPerPixel) / 8;
                //        writeableBitmap.WritePixels(rect, colorData, stride, 0);
                //    }
                //}

                unsafe
                {
                    foreach (Pixel pixel in pixels)
                    {
                        if (pixel == null) { continue; }
                        if (pixel.X > 0 && pixel.X < width && pixel.Y > 0 && pixel.Y < height)
                        {
                            long bufferWithOffset = pBackBuffer + pixel.Y * backBufferStride + pixel.X * writeableBitmap.Format.BitsPerPixel / 8;
                            *((int*)bufferWithOffset) = pixel.Color.B;
                            *((int*)bufferWithOffset) |= pixel.Color.G << 8;
                            *((int*)bufferWithOffset) |= pixel.Color.R << 16;
                            *((int*)bufferWithOffset) |= pixel.Color.A << 24;
                        }
                    }
                }

            }
            finally
            {
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                writeableBitmap.Unlock();
            }

            return writeableBitmap;
        }
    }
}
