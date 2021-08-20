using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSView
{
    public class NodeProject
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Description { get; set; }
        public double Coefficient { get; set; }

        [XmlArrayItem("Node")]
        public NodeProject[] Criterias { get; set; }

        [XmlArrayItem("row")]
        public double[][] Values { get; set; }
    }
}
