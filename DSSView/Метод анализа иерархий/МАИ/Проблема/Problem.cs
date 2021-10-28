using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSAHP
{
    public class Problem : Node
    {
        public override string ToString() => Name;

        public override Problem Main => this;
        public List<Exception> Exceptions { get; set; } = new List<Exception>();


        public override void UpdateCoeffs()
        {
            Coefficient = 1;
            foreach (var criteriaLevel in Dictionary)
            {
                foreach (var criteria in criteriaLevel.Value)
                {
                    if(criteria != this)
                        criteria.UpdateCoeffs();
                }
            }

            try
            {
                for (int i = 1; i < Dictionary.Count; i++)
                {
                    var criterias = Dictionary[i];
                    var prevs = Dictionary[i - 1];
                    var coeffs = prevs.Select(prev => prev.Matrix.Coeffiients).ToList();
                    for (int c = 0; c < criterias.Count; c++)
                    {
                        double coeff = 0;
                        for (int a = 0; a < coeffs.Count; a++)
                        {
                            double first = coeffs[a][c];
                            double sec = prevs[a].Coefficient;

                            coeff += first * sec;
                        }
                        criterias[c].Coefficient = coeff;
                    }
                }
                UpdateCoefficientEvent();
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
            }
        }


        public Problem() : base(0, "Альфа-проблема")
        {
            Dictionary[0] = new NodeList() { this };
        }
        public Problem(string name) : base(0, name)
        {
            Dictionary[0] = new NodeList() { this };
        }
        public Problem(NodeHierarcy node) : base(0, node.Name)
        {
            Dictionary[0] = new NodeList() { this };
            Description = node.Description;
        }
        public Problem(NodeProject project) : base(null,project)
        {
            Dictionary[0] = new NodeList() { this };

            Dictionary<Node, NodeProject> dict = new Dictionary<Node, NodeProject>();
            var groupedLevel = project.Criterias.GroupBy(crit => crit.Level).OrderBy(g => g.Key);
            foreach (var group in groupedLevel)
            {
                foreach (var criteria in group)
                {
                    Node newCriteria = new Node(this, criteria);
                    dict.Add(newCriteria, criteria);
                    AddInner(newCriteria);
                }
            }

           
            foreach (var item in dict)
            {
                item.Key.LoadValues(item.Value.Values);
            }
            LoadValues(project.Values);
        }





        public override NodeProject GetSaveVersionAlpha()
        {
            var basic = base.GetSaveVersionAlpha();
            List<Node> criterias = new List<Node>();

            foreach (var levelGroup in Dictionary)
            {
                if (levelGroup.Key == 0)
                    continue;
                else
                    criterias.AddRange(levelGroup.Value);
            }

            basic.Criterias = criterias.Select(crit => crit.GetSaveVersionAlpha()).ToArray();
            return basic;
        }
    }
}
