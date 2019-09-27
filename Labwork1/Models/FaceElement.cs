using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labwork1.Models
{
    public class FaceElement
    {
        public GeometricVertex GeometricVertex { get; set; };

        public VertexTexture VertexTexture { get; set; };

        public VertexNormal VertexNormal { get; set; };
    }
}
