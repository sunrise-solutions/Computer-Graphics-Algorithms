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
        int windowWidth = 2000, windowHeight = 1000, dx = 1000, dy = 500;
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
                for (int j = 0; j < group.Faces.Count; j++)
                {
                    Face face = group.Faces[j];
                    //int index = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    Vertex vertex0, vertex1;
                    int index0, index1;
                    for (int i = 0; i < face.FaceElements.Count - 1; i++)
                    {
                        index0 = (face.FaceElements.ElementAt(i).VertexIndex != -2) ? face.FaceElements.ElementAt(i).VertexIndex : (group.Vertices.Count - 1);
                        index1 = (face.FaceElements.ElementAt(i+1).VertexIndex != -2) ? face.FaceElements.ElementAt(i+1).VertexIndex : (group.Vertices.Count - 1);
                        vertex0 = group.Vertices.ElementAt(index0);
                        vertex1 = group.Vertices.ElementAt(index1);
                        pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * (-1) + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * (-1) + dy), windowWidth, windowHeight));
                    }
                    index0 = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    index1 = (face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex != -2) ? face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex : (group.Vertices.Count - 1);
                    vertex0 = group.Vertices.ElementAt(index0);
                    vertex1 = group.Vertices.ElementAt(index1);
                    pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * (-1) + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * (-1) + dy), windowWidth, windowHeight));
                    
                }
                GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
            }
        }

        async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);
            List<Pixel> pixels = new List<Pixel>();

            double value = 5 * Math.PI / 180;
            GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

            Matrix4x4 result;
            foreach (Group group in graphObject.Groups)
            {
                while (value < 10000)
                {
                    result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY((float)(value)), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale(1));
                    graphObject_copy = (GraphObject)graphObject.Clone();
                    for ( int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                    {
                        Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                        Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                        Vector4 vectorResult = Vector4.Transform(vector, result);
                        vertex.X = vectorResult.X;
                        vertex.Y = vectorResult.Y;
                        vertex.Z = vectorResult.Z;
                        vertex.W = vectorResult.W;
                    }
                    //Round(graphObject_copy.Groups[0]);

                    pixels = await GetListAsync(graphObject_copy.Groups[0]);
                    GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
                    value += 1 * Math.PI / 180;
                    //Thread.Sleep(100);
                }
            }
        }

        public static void Round(Group group)
        {
            float x, y, z;

            for (int i = 0; i < group.Vertices.Count; i++)
            {
                x = (float)Math.Round(group.Vertices[i].X, 0);
                y = (float)Math.Round(group.Vertices[i].Y, 0);
                z = (float)Math.Round(group.Vertices[i].Z, 0);

                group.Vertices[i] = new Vertex(x, y, z, group.Vertices[i].W);
            }
        }

        private async Task MultiplyAsync(Group group, Matrix4x4 result)
        {
            await Task.Run(() =>
            {
                
            });  
        }

        private async Task<List<Pixel>> GetListAsync(Group group)
        {
            return await Task.Run(() =>
            {
                List<Pixel> pixels = new List<Pixel>();

                foreach (Face face in group.Faces)
                {
                    Vertex vertex0, vertex1;
                    int index0, index1;
                    for (int i = 0; i < face.FaceElements.Count - 1; i++)
                    {
                        index0 = (face.FaceElements.ElementAt(i).VertexIndex != -2) ? face.FaceElements.ElementAt(i).VertexIndex : (group.Vertices.Count - 1);
                        index1 = (face.FaceElements.ElementAt(i + 1).VertexIndex != -2) ? face.FaceElements.ElementAt(i + 1).VertexIndex : (group.Vertices.Count - 1);
                        vertex0 = group.Vertices.ElementAt(index0);
                        vertex1 = group.Vertices.ElementAt(index1);
                        pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * (-1) + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * (-1) + dy), windowWidth, windowHeight));
                    }
                    index0 = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    index1 = (face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex != -2) ? face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex : (group.Vertices.Count - 1);
                    vertex0 = group.Vertices.ElementAt(index0);
                    vertex1 = group.Vertices.ElementAt(index1);
                    pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * (-1) + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * (-1) + dy), windowWidth, windowHeight));
                }

                return pixels;
            });
            
        }
    }
}
