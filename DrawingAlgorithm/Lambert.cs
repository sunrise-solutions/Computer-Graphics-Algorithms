using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DrawingAlgorithm
{
    public static class Lambert
    {
        public static Color GetPointColor(Vector3 normal, Vector3 lightVector, Color initialColor)
        {
            double coef = GetLightCoef(normal, lightVector);
            byte r = (byte)Math.Round(initialColor.R * coef);
            byte g = (byte)Math.Round(initialColor.G * coef);
            byte b = (byte)Math.Round(initialColor.B * coef);

            return Color.FromArgb(initialColor.A, r, g, b);
        }

        private static double GetLightCoef(Vector3 normal, Vector3 lightVector)
        {
            return Math.Max(Vector3.Dot(normal, -lightVector), 0);
        }
    }
}
