using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public interface IViewProblem
    {
        Problem Source { get; }
        IViewElement Selected { get; set; }
    }



    public class ViewCriteria : ViewElement, IViewProblem
    {
        public override string ToString() => Criteria.ToString();
        public Problem Source => Criteria as Problem;


        public AHPCriteriaAlpha Criteria { get; protected set; }

        protected ViewCriteria()
        {

        }
        public ViewCriteria(AHPCriteriaAlpha alpha)
        {
            Criteria = alpha;
            //Add(new ViewProblemInfo(problem));
            //Add(new ViewProblemAlternatives(problem));
            //Add(new ViewProblemCriterias(problem));
        }
    }



    public class ViewProblem : ViewElement, IViewProblem
    {
        public override string ToString() => Problem.ToString();
        public Problem Source => Problem;

        public Problem Problem { get; protected set; }

        protected ViewProblem()
        {

        }
        public ViewProblem(Problem problem)
        {
            Problem = problem;
            Add(new ViewProblemInfo(problem));
            Add(new ViewProblemAlternatives(problem));
            Add(new ViewProblemCriterias(problem));
        }
    }
    public class ViewProblemInfo : ViewProblem
    {
        public override string ToString() => "Общие сведения";

        public ViewProblemInfo(Problem problem)
        {
            Problem = problem;
        }
    }
    public class ViewProblemAlternatives : ViewProblem
    {
        public override string ToString() => "Альтернативы";

        public RelayCommand AddCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }


        public CriteriaAHPGroup[] GroupCriterias => Problem.Alternatives.GroupCriterias;

        private void Add(object obj)
        {
            AlternativeAHP newAlternative = new AlternativeAHP(Problem,$"Альтернатива {Problem.Alternatives.Count() + 1}");
            Problem.Alternatives.Add(newAlternative);
            Update();
        }
        private void Remove(object obj)
        {
            if(obj is AlternativeView alt)
            {
                Problem.Alternatives.Remove(Problem.Criterias.ToArray(), alt.Alternative);
                Update();
            }
        }

        public ViewProblemAlternatives(Problem problem)
        {
            AddCommand = new RelayCommand(Add, obj => true);
            RemoveCommand = new RelayCommand(Remove, obj => obj != null && obj is AlternativeView);
            Problem = problem;

            Problem.Alternatives.Added += Alternatives_ListChanged;
            Problem.Alternatives.Removed += Alternatives_ListChanged;
            Problem.Alternatives.CoeffsChanged += Alternatives_CoeffsChanged;
        }

        private void Alternatives_CoeffsChanged(AlternativesAHP obj)
        {
            OnPropertyChanged(nameof(GroupCriterias));
        }

        private void Alternatives_ListChanged(AlternativesAHP arg1, AlternativeAHP arg2)
        {
            OnPropertyChanged(nameof(GroupCriterias));
        }

        public override IEnumerable<IViewElement> Elements => Problem.Alternatives.Select(a => new AlternativeView(Problem,a));
    }
    public class ViewProblemCriterias : ViewProblem
    {
        public override string ToString() => "Критерии";

        public RelayCommand AddCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }


        public MatrixAHP Matrix => Problem.Criterias.Matrix;

        private void Add(object obj)
        {
            CriteriaAHP criteria = new CriteriaAHP(Problem,$"Критерий {Problem.Criterias.Count() + 1}");
            Problem.Criterias.Add(criteria);
            Update();
        }
        private void Remove(object obj)
        {
            if(obj is CriteriaAHPView crit)
            {
                Problem.Criterias.Remove(crit.Criteria);
                Update();
            }
        }


        public ViewProblemCriterias(Problem problem)
        {
            AddCommand = new RelayCommand(Add, obj => true);
            RemoveCommand = new RelayCommand(Remove, obj => obj != null && obj is CriteriaAHPView);
            Problem = problem;

            Problem.Criterias.CoeffsChanged += Criterias_CriteriaCoeffsChanged;
            Problem.Criterias.Added += Problem_ListChanged;
            Problem.Criterias.Removed += Problem_ListChanged;
        }

        private void Criterias_CriteriaCoeffsChanged(CriteriasAHP obj)
        {
            OnPropertyChanged(nameof(Matrix));
        }

        private void Problem_ListChanged(CriteriasAHP arg1, CriteriaAHP arg2)
        {
            OnPropertyChanged(nameof(Elements));
            OnPropertyChanged(nameof(Matrix));
        }

        public override IEnumerable<IViewElement> Elements => Problem.Criterias.Select(c => new CriteriaAHPView(Problem,c));
    }


}
