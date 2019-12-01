using System;
namespace Models
{
    public class TransformOptions : ICloneable
    {
        public float Scale { get; set; }

        public float CameraX { get; set; }
        public float CameraY { get; set; }
        public float CameraZ { get; set; }

        public float CameraAroundX { get; set; }
        public float CameraAroundY { get; set; }
        public float CameraAroundZ { get; set; }

        public float ModelX { get; set; }
        public float ModelY { get; set; }
        public float ModelZ { get; set; }

        public float ModelAroundX { get; set; }
        public float ModelAroundY { get; set; }
        public float ModelAroundZ { get; set; }

        public object Clone()
        {
            return new TransformOptions()
            {
                Scale = this.Scale,
                CameraX = this.CameraX,
                CameraY = this.CameraY,
                CameraZ = this.CameraZ,
                CameraAroundX = this.CameraAroundX,
                CameraAroundY = this.CameraAroundY,
                CameraAroundZ = this.CameraAroundZ,
                ModelX = this.ModelX,
                ModelY = this.ModelY,
                ModelZ = this.ModelZ,
                ModelAroundX = this.ModelAroundX,
                ModelAroundY = this.ModelAroundY,
                ModelAroundZ = this.ModelAroundZ
            };
        }
    }
}
