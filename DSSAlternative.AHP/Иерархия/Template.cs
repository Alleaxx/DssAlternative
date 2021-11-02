using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace DSSAlternative.AHP
{
    public interface ITemplate : ICloneable
    {
        string Name { get; }
        string Description { get; }
        string Img { get; }

        Node[] Nodes { get; }
        TemplateRelation[] Relations { get; }

        ITemplate CloneThis();
    }

    public class Template : ITemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }

        public DateTime Creation { get; set; }
        public Node[] Nodes { get; set; }
        public TemplateRelation[] Relations { get; set; }


        public Template()
        {
            Creation = DateTime.Now;
            Relations = Array.Empty<TemplateRelation>();
        }
        public Template(IProject project) : this(project.HierarchyActive, project.Relations.SelectMany(c => c.Required))
        {

        }
        public Template(IHierarchy hier, IEnumerable<INodeRelation> relations = null)
        {
            Creation = DateTime.Now;

            var goal = hier.MainGoal;
            Name = $"{goal.Name}";
            Description = !string.IsNullOrEmpty(goal.Description) ? goal.Description : $"Созданный пресет на основе открытой задачи c {hier.Count()} узлами";
            Img = !string.IsNullOrEmpty(goal.ImgPath) ? goal.ImgPath : "settings.svg";
   
            Nodes = hier.OfType<Node>().Select(n => n.CloneThis()).ToArray();
            if(relations != null)
            {
                Relations = relations.Select(r => new TemplateRelation(r)).ToArray();
            }
            else
            {
                Relations = Array.Empty<TemplateRelation>();
            }
        }


        public object Clone()
        {
            return CloneThis();
        }
        public ITemplate CloneThis()
        {
            Node[] copyNodes = Nodes.Select(n => new Node(n.Level, n.Name, n.Group, n.GroupIndex) { ImgPath = n.ImgPath }).ToArray();
            return new Template()
            {
                Nodes = copyNodes,
                Relations = Relations
            };
        }
    }

    public class TemplateRelation
    {
        public string Main { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Value { get; set; }

        public TemplateRelation()
        {

        }
        public TemplateRelation(INodeRelation relation)
        {
            Main = relation.Main.Name;
            From = relation.From.Name;
            To = relation.To.Name;
            Value = relation.Value;
        }
    }
}
