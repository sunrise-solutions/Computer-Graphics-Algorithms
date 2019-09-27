using System.Collections.Generic;

namespace Models
{
    public class GraphObject
    {
        public List<Group> Groups { get; set; }

        public GraphObject()
        {
            Groups = new List<Group>();
        }
    }
}
