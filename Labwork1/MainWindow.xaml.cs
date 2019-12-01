using DrawingAlgorithm;
using Models;
using ObjFileParser;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        string filePath = @"..\..\..\obj_files\african_head.obj";
        float scale = 400;
        int windowWidth = 2000, windowHeight = 1400, maxDx = 1000, maxDy = 700;
        int miniatureWidth = 700, miniatureHeight = 600, minDx = 350, minDy = 300;
        float defaultFieldOfView = (float)(45 * Math.PI / 180), defaultNearPlaneDistance = 1, defaultFarPlaneDistance = 1000;
        bool isApplyOptions = false;
        bool stopCameraX = false, stopCameraY = false, stopCameraZ = false, stopModelX = false, stopModelY = false, stopModelZ = false,
            stopCameraAroundX = false, stopCameraAroundY = false, stopCameraAroundZ = false, stopModelAroundX = false, stopModelAroundY = false, stopModelAroundZ = false;
        float cameraX, cameraY, cameraZ, modelX, modelY, modelZ, cameraAroundX, cameraAroundY, cameraAroundZ, modelAroundX, modelAroundY, modelAroundZ;

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
            DrawModel(graphObject, GraphicModel, maxDx, maxDy);
        }

        async void DrawModel(GraphObject graphObject, Image targetPlace, int dx, int dy)
        {
            List<Pixel> pixels = new List<Pixel>();
            double scale = 400;
            GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

            Matrix4x4 result;
            foreach (Group group in graphObject.Groups)
            {
                result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY(0), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale((float)scale, (float)scale, (float)scale));
                graphObject_copy = (GraphObject)graphObject.Clone();
                for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                {
                    Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                    Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                    Vector4 vectorResult = Vector4.Transform(vector, result);
                    vertex.X = vectorResult.X;
                    vertex.Y = vectorResult.Y;
                    vertex.Z = vectorResult.Z;
                    vertex.W = vectorResult.W;
                }
                pixels = await GetListAsync(graphObject_copy.Groups[0], dx, dy);
                targetPlace.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
            }
        }

        public async void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);
            List<Pixel> pixels = new List<Pixel>();

            if (!isApplyOptions)
            {
                double value = 5 * Math.PI / 180;
                GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

                Matrix4x4 result;
                foreach (Group group in graphObject.Groups)
                {
                    while (value < 10000)
                    {
                        result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY((float)(value)), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale((float)scale, (float)scale, (float)scale));
                        graphObject_copy = (GraphObject)graphObject.Clone();
                        for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                        {
                            Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                            Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                            Vector4 vectorResult = Vector4.Transform(vector, result);
                            vertex.X = vectorResult.X;
                            vertex.Y = vectorResult.Y;
                            vertex.Z = vectorResult.Z;
                            vertex.W = vectorResult.W;
                        }
                        pixels = await GetListAsync(graphObject_copy.Groups[0], maxDx, maxDy);
                        GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
                        value += 1 * Math.PI / 180;
                    }
                }
            } else
            {
                TransformOptions options = GetUserOptions();
                GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

                Matrix4x4 result;
                foreach (Group group in graphObject.Groups)
                {
                    result = GetOptionsMatrix(options);
                    graphObject_copy = (GraphObject)graphObject.Clone();
                    for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                    {
                        Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                        Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                        Vector4 vectorResult = Vector4.Transform(vector, result);
                        vertex.X = vectorResult.X;
                        vertex.Y = vectorResult.Y;
                        vertex.Z = vectorResult.Z;
                        vertex.W = vectorResult.W;
                    }
                    pixels = await GetListAsync(graphObject_copy.Groups[0], maxDx, maxDy);
                    GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
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

        private async Task<List<Pixel>> GetListAsync(Group group, int dx, int dy)
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

        public Matrix4x4 GetOptionsMatrix(TransformOptions options)
        {
            Matrix4x4 model = Matrix4x4.CreateScale(options.Scale) * Matrix4x4.CreateFromYawPitchRoll(options.ModelAroundX, options.ModelAroundY, options.ModelAroundZ)
                * Matrix4x4.CreateTranslation(options.ModelX, options.ModelY, options.ModelZ);
            Matrix4x4 view = Matrix4x4.CreateTranslation(-new Vector3(options.CameraX, options.CameraY, options.CameraZ))
                * Matrix4x4.Transpose(Matrix4x4.CreateFromYawPitchRoll(options.CameraAroundX, options.CameraAroundY, options.CameraAroundZ));
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(defaultFieldOfView, (float)windowWidth/windowHeight, defaultNearPlaneDistance, defaultFarPlaneDistance);

            Matrix4x4 mvp = model * view * projection;
            return mvp;
        }

        public TransformOptions GetUserOptions()
        {
            try
            {
                TransformOptions options = new TransformOptions()
                {
                    Scale = scale,
                    CameraX = (stopCameraX) ? cameraX : float.Parse(CameraX.Text, CultureInfo.InvariantCulture.NumberFormat),
                    CameraY = (stopCameraY) ? cameraY : float.Parse(CameraY.Text, CultureInfo.InvariantCulture.NumberFormat),
                    CameraZ = (stopCameraZ) ? cameraZ : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    CameraAroundX = (stopCameraAroundX) ? cameraAroundX : (float)(float.Parse(CameraAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    CameraAroundY = (stopCameraAroundY) ? cameraAroundY : (float)(float.Parse(CameraAroundY.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    CameraAroundZ = (stopCameraAroundZ) ? cameraAroundZ : (float)(float.Parse(CameraAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    ModelX = (stopModelX) ? modelX : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    ModelY = (stopModelY) ? modelY : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    ModelZ = (stopModelZ) ? modelZ : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    ModelAroundX = (stopModelAroundX) ? modelAroundX : (float)(float.Parse(ModelAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    ModelAroundY = (stopModelAroundY) ? modelAroundY : (float)(float.Parse(ModelAroundY.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    ModelAroundZ = (stopModelAroundZ) ? modelAroundZ : (float)(float.Parse(ModelAroundZ.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180)
                };
                return options;
            } catch
            {
                MessageBox.Show(" Даннные некорректны.");
                return null;
            }
        }

        public void ChangeCameraX(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeCameraY(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeCameraZ(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelX(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelY(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelZ(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeCameraAroundX(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeCameraAroundY(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeCameraAroundZ(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelAroundX(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelAroundY(object sender, TextChangedEventArgs e)
        {
        }

        public void ChangeModelAroundZ(object sender, TextChangedEventArgs e)
        {
        }

        public void StopChangeCameraX(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeCameraY(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeCameraZ(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelX(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelY(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelZ(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeCameraAroundX(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeCameraAroundY(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeCameraAroundZ(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelAroundX(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelAroundY(object sender, RoutedEventArgs e)
        {

        }

        public void StopChangeModelAroundZ(object sender, RoutedEventArgs e)
        {

        }
    }
}
