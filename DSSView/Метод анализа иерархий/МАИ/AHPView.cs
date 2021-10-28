using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSAHP
{
    public class ViewAHP : DSSLib.NotifyObj
    {
        public ObservableCollection<IViewProblem> Problems { get; private set; }
        public IViewProblem Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged();
            }
        }
        private IViewProblem selected;


        public RelayCommand CreateProblemCommand { get; private set; }
        public RelayCommand CreateProblemAlphaCommand { get; private set; }
        public RelayCommand CloseProblemCommand { get; private set; }
        public RelayCommand OpenProblemCommand { get; private set; }
        public RelayCommand SaveProblemCommand { get; private set; }

        private bool IsProblemAvailable(object obj) => Selected != null;

        private void CreateProblem(object obj)
        {
            Problem newProblem = new Problem($"Проблема №{Problems.Count}");
            IViewProblem view = new ViewProblem(newProblem);
            Add(view);
        }
        private void CreateProblemAlpha(object obj)
        {
            //AdviceSystem system = new AdviceSystem();
            //AHPAdvicorWindow window = new AHPAdvicorWindow(system);
            //window.ShowDialog();
        }
        private void OpenProblem(object obj)
        {
            ISaver<NodeProject> opener = new Saver<NodeProject>();
            NodeProject project = opener.Open();

            if(project != null)
            {
                Problem problem = new Problem(project);
                Add(new ViewProblem(problem));
                Selected = Problems.Last();
            }
        }


        private void SaveProblem(object obj)
        {
            ISaver<NodeProject> saver = new Saver<NodeProject>();
            saver.Save(Selected.Source.GetSaveVersionAlpha());
        }
        private void CloseProblem(object obj)
        {
            Remove(Selected);
        }
        

        public void Add(params IViewProblem[] problems)
        {
            foreach (var problem in problems)
            {
                Problems.Add(problem);
            }
        }
        public void Remove(params IViewProblem[] problems)
        {
            for (int i = 0; i < problems.Length; i++)
            {
                IViewProblem problem = problems[i];
                Problems.Remove(problem);
                if (Selected == problem)
                    Selected = null;
            }
        }


        public ViewAHP()
        {
            InitCommands();
            Problems = new ObservableCollection<IViewProblem>();
        }
        private void InitCommands()
        {
            CreateProblemCommand = new RelayCommand(CreateProblem, obj => true);
            CreateProblemAlphaCommand = new RelayCommand(CreateProblemAlpha, obj => true);
            SaveProblemCommand = new RelayCommand(SaveProblem, IsProblemAvailable);
            OpenProblemCommand = new RelayCommand(OpenProblem, obj => true);
            CloseProblemCommand = new RelayCommand(CloseProblem, IsProblemAvailable);
        }
    }
}
