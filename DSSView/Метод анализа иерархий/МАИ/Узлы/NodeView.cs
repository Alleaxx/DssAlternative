using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

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
        public RelayCommand RemoveThisCommand { get; set; }
        private void CountCoeff(object obj)
        {
            Criteria.UpdateCoeffs();
        }



        public ViewNode(Node alpha)
        {
            Criteria = alpha;
            CountCoeffCommand = new RelayCommand(CountCoeff, obj => true);

            
            Criteria.CoefficientUpdated += Criteria_CoefficientUpdated;
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
