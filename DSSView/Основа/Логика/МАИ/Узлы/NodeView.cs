using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class ViewNode : ViewElement, IViewProblem
    {
        public override string ToString() => Criteria.ToString();
        public Problem Source => Criteria as Problem;


        public Node Criteria { get; protected set; }
        public ViewNode[] Inner => Criteria.Inner.Select(cr => new ViewNode(cr)).ToArray();
        public MatrixAHP Matrix => Criteria.Matrix;
        public List<NodeRelation> Relations => Criteria.Relations;
        public IGrouping<Node, NodeRelation>[] Grouped => Criteria.Grouped;


        public Dictionary<int, NodeList> Structure => Criteria.Dictionary;



        public RelayCommand CountCoeffCommand { get; set; }
        public RelayCommand AddInnerCommand { get; set; }
        public RelayCommand RemoveThisCommand { get; set; }
        private void CountCoeff(object obj)
        {
            Criteria.UpdateCoeffs();
        }
        private void AddInner(object obj)
        {
            char symbol = (char)(65 + Criteria.Level);


            Node newCriteria = new Node(Criteria.Main, Criteria.Level + 1, $"{symbol}{Criteria.Inner.Count}");
            Criteria.AddInner(newCriteria);

            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(Inner));
        }
        public void RemoveThis(object obj)
        {
            Criteria.RemoveThis();
        }


        public ViewNode(Node alpha)
        {
            Criteria = alpha;
            CountCoeffCommand = new RelayCommand(CountCoeff, obj => true);
            AddInnerCommand = new RelayCommand(AddInner, obj => true);
            RemoveThisCommand = new RelayCommand(RemoveThis,obj => !(Criteria is Problem));

            
            Criteria.CoefficientUpdated += Criteria_CoefficientUpdated;
            Criteria.StructureChanged += Criteria_InnerListChanged;
        }

        private void Criteria_InnerListChanged(Node arg1)
        {
            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(Inner));
            OnPropertyChanged(nameof(Matrix));
            OnPropertyChanged(nameof(Relations));
            OnPropertyChanged(nameof(Grouped));
        }

        private void Criteria_CoefficientUpdated(Node obj)
        {
            OnPropertyChanged(nameof(Matrix));
        }

        public override IEnumerable<IViewElement> Elements
        {
            get
            {
                List<IViewElement> elems = new List<IViewElement>();
                elems.AddRange(Criteria.Inner.Select(critInner => new ViewNode(critInner)));
                return elems;
            }
        }
    }
}
