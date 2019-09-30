using DrawingAlgorithm;
using Models;
using ObjFileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Labwork1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = @"..\..\..\obj_files\cat.obj";
        int windowWidth = 1500, windowHeight = 700, dx = 500, dy = 500;
        public MainWindow()
        {
            InitializeComponent();
        }

        void LoadWindow(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);
            List<Pixel> pixels = new List<Pixel>();

            foreach(Group group in graphObject.Groups)
            {
                foreach(Face face in group.Faces)
                {
                    int index = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    Vertex vertex0 = group.Vertices.ElementAt(index);
                    Vertex vertex1;
                    for(int i = 1; i < face.FaceElements.Count; i ++)
                    {
                        index = (face.FaceElements.ElementAt(i).VertexIndex != -2) ? face.FaceElements.ElementAt(i).VertexIndex : (group.Vertices.Count - 1);
                        vertex1 = group.Vertices.ElementAt(index);
                        pixels.AddRange(Bresenham.GetPixels((int)vertex0.X + dx, (int)vertex0.Y * (-1) + dy, (int)vertex1.X + dx, (int)vertex1.Y * (-1) + dy));
                        vertex0 = vertex1;
                    }
                    vertex1 = group.Vertices.ElementAt(face.FaceElements.ElementAt(0).VertexIndex);
                    pixels.AddRange(Bresenham.GetPixels((int)vertex0.X + dx, (int)vertex0.Y * (-1) + dy, (int)vertex1.X + dx, (int)vertex1.Y * (-1) + dy));
                }
                GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
            }
        }
    }
}
