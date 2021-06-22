using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public interface ITemplate
    {
        string Name { get; }
        string Description { get; }
        string Img { get; }
        Node[] Nodes { get; }
    }

    public class Template : ITemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public Node[] Nodes { get; set; }

        public Template()
        {

        }
    }
}
