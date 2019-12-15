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
        string filePathMiniature = @"..\..\..\obj_files\moon.obj";
        float minScale = 50, maxScale = 200;
        int windowWidth = 2000, windowHeight = 1400, maxDx = 1000, maxDy = 700;
        int miniatureWidth = 1000, miniatureHeight = 700, minDx = 500, minDy = 350;
        float defaultFieldOfView = (float)(45 * Math.PI / 180), defaultNearPlaneDistance = 1, defaultFarPlaneDistance = 1000;
        bool isApplyOptions = false, isActive = false;
        bool stopCameraX = false, stopCameraY = false, stopCameraZ = false, stopModelX = false, stopModelY = false, stopModelZ = false,
            stopCameraAroundX = false, stopCameraAroundY = false, stopCameraAroundZ = false, stopModelAroundX = false, stopModelAroundY = false, stopModelAroundZ = false;
        float cameraX = 0, cameraY = 0, cameraZ = 0, modelX = 0, modelY = 0, modelZ = 0, cameraAroundX = 0, cameraAroundY = 0, cameraAroundZ = 0, modelAroundX = 0, modelAroundY = 0, modelAroundZ = 0;
        GraphObject globalModel, globalMiniature;
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

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
            miniatureWidth = (int)MiniatureModel.Width;
            miniatureHeight = (int)MiniatureModel.Height;
            minDx = miniatureWidth / 2;
            minDy = miniatureHeight / 2;

            windowWidth = (int)GraphicModel.Width;
            windowHeight = (int)GraphicModel.Height;
            maxDx = windowWidth / 2;
            maxDy = windowHeight / 2;

            Parser parser = new Parser();
            globalModel = parser.ParseFile(filePath);
            DrawModel(globalModel, GraphicModel, maxDx, maxDy, maxScale);

            Parser parser2 = new Parser();
            globalMiniature = parser2.ParseFile(filePathMiniature);
            DrawModel(globalMiniature, MiniatureModel, minDx, minDy, minScale);
        }

        async void DrawModel(GraphObject graphObject, Image targetPlace, int dx, int dy, double scale)
        {
            List<Pixel> pixels = new List<Pixel>();
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
                pixels = await GetListAsync(graphObject_copy.Groups[0], dx, dy, -1);
                targetPlace.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
            }
        }

        async Task<bool> DrawMiniatureByMatrix(Matrix4x4 result, Matrix4x4 viewport)
        {
            List<Pixel> pixels = new List<Pixel>();
            //double scale = 400;
            GraphObject graphObject_copy = (GraphObject)globalMiniature.Clone();
            foreach (Group group in globalMiniature.Groups)
            {
                //result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY(0), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale((float)scale, (float)scale, (float)scale));
                graphObject_copy = (GraphObject)globalMiniature.Clone();
                for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                {
                    Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                    Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                    Vector4 vectorResult = Vector4.Transform(vector, result);
                    //vectorResult /= vertex.W;
                    //vectorResult = Vector4.Transform(vectorResult, viewport);
                    vertex.X = vectorResult.X;
                    vertex.Y = vectorResult.Y;
                    vertex.Z = vectorResult.Z;
                    vertex.W = vectorResult.W;
                }
                pixels = await GetListAsync(graphObject_copy.Groups[0], minDx, minDy, -1);
                MiniatureModel.Source = PixelDrawing.GetBitmap(miniatureWidth, miniatureHeight, pixels);
            }
            return true;
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
                        result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY((float)(value)), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale((float)maxScale, (float)maxScale, (float)maxScale));
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
                        pixels = await GetListAsync(graphObject_copy.Groups[0], maxDx, maxDy, -1);
                        GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
                        value += 1 * Math.PI / 180;
                    }
                }
            } else
            {
                TransformOptions options = GetUserOptions(maxScale);
                GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

                Matrix4x4 result, viewport;
                foreach (Group group in graphObject.Groups)
                {
                    result = GetOptionsMatrix(options, windowWidth, windowHeight);
                    viewport = GetViewPort(0, 0, windowWidth, windowHeight);
                    graphObject_copy = (GraphObject)graphObject.Clone();
                    for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                    {
                        Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                        Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                        Vector4 vectorResult = Vector4.Transform(vector, result);
                        vectorResult /= vertex.W;
                        vectorResult = Vector4.Transform(vectorResult, viewport);
                        vertex.X = vectorResult.X;
                        vertex.Y = vectorResult.Y;
                        vertex.Z = vectorResult.Z;
                        vertex.W = vectorResult.W;
                    }
                    pixels = await GetListAsync(graphObject_copy.Groups[0], 0, 0, 1);
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

        private async Task<List<Pixel>> GetListAsync(Group group, int dx, int dy, int viceVersa)
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
                        pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * viceVersa + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * viceVersa + dy), windowWidth, windowHeight));
                    }
                    index0 = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                    index1 = (face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex != -2) ? face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex : (group.Vertices.Count - 1);
                    vertex0 = group.Vertices.ElementAt(index0);
                    vertex1 = group.Vertices.ElementAt(index1);
                    pixels.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * viceVersa + dy), (int)(vertex1.X + dx), (int)(vertex1.Y * viceVersa + dy), windowWidth, windowHeight));
                }

                return pixels;
            });
        }

        public Matrix4x4 GetOptionsMatrix(TransformOptions options, int width, int height)
        {
            Matrix4x4 model = Matrix4x4.CreateScale(options.Scale) * Matrix4x4.CreateFromYawPitchRoll(options.ModelAroundX, options.ModelAroundY, options.ModelAroundZ)
                * Matrix4x4.CreateTranslation(options.ModelX, options.ModelY, options.ModelZ);
            Matrix4x4 view = Matrix4x4.CreateTranslation(-new Vector3(options.CameraX, options.CameraY, options.CameraZ))
                * Matrix4x4.Transpose(Matrix4x4.CreateFromYawPitchRoll(options.CameraAroundX, options.CameraAroundY, options.CameraAroundZ));
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(defaultFieldOfView, (float)width/height, defaultNearPlaneDistance, defaultFarPlaneDistance);

            Matrix4x4 mvp = model * view * projection;
            return mvp;
        }

        public TransformOptions GetUserOptions(double scale)
        {
            try
            {
                TransformOptions options = new TransformOptions()
                {
                    Scale = (float)scale,
                    CameraX = cameraX,
                    CameraY = cameraY,
                    CameraZ = cameraZ,
                    CameraAroundX = cameraAroundX,
                    CameraAroundY = cameraAroundY,
                    CameraAroundZ = cameraAroundZ,
                    ModelX = modelX,
                    ModelY = modelY,
                    ModelZ = modelZ,
                    ModelAroundX = modelAroundX,
                    ModelAroundZ = modelAroundZ
                    //CameraX = (stopCameraX) ? cameraX : float.Parse(CameraX.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //CameraY = (stopCameraY) ? cameraY : float.Parse(CameraY.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //CameraZ = (stopCameraZ) ? cameraZ : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //CameraAroundX = (stopCameraAroundX) ? cameraAroundX : (float)(float.Parse(CameraAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    //CameraAroundY = (stopCameraAroundY) ? cameraAroundY : (float)(float.Parse(CameraAroundY.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    //CameraAroundZ = (stopCameraAroundZ) ? cameraAroundZ : (float)(float.Parse(CameraAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    //ModelX = (stopModelX) ? modelX : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //ModelY = (stopModelY) ? modelY : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //ModelZ = (stopModelZ) ? modelZ : float.Parse(CameraZ.Text, CultureInfo.InvariantCulture.NumberFormat),
                    //ModelAroundX = (stopModelAroundX) ? modelAroundX : (float)(float.Parse(ModelAroundX.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    //ModelAroundY = (stopModelAroundY) ? modelAroundY : (float)(float.Parse(ModelAroundY.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180),
                    //ModelAroundZ = (stopModelAroundZ) ? modelAroundZ : (float)(float.Parse(ModelAroundZ.Text, CultureInfo.InvariantCulture.NumberFormat) * Math.PI / 180)
                };
                return options;
            } catch
            {
                MessageBox.Show(" Даннные некорректны.");
                return null;
            }
        }

        public Matrix4x4 GetViewPort(int minX, int minY, int width, int height)
        {
            return new Matrix4x4(width / 2, 0, 0, 0, 0, -1 * height / 2, 0, 0, 0, 0, 1, 0, minX + width / 2, minY + height / 2, 0, 1);
        }

        async public void ChangeCameraX(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
            isActive = true;
            int i = 0;  
            while (isActive)
            {
                cameraX = i;
                i++;
                bool check = await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(minScale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }

            //float scaling = (float)scaleSlider.Value;
            //float modelYaw = (float)(modelYawSlider.Value * Math.PI / 180);
            //float modelPitch = (float)(modelPitchSlider.Value * Math.PI / 180);
            //float modelRoll = (float)(modelRollSlider.Value * Math.PI / 180);
            //float translationX = (float)translationXSlider.Value;
            //float translationY = (float)translationYSlider.Value;
            //float translationZ = (float)translationZSlider.Value;
            //float cameraPositionX = (float)CameraPositionXSlider.Value;
            //float cameraPositionY = (float)CameraPositionYSlider.Value;
            //float cameraPositionZ = (float)CameraPositionZSlider.Value;
            //float cameraYaw = (float)(CameraYawSlider.Value * Math.PI / 180);
            //float cameraPitch = (float)(CameraPitchSlider.Value * Math.PI / 180);
            //float cameraRoll = (float)(CameraRollSlider.Value * Math.PI / 180);
            //float fieldOfView = (float)(FieldOfViewSlider.Value * Math.PI / 180);
            //float aspectRatio = (float)width / height;
            //float nearPlaneDistance = (float)NearPlaneDistanceSlider.Value;
            //float farPlaneDistance = (float)FarPlaneDistanceSlider.Value;
            //int xMin = 0;
            //int yMin = 0;
        }

        public void ChangeCameraY(object sender, RoutedEventArgs e)
        {
            int i = 0;
            while(isApplyOptions)
            {
                cameraY = i;
                i++;
            }
        }

        public void ChangeCameraZ(object sender, RoutedEventArgs e)
        {
            for (int i = -100; i < 100; i++)
            {
                cameraZ = i;
            }
            isApplyOptions = true;
        }

        public void ChangeModelX(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeModelY(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeModelZ(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeCameraAroundX(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeCameraAroundY(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeCameraAroundZ(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeModelAroundX(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeModelAroundY(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void ChangeModelAroundZ(object sender, RoutedEventArgs e)
        {
            isApplyOptions = true;
        }

        public void StopChangeCameraX(object sender, RoutedEventArgs e)
        {
            isActive = false;
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
