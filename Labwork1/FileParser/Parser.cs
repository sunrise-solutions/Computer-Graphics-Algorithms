using Labwork1.Models;
using System.IO;

namespace Labwork1.FileParser
{
    public static class Parser
    {
        public static GraphObject ParseFile(string filePath)
        {
            GraphObject graphObject = new GraphObject();

            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {

                        }
                    }
                }
            }

            return null;
        }
    }
}
