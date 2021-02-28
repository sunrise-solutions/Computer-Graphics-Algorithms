using System.Collections.Generic;
using System;
using System.Linq;

namespace Models
{
    // Polygonal face element.
    // f 1 2 3
    // f 3/1 4/2 5/3
    // f 6/4/1 3/5/3 7/6/5
    // f 7//1 8//2 9//3
    public class Face : ICloneable
    {     
        public List<FaceElement> FaceElements { get; set; }

        public Face()
        {
            FaceElements = new List<FaceElement>();
        }

        public Face(List<FaceElement> faceElements)
        {
            FaceElements = faceElements;
        }

        public object Clone()
        {
            var faceElements = this.FaceElements.Select(x => (FaceElement)x.Clone()).ToList();

            return new Face(faceElements);
        }
    }
}
