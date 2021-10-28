using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSAHP
{
    public class ViewTree : DSSLib.NotifyObj
    {
        public ObservableCollection<ITreeDecision> Problems { get; set; }

        public ITreeDecision SelectedProblem
        {
            get => selectedProblem;
            set
            {
                selectedProblem = value;
                OnPropertyChanged();
            }
        }
        private ITreeDecision selectedProblem;


        private void AddProblem(object obj)
        {
            if(obj is InfoAltCase info)
            {
                Problems.Add(new AlternativeTree("Условие задачи") );
            }
        }
        private void RemoveProblem(object obj)
        {
            Problems.Remove(obj as ITreeDecision);
        }

        public ViewTree()
        {
            Problems = new ObservableCollection<ITreeDecision>();
            ITreeDecision altCase = new AlternativeTree("Бросок монетки");
            altCase.AddBranch(new CaseTree("Выпадение решки", 0.5, 1));
            altCase.AddBranch(new CaseTree("Выпадение орла", 0.5, -1));

            Problems.Add(altCase);
            SelectedProblem = altCase;


            //ITreeDecision altCase2 = new AlternativeTree("Бросок монетки v.2.0");
            //altCase2.AddBranch(new CaseTree("Выпадение решки", 0.5, 1));


            //ITreeDecision orel = new CaseTree("Выпадение орла", 0.5, -1);
            //orel.AddBranch(new AlternativeTree("Больше не бросаем"));
            //orel.AddBranch(altCase);
            //altCase2.AddBranch(orel);
            //Problems.Add(altCase2);

            ITreeDecision company = new AlternativeTree("Разработка витамина", -20);
            company.AddBranch(new CaseTree("А купила", 0.5, 30));
            company.AddBranch(new CaseTree("А не купила", 0.5, 0));

            ITreeDecision company2 = new AlternativeTree("Разработка витамина", -20);


            ITreeDecision caseTreeAPos = new CaseTree("A купила", 0.5, 30);
            ITreeDecision caseTreeANeg = new CaseTree("A не купила", 0.5, 0);


            ITreeDecision caseTreeBPos = new CaseTree("B купила", 0.5, 30);
            ITreeDecision caseTreeBNeg = new CaseTree("B не купила", 0.5, 0);


            ITreeDecision caseTreeBPos1 = new CaseTree("B купила", 0.5, 30);
            ITreeDecision caseTreeBNeg1 = new CaseTree("B не купила", 0.5, 0);

            caseTreeANeg.AddBranch(caseTreeBPos);
            caseTreeANeg.AddBranch(caseTreeBNeg);
            caseTreeAPos.AddBranch(caseTreeBPos1);
            caseTreeAPos.AddBranch(caseTreeBNeg1);

            company2.AddBranch(caseTreeANeg);
            company2.AddBranch(caseTreeAPos);


            Problems.Add(company);
            Problems.Add(company2);


        }
    }
    public class InfoAltCase
    {
        public string Name { get; set; }
    }


}
