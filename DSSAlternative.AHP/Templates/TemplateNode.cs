using DSSAlternative.AHP.HierarchyInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Templates
{
    /// <summary>
    /// Модель для узла
    /// </summary>
    public class TemplateNode : ICloneable
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string Group { get; set; }
        public string GroupOwner { get; set; }

        public TemplateNode()
        {

        }
        public TemplateNode(INode node)
        {
            Name = node.Name;
            Level = node.Level;
            Group = node.Group;
            GroupOwner = node.GroupOwner;
        }


        public INode CreateNode()
        {
            return new Node(Level, Name, Group, GroupOwner);
        }

        public object Clone()
        {
            return CloneThis();
        }
        public TemplateNode CloneThis()
        {
            return new TemplateNode()
            {
                Name = this.Name,
                Level = this.Level,
                Group = this.Group,
                GroupOwner = this.GroupOwner
            };
        }
    }
}
