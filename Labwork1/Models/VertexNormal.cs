using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labwork1.Models
{
    // List of vertex normals in (x,y,z) form; normals might not be unit vectors.
    // vn 0.707 0.000 0.707
    public class VertexNormal
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }
    }
}
