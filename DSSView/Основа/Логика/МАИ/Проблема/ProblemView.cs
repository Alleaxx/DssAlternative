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
        }

    }
}
