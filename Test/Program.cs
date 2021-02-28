using Models;
using ObjFileParser;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"D:\IronMan - Copy.obj";
            Parser parser = new Parser();
            GraphObject graphObject = parser.ParseFile(filePath);

            int i = 1;
            
        }
    }
}
