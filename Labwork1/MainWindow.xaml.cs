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
        string filePathMiniature = @"..\..\..\obj_files\african_head.obj";
        float scale = 100, aspectRation = 1;
        int windowWidth = 2000, windowHeight = 1400, maxDx = 1000, maxDy = 700;
        int miniatureWidth = 1000, miniatureHeight = 700, minDx = 500, minDy = 350;
        float fieldOfView = (float)(45 * Math.PI / 180), defaultNearPlaneDistance = 1, defaultFarPlaneDistance = 1000;
        bool isApplyOptions = false, isActive = false;
        int minRange = -200, maxRange = 200;
        int minAngle = -180, maxAngle = 180;
        float cameraX = 0, cameraY = 0, cameraZ = 0, modelX = 0, modelY = 0, modelZ = 0, cameraAroundX = 0, cameraAroundY = 0, cameraAroundZ = 0, modelAroundX = 0, modelAroundY = 0, modelAroundZ = 0;
        GraphObject globalModel, globalMiniature;
        int delay = 0;

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
            miniatureWidth = (int)MiniatureModel.Width + 700;
            miniatureHeight = (int)MiniatureModel.Height + 500;
            minDx = miniatureWidth / 2;
            minDy = miniatureHeight / 2;

            windowWidth = (int)GraphicModel.Width + 500;
            windowHeight = (int)GraphicModel.Height + 500;
            maxDx = windowWidth / 2;
            maxDy = windowHeight / 2;

            Parser parser = new Parser();
            globalModel = parser.ParseFile(filePath);
            DrawModel(globalModel, GraphicModel, maxDx, maxDy, scale * 2);

            Parser parser2 = new Parser();
            globalMiniature = parser2.ParseFile(filePathMiniature);
            DrawModel(globalMiniature, MiniatureModel, minDx, minDy, scale);
        }

        async void DrawModel(GraphObject graphObject, Image targetPlace, int dx, int dy, double scale)
        {
            List<Pixel> pixels = new List<Pixel>();
            GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

            Matrix4x4 result;
            foreach (Group group in graphObject.Groups)
            {
                WriteableBitmap source = new WriteableBitmap(windowWidth, windowHeight, 96, 96, PixelFormats.Bgra32, null);
                Bgr24Bitmap bitmap = new Bgr24Bitmap(source);
                bitmap.Source.Lock();
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
                for (int i = 0; i < graphObject_copy.Groups[0].VertexNormals.Count; i++)
                {
                    VertexNormal vertexNormal = graphObject_copy.Groups[0].VertexNormals[i];
                    Vector3 vector = new Vector3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);
                    Vector3 vectorResult = Vector3.Normalize(Vector3.TransformNormal(vector, result));
                    vertexNormal.X = vectorResult.X;
                    vertexNormal.Y = vectorResult.Y;
                    vertexNormal.Z = vectorResult.Z;
                }
                pixels = await GetListAsync(graphObject_copy.Groups[0], dx, dy, -1, windowWidth, windowHeight, bitmap);
                targetPlace.Source = bitmap.Source;
                bitmap.Source.Unlock();
                //targetPlace.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
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
                WriteableBitmap source = new WriteableBitmap(miniatureWidth, miniatureHeight, 96, 96, PixelFormats.Bgra32, null);
                Bgr24Bitmap bitmap = new Bgr24Bitmap(source);
                bitmap.Source.Lock();
                graphObject_copy = (GraphObject)globalMiniature.Clone();
                for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                {
                    Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                    Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                    Vector4 vectorResult = Vector4.Transform(vector, result);
                    vectorResult /= vertex.W;
                    //vectorResult = Vector4.Transform(vectorResult, viewport);
                    vertex.X = vectorResult.X;
                    vertex.Y = vectorResult.Y;
                    vertex.Z = vectorResult.Z;
                    vertex.W = vectorResult.W;
                }
                for (int i = 0; i < graphObject_copy.Groups[0].VertexNormals.Count; i++)
                {
                    VertexNormal vertexNormal = graphObject_copy.Groups[0].VertexNormals[i];
                    Vector3 vector = new Vector3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);
                    Vector3 vectorResult = Vector3.Normalize(Vector3.TransformNormal(vector, result));
                    vertexNormal.X = vectorResult.X;
                    vertexNormal.Y = vectorResult.Y;
                    vertexNormal.Z = vectorResult.Z;
                }
                //VertexNormal vertexNormal = graphObject_copy.Groups[0].VertexNormals[i];
                pixels = await GetListAsync(graphObject_copy.Groups[0], minDx, minDy, -1, miniatureWidth, miniatureHeight, bitmap);
                MiniatureModel.Source = bitmap.Source;
                bitmap.Source.Unlock();
                //MiniatureModel.Source = PixelDrawing.GetBitmap(miniatureWidth, miniatureHeight, pixels);
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
                        WriteableBitmap source = new WriteableBitmap(windowWidth, windowHeight, 96, 96, PixelFormats.Bgra32, null);
                        Bgr24Bitmap bitmap = new Bgr24Bitmap(source);
                        bitmap.Source.Lock();
                        result = GetResultMatrix(Matrix4x4.CreateTranslation(0, 0, 0), Matrix4x4.CreateRotationX(0), Matrix4x4.CreateRotationY((float)(value)), Matrix4x4.CreateRotationZ(0), Matrix4x4.CreateScale((float)scale * 2, (float)scale * 2, (float)scale * 2));
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
                        for (int i = 0; i < graphObject_copy.Groups[0].VertexNormals.Count; i++)
                        {
                            VertexNormal vertexNormal = graphObject_copy.Groups[0].VertexNormals[i];
                            Vector3 vector = new Vector3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);
                            Vector3 vectorResult = Vector3.Normalize(Vector3.TransformNormal(vector, result));
                            vertexNormal.X = vectorResult.X;
                            vertexNormal.Y = vectorResult.Y;
                            vertexNormal.Z = vectorResult.Z;
                        }
                        pixels = await GetListAsync(graphObject_copy.Groups[0], maxDx, maxDy, -1, windowWidth, windowHeight, bitmap);
                        //GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels);
                        value += 1 * Math.PI / 180;
                        GraphicModel.Source = bitmap.Source;
                        bitmap.Source.Unlock();
                    }
                }
            } else
            {
                TransformOptions options = GetUserOptions(scale * 2);
                GraphObject graphObject_copy = (GraphObject)graphObject.Clone();

                Matrix4x4 result, viewport;
                foreach (Group group in graphObject.Groups)
                {
                    WriteableBitmap source = new WriteableBitmap(windowWidth, windowHeight, 96, 96, PixelFormats.Bgra32, null);
                    Bgr24Bitmap bitmap = new Bgr24Bitmap(source);
                    bitmap.Source.Lock();
                    result = GetOptionsMatrix(options, windowWidth, windowHeight);
                    viewport = GetViewPort(0, 0, windowWidth, windowHeight);
                    graphObject_copy = (GraphObject)graphObject.Clone();
                    for (int i = 0; i < graphObject_copy.Groups[0].Vertices.Count; i++)
                    {
                        Vertex vertex = graphObject_copy.Groups[0].Vertices[i];
                        Vector4 vector = new Vector4(vertex.X, vertex.Y, vertex.Z, vertex.W);
                        Vector4 vectorResult = Vector4.Transform(vector, result);
                        vectorResult /= vertex.W;
                        //vectorResult = Vector4.Transform(vectorResult, viewport);
                        vertex.X = vectorResult.X;
                        vertex.Y = vectorResult.Y;
                        vertex.Z = vectorResult.Z;
                        vertex.W = vectorResult.W;
                    }
                    for (int i = 0; i < graphObject_copy.Groups[0].VertexNormals.Count; i++)
                    {
                        VertexNormal vertexNormal = graphObject_copy.Groups[0].VertexNormals[i];
                        Vector3 vector = new Vector3(vertexNormal.X, vertexNormal.Y, vertexNormal.Z);
                        Vector3 vectorResult = Vector3.Normalize(Vector3.TransformNormal(vector, result));
                        vertexNormal.X = vectorResult.X;
                        vertexNormal.Y = vectorResult.Y;
                        vertexNormal.Z = vectorResult.Z;
                    }
                    pixels = await GetListAsync(graphObject_copy.Groups[0], maxDx, maxDy, -1, windowWidth, windowHeight, bitmap);
                    GraphicModel.Source = bitmap.Source;
                    bitmap.Source.Unlock();
                    
                    //if (pixels != null) { GraphicModel.Source = PixelDrawing.GetBitmap(windowWidth, windowHeight, pixels); }
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

        private async Task<List<Pixel>> GetListAsync(Group group, int dx, int dy, int viceVersa, int width, int height, Bgr24Bitmap bitmap)
        {
            return await Task.Run(() =>
            {
                List<Pixel> pixels = new List<Pixel>();
                int count = 0;
                ZBuffer zBuf = new ZBuffer(width, height);
                Parallel.ForEach(group.Faces, face =>
                {
                    if (IsFaceVisible(group.Vertices, face))
                    {
                        List<Pixel> pixelsForSide = new List<Pixel>();
                        List<Pixel> pixelsInSide = new List<Pixel>();
                        Vertex vertex0, vertex1;
                        int index0, index1;

                        Vector3 lightingVector = new Vector3(0, 0, 1);

                        int ind0 = (face.FaceElements.ElementAt(0).VertexNormalIndex != null) ? (int)face.FaceElements.ElementAt(0).VertexNormalIndex : 1;
                        Vector3 point0Normal = new Vector3(group.VertexNormals[ind0 - 1].X, group.VertexNormals[ind0 - 1].Y, group.VertexNormals[ind0 - 1].Z);

                        int ind1 = (face.FaceElements.ElementAt(1).VertexNormalIndex != null) ? (int)face.FaceElements.ElementAt(1).VertexNormalIndex : 1;
                        Vector3 point1Normal = new Vector3(group.VertexNormals[ind1 - 1].X, group.VertexNormals[ind1 - 1].Y, group.VertexNormals[ind1 - 1].Z);

                        int ind2 = (face.FaceElements.ElementAt(2).VertexNormalIndex != null) ? (int)face.FaceElements.ElementAt(2).VertexNormalIndex : 1;
                        Vector3 point2Normal = new Vector3(group.VertexNormals[ind2 - 1].X, group.VertexNormals[ind2 - 1].Y, group.VertexNormals[ind2 - 1].Z);

                        Color color = Color.FromArgb(255, 255, 0, 0);
                        Color point1Color = Lambert.GetPointColor(point0Normal, lightingVector, color);
                        Color point2Color = Lambert.GetPointColor(point1Normal, lightingVector, color);
                        Color point3Color = Lambert.GetPointColor(point2Normal, lightingVector, color);
                        Color faceColor = PlaneShading.GetAverageColor(point1Color, point2Color, point3Color);

                        for (int i = 0; i < face.FaceElements.Count - 1; i++)
                        {
                            index0 = (face.FaceElements.ElementAt(i).VertexIndex != -2) ? face.FaceElements.ElementAt(i).VertexIndex : (group.Vertices.Count - 1);
                            index1 = (face.FaceElements.ElementAt(i + 1).VertexIndex != -2) ? face.FaceElements.ElementAt(i + 1).VertexIndex : (group.Vertices.Count - 1);
                            vertex0 = group.Vertices.ElementAt(index0);
                            vertex1 = group.Vertices.ElementAt(index1);
                            pixelsForSide.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * viceVersa + dy), (int)(vertex0.Z), (int)(vertex1.X + dx), (int)(vertex1.Y * viceVersa + dy), (int)(vertex1.Z), windowWidth, windowHeight, bitmap, zBuf, faceColor));
                        }
                        index0 = (face.FaceElements.ElementAt(0).VertexIndex != -2) ? face.FaceElements.ElementAt(0).VertexIndex : (group.Vertices.Count - 1);
                        index1 = (face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex != -2) ? face.FaceElements.ElementAt(face.FaceElements.Count - 1).VertexIndex : (group.Vertices.Count - 1);
                        vertex0 = group.Vertices.ElementAt(index0);
                        vertex1 = group.Vertices.ElementAt(index1);
                        pixelsForSide.AddRange(Bresenham.GetPixels((int)(vertex0.X + dx), (int)(vertex0.Y * viceVersa + dy), vertex0.Z, (int)(vertex1.X + dx), (int)(vertex1.Y * viceVersa + dy), vertex1.Z, windowWidth, windowHeight, bitmap, zBuf, faceColor));

                        
                        RastAlgorithm.DrawPixelForRasterization(pixelsForSide, bitmap, zBuf, faceColor);
                        //pixels.AddRange(pixelsForSide);
                        //pixelsInSide.AddRange(RastAlgorithm.DrawPixelForRasterization(pixelsForSide, bitmap, zBuf));
                        //pixels.AddRange(pixelsInSide);
                        count++;
                    }
                });
                return pixels;
            });
        }

        private static bool IsFaceVisible(List<Vertex> pointsList, Face face)
        {
            bool result = true;
            int indexPoint1 = (int)face.FaceElements[0].VertexIndex;
            int indexPoint2 = (int)face.FaceElements[1].VertexIndex;
            int indexPoint3 = (int)face.FaceElements[2].VertexIndex;
            Vector4 point1 = new Vector4(pointsList[indexPoint1].X, pointsList[indexPoint1].Y, pointsList[indexPoint1].Z, pointsList[indexPoint1].W);
            Vector4 point2 = new Vector4(pointsList[indexPoint2].X, pointsList[indexPoint2].Y, pointsList[indexPoint2].Z, pointsList[indexPoint2].W);
            Vector4 point3 = new Vector4(pointsList[indexPoint3].X, pointsList[indexPoint3].Y, pointsList[indexPoint3].Z, pointsList[indexPoint3].W);

            Vector4 vector1 = point2 - point1;
            Vector4 vector2 = point3 - point1;
            Vector3 vector1XYZ = new Vector3(vector1.X, vector1.Y, vector1.Z);
            Vector3 vector2XYZ = new Vector3(vector2.X, vector2.Y, vector2.Z);
            Vector3 normal = Vector3.Cross(vector1XYZ, vector2XYZ);

            if (normal.Z >= 0)
            {
                result = false;
            }

            return result;
        }

        public Matrix4x4 GetOptionsMatrix(TransformOptions options, int width, int height)
        {
            Matrix4x4 model = Matrix4x4.CreateScale(options.Scale) * Matrix4x4.CreateFromYawPitchRoll(options.ModelAroundY, options.ModelAroundX, options.ModelAroundZ)
                * Matrix4x4.CreateTranslation(options.ModelX, options.ModelY, options.ModelZ);
            Matrix4x4 view = Matrix4x4.CreateTranslation(-new Vector3(options.CameraX, options.CameraY, options.CameraZ))
                * Matrix4x4.Transpose(Matrix4x4.CreateFromYawPitchRoll(options.CameraAroundY, options.CameraAroundX, options.CameraAroundZ));
            Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(options.FieldOfView, aspectRation, defaultNearPlaneDistance, defaultFarPlaneDistance);

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
                    CameraAroundX = (float)(cameraAroundX * Math.PI / 180),
                    CameraAroundY = (float)(cameraAroundY * Math.PI / 180),
                    CameraAroundZ = (float)(cameraAroundZ * Math.PI / 180),
                    ModelX = modelX,
                    ModelY = modelY,
                    ModelZ = modelZ,
                    ModelAroundX = (float)(modelAroundX * Math.PI / 180),
                    ModelAroundY = (float)(modelAroundY * Math.PI / 180),
                    ModelAroundZ = (float)(modelAroundZ * Math.PI / 180),
                    FieldOfView = (float)(fieldOfView * Math.PI / 180)
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
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;  
            while (isActive)
            {
                cameraX = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeCameraY(object sender, RoutedEventArgs e)
        {
            if (isActive){ isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;
            while (isActive)
            {
                cameraY = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeCameraZ(object sender, RoutedEventArgs e)
        {
            if (isActive){ isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;
            while (isActive)
            {
                cameraZ = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelX(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;
            while (isActive)
            {
                modelX = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelY(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;
            while (isActive)
            {
                modelY = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelZ(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minRange;
            while (isActive)
            {
                modelZ = i; i++;
                if (i > maxRange) { i = minRange; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeCameraAroundX(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                cameraAroundX = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeCameraAroundY(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                cameraAroundY = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeCameraAroundZ(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                cameraAroundZ = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelAroundX(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                modelAroundX = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelAroundY(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                modelAroundY = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeModelAroundZ(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = minAngle;
            while (isActive)
            {
                cameraAroundZ = i; i++;
                if (i > maxAngle) { i = minAngle; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeFieldOfView(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            int i = 1;
            while (isActive)
            {
                fieldOfView = i; i++;
                if (i > 180) { i = 1; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        async public void ChangeScale(object sender, RoutedEventArgs e)
        {
            if (isActive) { isActive = false; return; }
            isApplyOptions = true;
            isActive = true;
            float i = 1;
            while (isActive)
            {
                scale = i; i++;
                if (i > 100) { i = 1; }
                await DrawMiniatureByMatrix(GetOptionsMatrix(GetUserOptions(scale), miniatureWidth, miniatureHeight), GetViewPort(0, 0, miniatureWidth, miniatureHeight));
            }
        }

        public void StopChange(object sender, RoutedEventArgs e)
        {
            isActive = false;
        }
    }
}
