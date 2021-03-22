using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSSView
{
    public class ViewAHP : NotifyObj
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
            AdviceSystem system = new AdviceSystem();
            AHPAdvicorWindow window = new AHPAdvicorWindow(system);
            window.ShowDialog();
        }
        private void OpenProblem(object obj)
        {
            IXMLProvider<NodeProject> provider = new DefaultXmlProvider<NodeProject>(); 
            IFileSelector selector = new DialogFileSelector();
            FileInfo file = selector.Open();

            if(file != null && file.Exists)
            {
                using(FileStream stream = File.OpenRead(file.FullName))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    string xml = Encoding.Default.GetString(array);

                    NodeProject problemProject = provider.FromXml(xml);
                    Problem problem = new Problem(problemProject);
                    Add(new ViewProblem(problem));
                    Selected = Problems.Last();
                }
            }
        }


        private void SaveProblem(object obj)
        {
            IXMLProvider<NodeProject> provider = new DefaultXmlProvider<NodeProject>(); 
            IFileSelector selector = new DialogFileSelector();
            FileInfo file = selector.Save();

            if(file != null)
            {
                string xml = provider.ToXml(Selected.Source.GetSaveVersionAlpha());
                using(FileStream stream = new FileStream(file.FullName, FileMode.Create))
                {
                    byte[] array = Encoding.Default.GetBytes(xml);
                    stream.Write(array, 0, array.Length);
                }
            }
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
