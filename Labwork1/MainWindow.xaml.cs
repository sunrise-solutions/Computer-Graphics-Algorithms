using DrawingAlgorithm;
using Models;
using ObjFileParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
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
        int windowWidth = 2000, windowHeight = 950, dx = 500, dy = 500;
        public MainWindow()
        {
            InitializeComponent();
        }

        public static Matrix4x4 GetResultMatrix(Matrix4x4 translateMatrix, Matrix4x4 rotateXMatrix, Matrix4x4 rotateYMatrix,
            Matrix4x4 rotateZMatrix, Matrix4x4 scaleMatrix)
        {
            return scaleMatrix * rotateXMatrix * rotateYMatrix * rotateZMatrix * translateMatrix;
        }

        void LoadWindow(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);
            List<Pixel> pixels = new List<Pixel>();

            foreach (Group group in graphObject.Groups)
            {
                foreach (Face face in group.Faces)
                {
                    int index = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    Vertex vertex0 = group.Vertices.ElementAt(index);
                    Vertex vertex1;
                    for (int i = 1; i < face.FaceElements.Count; i++)
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

        void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);
            List<Pixel> pixels = new List<Pixel>();

            float value = 1;

            while (value < 1000)
            {
                GraphicModel.Source = null;

                foreach (Group group in graphObject.Groups)
                {
                    foreach (Vertex vertex in group.Vertices)
                    {
                        Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                        Vector4 vectorResult = Vector4.Transform(vector, GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(value++), Matrix4x4.CreateRotationY(1), Matrix4x4.CreateRotationZ(1), Matrix4x4.CreateScale(1)));
                        vertex.X = vectorResult.X;
                        vertex.Y = vectorResult.Y;
                        vertex.Z = vectorResult.Z;
                        vertex.W = vectorResult.W;
                    }
                }

                foreach (Group group in graphObject.Groups)
                {
                    foreach (Face face in group.Faces)
                    {
                        int index = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                        Vertex vertex0 = group.Vertices.ElementAt(index);
                        Vertex vertex1;
                        for (int i = 1; i < face.FaceElements.Count; i++)
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
                Thread.Sleep(1000);
            }
        }
    }
}
