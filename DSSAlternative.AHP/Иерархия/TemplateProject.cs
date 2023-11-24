using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace DSSAlternative.AHP
{
    /// <summary>
    /// Шаблон задачи с заданной иерархией и отношениями. Сохраняется в JSON
    /// </summary>
    public interface ITemplateProject : ICloneable
    {
        string Name { get; }
        string Description { get; }
        string Img { get; }

        /// <summary>
        /// Список узлов
        /// </summary>
        Node[] Nodes { get; }
        TemplateRelation[] Relations { get; }

        ITemplateProject CloneThis();
    }

    /// <summary>
    /// Шаблон задачи с заданной иерархией и отношениями. Сохраняется в JSON
    /// </summary>
    public class TemplateProject : ITemplateProject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime Creation { get; set; }

        public Node[] Nodes { get; set; }

        //Список отношений
        public TemplateRelation[] Relations { get; set; }


        public TemplateProject()
        {
            Creation = DateTime.Now;
            Relations = Array.Empty<TemplateRelation>();
        }
        public TemplateProject(IProject project) : this(project.HierarchyActive, project.Relations.SelectMany(c => c.Required))
        {

        }
        public TemplateProject(IHierarchy hier, IEnumerable<INodeRelation> relations = null)
        {
            Creation = DateTime.Now;

            var goal = hier.MainGoal;
            Name = $"{goal.Name}";
            Description = !string.IsNullOrEmpty(goal.Description) ? goal.Description : $"Созданный пресет на основе открытой задачи c {hier.Nodes.Count()} узлами";
            Img = !string.IsNullOrEmpty(goal.ImgPath) ? goal.ImgPath : "settings.svg";
   
            Nodes = hier.Nodes.OfType<Node>().Select(n => n.CloneThis()).ToArray();
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
        public ITemplateProject CloneThis()
        {
            Node[] copyNodes = Nodes.Select(n => new Node(n.Level, n.Name, n.Group, n.GroupOwner) { ImgPath = n.ImgPath }).ToArray();
            return new TemplateProject()
            {
                Nodes = copyNodes,
                Relations = Relations
            };
        }
    }
}
