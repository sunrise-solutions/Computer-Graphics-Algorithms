using Models;
using System;
using System.Collections.Generic;

namespace DrawingAlgorithm
{
    public static class Bresenham
    {
        public static IEnumerable<Pixel> GetPixels(int x1, int y1, int x2, int y2, int width, int height)
        {
            List<Pixel> list = new List<Pixel>();
            if (((x1 > 0 && x1 < width) || (x2 > 0 && x2 < width)) && ((y1 > 0 && y1 < height) || (y2 > 0 && y2 < height)))
            {
                x1 = x1 < 0 ? 0 : x1;
                x2 = x2 < 0 ? 0 : x2;
                y1 = y1 < 0 ? 0 : y1;
                y2 = y2 < 0 ? 0 : y2;
                x1 = x1 > width ? width : x1;
                x2 = x2 > width ? width : x2;
                y1 = y1 > height ? height : y1;
                y2 = y2 > height ? height : y2;

                int deltaX = Math.Abs(x2 - x1);
                int deltaY = Math.Abs(y2 - y1);
                int signX = x1 < x2 ? 1 : -1;
                int signY = y1 < y2 ? 1 : -1;

                int error = deltaX - deltaY;

                list.Add(new Pixel(x2, y2));

                while (x1 != x2 || y1 != y2)
                {
                    list.Add(new Pixel( x1, y1));

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
                    }
                }
            }
            return list;
        }

        //public static List<PixelInfo> GetAllPixels(Group drawingObject, int width, int height)
        //{
        //    var list = new List<PixelInfo>();
        //    int point1, point2;

        //    foreach (var face in drawingObject.Faces)
        //    {
        //        for (int i = 0; i < face.FaceElements.Count - 1; i++)
        //        {
        //            point1 = (int)face.FaceElements[i].VertexIndex - 1;
        //            point2 = (int)face.FaceElements[i+1].VertexIndex - 1;

        //            AddPixelsForLine(list, (int)drawingObject.Vertices[point1].X, (int)drawingObject.pointsList[point1].Y,
        //            (int)drawingObject.pointsList[point2].X, (int)drawingObject.pointsList[point2].Y, width, height);
        //        }

        //        point1 = (int)face[0] - 1;
        //        point2 = (int)face[face.Count - 1] - 1;

        //        AddPixelsForLine(list, (int)drawingObject.pointsList[point1].X, (int)drawingObject.pointsList[point1].Y,
        //            (int)drawingObject.pointsList[point2].X, (int)drawingObject.pointsList[point2].Y, width, height);
        //    }

        //    return list;
        //}

        //private static void AddPixelsForLine(List<PixelInfo> list, int x1, int y1, int x2, int y2, int width, int height)
        //{
        //    if (((x1 > 0 && x1 < width) || (x2 > 0 && x2 < width)) && ((y1 > 0 && y1 < height) || (y2 > 0 && y2 < height)))
        //    {
        //        x1 = x1 < 0 ? 0 : x1;
        //        x2 = x2 < 0 ? 0 : x2;
        //        y1 = y1 < 0 ? 0 : y1;
        //        y2 = y2 < 0 ? 0 : y2;
        //        x1 = x1 > width ? width : x1;
        //        x2 = x2 > width ? width : x2;
        //        y1 = y1 > height ? height : y1;
        //        y2 = y2 > height ? height : y2;

        //        int deltaX = Math.Abs(x2 - x1);
        //        int deltaY = Math.Abs(y2 - y1);
        //        int signX = x1 < x2 ? 1 : -1;
        //        int signY = y1 < y2 ? 1 : -1;

        //        int error = deltaX - deltaY;

        //        list.Add(new PixelInfo() { X = x2, Y = y2, Color = Color.FromArgb(255, 255, 0, 0) });

        //        while (x1 != x2 || y1 != y2)
        //        {
        //            list.Add(new PixelInfo() { X = x1, Y = y1, Color = Color.FromArgb(255, 255, 0, 0) });

        //            int error2 = error * 2;

        //            if (error2 > -deltaY)
        //            {
        //                error -= deltaY;
        //                x1 += signX;
        //            }
        //            if (error2 < deltaX)
        //            {
        //                error += deltaX;
        //                y1 += signY;
        //            }
        //        }
        //    }
        //}
    }
}
