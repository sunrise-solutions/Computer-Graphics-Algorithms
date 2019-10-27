using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class GraphObject : ICloneable
    {
        public List<Group> Groups { get; set; }

        public GraphObject()
        {
            Groups = new List<Group>();
        }

        public GraphObject(List<Group> groups)
        {
            Groups = groups;
        }

        public object Clone()
        {
            var groups = this.Groups.Select(x => (Group)x.Clone()).ToList();

            return new GraphObject(groups);
        }
    }
}
