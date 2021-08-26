using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSSLib;

namespace DSSView
{
    public interface IAdviceSystem
    {
        void SetProblem(Problem problem);
        void ClearProblem();
    }
    public interface IHierarchyAdviceSystem : IAdviceSystem
    {
        event Action<IHierarchyAdviceSystem, Problem> Finished;
    }


    public class AdviceSystem : DSSLib.NotifyObj, IAdviceSystem
    {
        public override string ToString() => $"Система заполнения проблемы {Problem.Name}";

        public Problem Problem
        {
            get => problem;
            set
            {
                problem = value;
                OnPropertyChanged();
            }
        }
        private Problem problem;

        public ObservableCollection<Problem> Problems { get; private set; } = new ObservableCollection<Problem>();

        public IHierarchyAdviceSystem HierarchySystem
        {
            get => hierarcySystem;
            set
            {
                if (hierarcySystem != null)
                    hierarcySystem.Finished -= HierarchySystem_Finished;

                hierarcySystem = value;
                OnPropertyChanged();
                hierarcySystem.SetProblem(Problem);
                hierarcySystem.Finished += HierarchySystem_Finished;
            }
        }
        private IHierarchyAdviceSystem hierarcySystem;
        public IAdviceSystem RelationsSystem
        {
            get => relationSystem;
            set
            {
                relationSystem = value;
                OnPropertyChanged();
                relationSystem.SetProblem(Problem);
            }
        }
        private IAdviceSystem relationSystem;
        public IAdviceSystem ResultsSystem
        {
            get => resultSystem;
            set
            {
                resultSystem = value;
                OnPropertyChanged();
                resultSystem.SetProblem(Problem);
            }
        }
        private IAdviceSystem resultSystem;
        
        public IHierarchyAdviceSystem[] HierarchySystems { get; set; } = new IHierarchyAdviceSystem[]
        {
            new AdviceSystemHierarchy()
        };
        public IAdviceSystem[] RelationSystems { get; set; } = new IAdviceSystem[]
        {
            new AdviceSystemRelationsMatrix(),
            new AdviceSystemRelation(),
            new AdviceSystemRelationNode(),
            new SingleChoiceSystemRelations()
        };
        public IAdviceSystem[] ResultSystems { get; set; } = new IAdviceSystem[]
        {
            new AdviceSystemResults()
        };

        
        public RelayCommand OpenProblemCommand { get; set; }
        public RelayCommand SaveProblemCommand { get; set; }
        public RelayCommand CloseProblemCommand { get; set; }
        public RelayCommand OpenTechWindowCommand { get; set; }


        private void OpenProblemFromFile(object obj)
        {
            ISaver<NodeProject> opener = new Saver<NodeProject>();
            NodeProject project = opener.Open();

            if(project != null)
            {
                SetProblem(new Problem(project));
            }
        }
        private void SaveProblemToFile(object obj)
        {
            ISaver<NodeProject> saver = new Saver<NodeProject>();
            saver.Save(Problem.GetSaveVersionAlpha());
        }
        private void CloseProblem(object obj)
        {
            SetProblem(new Problem());
        }
        private void OpenTechWindow(object obj)
        {
            //View..OpenAHPWindow.Execute(null);
        }


        public AdviceSystem() : this(new Problem())
        {

        }
        public AdviceSystem(Problem problem)
        {
            InitCommands();
            Problem = problem;
            HierarchySystem = HierarchySystems[0];
            RelationsSystem = RelationSystems[0];
            ResultsSystem = ResultSystems[0];
            Problems.Add(problem);
        }

        private void InitCommands()
        {
            OpenTechWindowCommand = new RelayCommand(OpenTechWindow, RelayCommand.IsTrue);
            OpenProblemCommand = new RelayCommand(OpenProblemFromFile, RelayCommand.IsTrue);
            SaveProblemCommand = new RelayCommand(SaveProblemToFile, obj => Problem != null);
            CloseProblemCommand = new RelayCommand(CloseProblem, obj => Problem != null);
        }


        public void SetProblem(Problem problem)
        {
            Problem = problem;
            HierarchySystem.SetProblem(problem);
            RelationsSystem.SetProblem(problem);
            ResultsSystem.SetProblem(problem);
            Problems.Add(problem);
        }
        public void ClearProblem()
        {
            SetProblem(new Problem());
        }


        private void HierarchySystem_Finished(IHierarchyAdviceSystem system, Problem obj)
        {
            problem = obj;
            RelationsSystem.SetProblem(obj);
            ResultsSystem.SetProblem(obj);
        }
    }

}
