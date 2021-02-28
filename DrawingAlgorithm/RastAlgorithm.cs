using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DrawingAlgorithm
{
    public static class RastAlgorithm
    {
        public static IEnumerable<Pixel> DrawPixelForRasterization(List<Pixel> sidesList, Bgr24Bitmap bitmap, ZBuffer zBuf, Color color)
        {
            List<Pixel> list = new List<Pixel>();
            int minY, maxY;
            Pixel xStartPixel, xEndPixel;
            FindMinAndMaxY(sidesList, out minY, out maxY);

            for (int y = minY + 1; y < maxY; y++)
            {
                FindStartAndEndXByY(sidesList, y, out xStartPixel, out xEndPixel);
                int signZ = xStartPixel.Z < xEndPixel.Z ? 1 : -1;
                float deltaZ = Math.Abs((xEndPixel.Z - xStartPixel.Z) / (float)((xEndPixel.X) - (xStartPixel.X)));
                float curZ = xStartPixel.Z;

                for (int x = xStartPixel.X + 1; x < xEndPixel.X; x++)
                {
                    curZ += signZ * deltaZ;

                    if (x > 0 && x < zBuf.Width && y > 0 && y < zBuf.Height)
                    {
                        if (curZ <= zBuf[x, y])
                        {
                            zBuf[x, y] = curZ;
                            list.Add(new Pixel(x, y, (int)curZ));
                            bitmap[x, y] = color; //Color.FromRgb(33, 105, 72); //new Vector4(255, 0, 0, 255);
                        }
                    }
                }
            }
            return list;
        }

        private static void FindMinAndMaxY(List<Pixel> sidesList, out int min, out int max)
        {
            try
            {
                var list = sidesList.OrderBy(x => x.Y).ToList();
                min = list[0].Y;
                max = list[sidesList.Count - 1].Y;
            } catch
            {
                min = 0;
                max = 0;
            }
        }

        private static void FindStartAndEndXByY(List<Pixel> sidesList, int y, out Pixel xStartPixel, out Pixel xEndPixel)
        {
            List<Pixel> sameYList = sidesList.Where(x => x.Y == y).OrderBy(x => x.X).ToList(); //sorted by X

            xStartPixel = sameYList[0];
            xEndPixel = sameYList[sameYList.Count - 1];
            for (int i = 1; i < sameYList.Count; i++)
            {
                if (sameYList[i].X - sameYList[i - 1].X > 1)
                {
                    xEndPixel = sameYList[i];
                    break;
                }
                else
                {
                    xStartPixel = sameYList[i];
                }
            }
        }
    }
}
