using Labwork1.Models;
using System;

namespace Labwork1.FileParser
{
    public static class VertexParser
    {
        public static Vertex Parse(string line)
        {
            string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            Vertex vertex = new Vertex();

            vertex.X = parts[0].ParseInvariantFloat();
            vertex.Y = parts[1].ParseInvariantFloat();
            vertex.Z = parts[2].ParseInvariantFloat();
            vertex.Z = (parts.Length > 3) ? parts[2].ParseInvariantFloat() : 1;

            return vertex;
        }
    }
}
