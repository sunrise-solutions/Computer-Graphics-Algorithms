using Models;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;

namespace DrawingAlgorithm
{
    public static class Bresenham
    {
        public static IEnumerable<Pixel> GetPixels(int x1, int y1, float z1, int x2, int y2, float z2, int width, int height, Bgr24Bitmap bitmap, ZBuffer zBuf, Color color)
        {
            List<Pixel> list = new List<Pixel>();
            int deltaX = Math.Abs(x2 - x1);
            int deltaY = Math.Abs(y2 - y1);
            float deltaZ = Math.Abs(z2 - z1);
            int signX = x1 < x2 ? 1 : -1;
            int signY = y1 < y2 ? 1 : -1;
            float signZ = z1 < z2 ? 1 : -1;

            float curZ = z1;
            float z3 = deltaZ / deltaY;

            int error = deltaX - deltaY;

            list.Add(new Pixel(x2, y2, z2));

            if (x2 > 0 && x2 < zBuf.Width && y2 > 0 && y2 < zBuf.Height)
            {
                if (z2 <= zBuf[x2, y2])
                {
                    //if (z2 > 0 && z2 < 1)
                    //{
                    //    zBuf[x2, y2] = z2;
                    //    bitmap[x2, y2] = Color.FromRgb(33, 105, 72); //new Vector4(0, 0, 255, 255);
                    //}
                    zBuf[x2, y2] = z2;
                    bitmap[x2, y2] = color; //Color.FromRgb(0, 0, 255); //new Vector4(0, 0, 255, 255);
                }
            }

            while (x1 != x2 || y1 != y2)
            {
                list.Add(new Pixel(x1, y1, curZ));

                if (x1 > 0 && x1 < zBuf.Width && y1 > 0 && y1 < zBuf.Height)
                {
                    if (curZ <= zBuf[(int)x1, (int)y1])
                    {
                        //if (curZ > 0 && curZ < 1)
                        //{
                        //    zBuf[(int)x1, (int)y1] = curZ; //!!!! curZ????? point2.Z
                        //    bitmap[(int)x1, (int)y1] = Color.FromRgb(0, 0, 255);
                        //}
                        zBuf[(int)x1, (int)y1] = curZ; //!!!! curZ????? point2.Z
                        bitmap[(int)x1, (int)y1] = color;//Color.FromRgb(0, 0, 255);
                    }
                }

                int error2 = error * 2;

                if (error2 > -deltaY)
                {
                    error -= deltaY;
                    x1 += signX;
                }
                if (error2 < deltaX)
                {
                    error += deltaX;
                    y1 += signY;
                    curZ += signZ * z3;
                }
            }
            return list;
        }

        private static bool IsFaceVisible(List<Vector4> pointsList, List<Vector3> face)
        {
            bool result = true;
            int indexPoint1 = (int)face[0].X;
            int indexPoint2 = (int)face[1].X;
            int indexPoint3 = (int)face[2].X;
            Vector4 point1 = pointsList[indexPoint1];
            Vector4 point2 = pointsList[indexPoint2];
            Vector4 point3 = pointsList[indexPoint3];

            Vector4 vector1 = point2 - point1;
            Vector4 vector2 = point3 - point2;
            Vector3 vector1XYZ = new Vector3(vector1.X, vector1.Y, vector1.Z);
            Vector3 vector2XYZ = new Vector3(vector2.X, vector2.Y, vector2.Z);
            Vector3 normal = Vector3.Cross(vector1XYZ, vector2XYZ);

            if (normal.Z >= 0)
            {
                result = false;
            }

            return result;
        }
    }
}
