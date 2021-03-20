using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSView
{
    public class Project
    {

    }
    public class ProblemProject : Project
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public AlternativeAHPProject[] Alternatives { get; set; }
        public CriteriaAHPProject[] Criterias { get; set; }
    }

    public class CriteriaAHPProject : Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Coefficient { get; set; }

        public CriteriaAHPRelationProject[] Coeffs { get; set; }
    }


    public class CriteriaAHPRelationProject : Project
    {        
        [XmlAttribute]
        public string To { get; set; }
        [XmlAttribute]
        public double Value { get; set; }
    }

    public class AlternativeAHPProject : Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Coefficient { get; set; }

        public AlternativeAHPRelationProject[] Relations { get; set; }
    }
    public class AlternativeAHPRelationProject : Project
    {
        [XmlAttribute]
        public string Criteria { get; set; }        
        [XmlAttribute]
        public string To { get; set; }
        [XmlAttribute]
        public double Value { get; set; }
    } 
}
