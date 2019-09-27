using Labwork1.Models;
using System.IO;

namespace Labwork1.FileParser
{
    public class Parser
    {
        GraphObject graphObject = new GraphObject();
        Group group = new Group();

        public GraphObject ParseFile(string filePath)
        {            
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            ParseLine(line);
                        }
                    }
                }
            }
            graphObject.Groups.Add(group);
            return graphObject;
        }

        private void ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
            {
                return;
            }

            var fields = line.Trim().Split(null, 2);
            var keyword = fields[0].Trim();
            var data = fields[1].Trim();

            ParseLine(keyword, data);
        }

        private void ParseLine(string keyword, string data)
        {
            switch (keyword)
            {
                case "v":
                    group.Vertices.Add(VertexParser.Parse(data));
                    break;
                case "vt":
                    group.VertexTextures.Add(VertexTextureParser.Parse(data));
                    break;
                case "vn":
                    group.VertexNormals.Add(VertexNormalParser.Parse(data));
                    break;
                case "f":
                    group.Faces.Add(FaceParser.Parse(data));
                    break;
                default:
                    break;
            }
        }    
    }
}
