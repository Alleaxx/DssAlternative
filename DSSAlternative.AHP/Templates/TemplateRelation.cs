using DSSAlternative.AHP.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAlternative.AHP.Templates
{
    /// <summary>
    /// Шаблон для отношения
    /// </summary>
    public class TemplateRelation
    {
        public string Main { get; set; }
        public int Level { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Value { get; set; }

        public TemplateRelation()
        {

        }
        public TemplateRelation(IRelationNode relation)
        {
            Level = relation.Main.Level;
            Main = relation.Main.Name;
            From = relation.From.Name;
            To = relation.To.Name;
            Value = relation.Value;
        }
    }
}
