using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DSSView
{
    public class Problem : AHPCriteriaAlpha
    {
        public override string ToString() => Name;

        public override Problem Main => this;

        public AlternativesAHP Alternatives { get; private set; } = new AlternativesAHP();
        public CriteriasAHP Criterias { get; private set; } = new CriteriasAHP();

        public Dictionary<int, List<double[]>> UpperCoeffs { get; set; } = new Dictionary<int, List<double[]>>();
        public override void CountCoeffs()
        {
            UpperCoeffs.Clear();
            foreach (var item in Dictionary)
            {
                foreach (var rt2 in item.Value)
                {
                    rt2.CountCoeffs();
                }
            }
            base.CountCoeffs();
            Coefficient = 1;
            UpperCoeffs.Add(-1, new List<double[]>() { new double[] { 1 } });

            AHPCriteriaAlpha[] prevs = new AHPCriteriaAlpha[] { this };
            for (int i = 0; i < Dictionary.Count; i++)
            {
                var coeffs = UpperCoeffs[i];
                var criterias = Dictionary.Values.ElementAt(i);
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
                prevs = criterias.ToArray();
            }
        }

        public Problem() : this("Альфа-проблема")
        {
            //AHPCriteriaAlpha F = new AHPCriteriaAlpha(this, "Ф");
            //AHPCriteriaAlpha F1 = new AHPCriteriaAlpha(F, "Ф1");
            //AHPCriteriaAlpha F2 = new AHPCriteriaAlpha(F, "Ф2");
            //AHPCriteriaAlpha F3 = new AHPCriteriaAlpha(F, "Ф3");
            //F.Add(F1, F2, F3);

            //AHPCriteriaAlpha M = new AHPCriteriaAlpha(this, "М");
            //AHPCriteriaAlpha M1 = new AHPCriteriaAlpha(M, "М1");
            //AHPCriteriaAlpha M2 = new AHPCriteriaAlpha(M, "М2");
            //AHPCriteriaAlpha M3 = new AHPCriteriaAlpha(M, "М3");
            //M.Add(M1, M2, M3);

            //AHPCriteriaAlpha O = new AHPCriteriaAlpha(this, "О");
            //AHPCriteriaAlpha O1 = new AHPCriteriaAlpha(O, "О1");
            //AHPCriteriaAlpha O2 = new AHPCriteriaAlpha(O, "О1");
            //AHPCriteriaAlpha O3 = new AHPCriteriaAlpha(O, "О1");
            //O.Add(O1, O2, O3);

            //Add(F, M, O);



            AHPCriteriaAlpha place = new AHPCriteriaAlpha(this,1, "Местоположение");
            AHPCriteriaAlpha reputation = new AHPCriteriaAlpha(this,1,"Репутация");
            Add(0, reputation, place);

            AHPCriteriaAlpha A = new AHPCriteriaAlpha(this, 2, "Универ А");
            AHPCriteriaAlpha B = new AHPCriteriaAlpha(this, 2, "Универ B");
            AHPCriteriaAlpha C = new AHPCriteriaAlpha(this, 2, "Универ C");
            reputation.Add(1, A, B, C);
            place.Add(1, A, B, C);

            AHPCriteriaAlpha F1 = new AHPCriteriaAlpha(this, 3, "Факультет ПИ");
            AHPCriteriaAlpha F2 = new AHPCriteriaAlpha(this, 3, "Факультет БИ");
            A.Add(2, F1, F2);
            B.Add(2, F1, F2);
            C.Add(2, F1, F2);


        }
        public Problem(string name) : base(name)
        {
            Name = name;
            Alternatives.Set(Criterias);
        }
        public Problem(ProblemProject project) : base(project.Name)
        {
            Description = project.Description;

            var crits = new List<CriteriaAHP>(project.Criterias.Select(c => new CriteriaAHP(c)));
            Criterias.AddSave(crits.ToArray());
            foreach (var criteria in Criterias)
            {
                criteria.InitAfterLoading(this,Criterias.ToArray());
            }

            Alternatives.Set(Criterias);

            var alts = new List<AlternativeAHP>(project.Alternatives.Select(a => new AlternativeAHP(a)));
            Alternatives.AddSave(alts.ToArray());
            foreach (var alt in alts)
            {
                alt.InitAfterLoading(Alternatives.ToArray(), Criterias.ToArray());
            }
            Alternatives.UpdateGroups();
        }


        public ProblemProject GetSaveVersion()
        {
            return new ProblemProject()
            {
                Name = Name,
                Description = Description,
                Alternatives = Alternatives.Select(a => a.GetSaveVersion()).ToArray(),
                Criterias = Criterias.Select(c => c.GetSaveVersion()).ToArray()
            };
        }
    }
}
