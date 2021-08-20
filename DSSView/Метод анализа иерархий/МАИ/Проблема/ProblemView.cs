using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public interface IViewProblem
    {
        Problem Source { get; }
        IViewElement Selected { get; set; }
    }

    public class ViewProblem : ViewNode, IViewProblem
    {
        public ViewProblem(Problem problem) : base(problem)
        {

        }
    }
}
