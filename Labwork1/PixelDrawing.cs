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

            Application.Current.Dispatcher.Invoke(() =>
            {
                writeableBitmap.Lock();
                pBackBuffer = (long)writeableBitmap.BackBuffer;
                backBufferStride = writeableBitmap.BackBufferStride;
            });

            unsafe
            {
                foreach(Pixel pixel in pixels)
                {
                    long bufferWithOffset = pBackBuffer + pixel.Y * backBufferStride + pixel.X * writeableBitmap.Format.BitsPerPixel / 8;
                    *((int*)bufferWithOffset) = pixel.Color.B;
                    *((int*)bufferWithOffset + 1) = pixel.Color.G;
                    *((int*)bufferWithOffset + 2) = pixel.Color.R;
                    *((int*)bufferWithOffset + 3) = pixel.Color.A;
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
                writeableBitmap.Unlock();
            });

            return writeableBitmap;
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern void RtlZeroMemory(IntPtr dst, int length);

        public static void ClearWriteableBitmap(WriteableBitmap bmp)
        {
            RtlZeroMemory(bmp.BackBuffer, bmp.PixelWidth * bmp.PixelHeight * (bmp.Format.BitsPerPixel / 8));

            bmp.Dispatcher.Invoke(() =>
            {
                bmp.Lock();
                bmp.AddDirtyRect(new Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
                bmp.Unlock();
            });
        }
    }
}
