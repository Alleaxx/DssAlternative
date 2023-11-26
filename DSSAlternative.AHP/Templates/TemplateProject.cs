using DSSAlternative.AHP.HierarchyInfo;
using DSSAlternative.AHP.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace DSSAlternative.AHP.Templates
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
        TemplateNode[] Nodes { get; }
        /// <summary>
        /// Список отношений. Может быть пустым
        /// </summary>
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

        public TemplateNode[] Nodes { get; set; }
        public TemplateRelation[] Relations { get; set; }


        public TemplateProject()
        {
            Creation = DateTime.Now;
            Relations = Array.Empty<TemplateRelation>();
        }
        /// <summary>
        /// Из проекта берется активная иерархия и отбираются отношения
        /// </summary>
        public TemplateProject(IProject project) : this(project.HierarchyActive, project.Relations.GetAllNodeComparesMini())
        {

        }
        public TemplateProject(IHierarchy hier, IEnumerable<IRelationNode> relations = null)
        {
            Creation = DateTime.Now;

            var goal = hier.MainGoal;
            Name = $"{goal.Name}";
            Description = !string.IsNullOrEmpty(goal.Description) ? goal.Description : $"Созданный пресет на основе открытой задачи c {hier.Nodes.Count()} узлами";
            Img = !string.IsNullOrEmpty(goal.ImgPath) ? goal.ImgPath : "settings.svg";
   
            Nodes = hier.Nodes.Select(n => new TemplateNode(n)).ToArray();
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
            var copyNodes = Nodes.Select(n => n.CloneThis()).ToArray();
            return new TemplateProject()
            {
                Nodes = copyNodes,
                Relations = this.Relations
            };
        }
    }
}
