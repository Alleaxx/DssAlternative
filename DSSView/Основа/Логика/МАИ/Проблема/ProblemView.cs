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

    public class ViewProblem : ViewNode, IViewProblem
    {
        public RelayCommand OpenAdvicorWindow { get; set; }
        private void OpenAdvicor(object obj)
        {
            AdviceSystem system = new AdviceSystem(Criteria as Problem);
            AHPAdvicorWindow window = new AHPAdvicorWindow(system);
            window.ShowDialog();
        }

        public ViewProblem(Problem problem) : base(problem)
        {
            OpenAdvicorWindow = new RelayCommand(OpenAdvicor, obj => true);
            Add(new ViewNode(problem));
            Criteria.StructureChanged += Problem_StructureChanged;
        }

        private void Problem_StructureChanged(Node obj)
        {
            //OnPropertyChanged(nameof(Elements));
        }

        public override IEnumerable<IViewElement> Elements
        {
            get
            {
                List<IViewElement> collection = new List<IViewElement>();
                collection.AddRange(Collection);
                foreach (var group in Criteria.Dictionary)
                {
                    ViewElement newElement = new ViewElement();
                    newElement.Name = $"Уровень {group.Key}";
                    newElement.Add(group.Value.Select(cr => new ViewNode(cr)).ToArray());
                    collection.Add(newElement);
                }
                return collection;
            }
        }

    }
}
