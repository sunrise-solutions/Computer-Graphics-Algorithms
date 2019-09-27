using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labwork1.Models
{
    // List of texture coordinates, in (u, [v ,w]) coordinates, these will vary between 0 and 1, v and w are optional and default to 0.
    // vt 0.500 1 [0]
    public class VertexTexture
    {
        // ?
        public int Id { get; set; }

        public int U { get; set; }

        public int V { get; set; }

        public int W { get; set; }
    }
}
