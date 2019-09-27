using System.Collections.Generic;

namespace Labwork1.Models
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
