using Models;
using ObjFileParser.Parsers;
using System.IO;

namespace ObjFileParser
{
    public class Parser
    {
        bool isLastLineAFace = false;
        GraphObject graphObject = new GraphObject();
        Group group;

        public GraphObject ParseFile(string filePath)
        {            
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        group = new Group();
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

            string data = null;
            var fields = line.Trim().Split(null, 2);
            var keyword = fields[0].Trim();
            if (fields.Length > 1)
            {
                data = fields[1].Trim();
            }
            else
            {
                return;
            }   

            ParseLine(keyword, data);
        }

        private void ParseLine(string keyword, string data)
        {
            switch (keyword)
            {
                case "v":
                    if (isLastLineAFace)
                    {
                        graphObject.Groups.Add(group);
                        group = new Group();
                        isLastLineAFace = false;
                    }                  
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
                    isLastLineAFace = true;
                    break;
                default:
                    break;
            }
        }    
    }
}
