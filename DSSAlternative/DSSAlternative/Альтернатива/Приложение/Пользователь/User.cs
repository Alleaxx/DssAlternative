using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AHP
{
    public class Account
    {
        public DSS App { get; set; }
        public User CurrentUser { get; set; }
        public Account(DSS app)
        {
            App = app;
            CurrentUser = new User();
        }

        public void LoadState()
        {
            App.LoadState(CurrentUser.State);
        }
        public void SaveState()
        {
            CurrentUser.State = new DssState(App);
        }


        public void LoadTemplate(ITemplate template)
        {
            App.AddProject(new Project(template));
        }
        public void AddTemplate(IProject project)
        {
            AddTemplate(new Template(project));
        }
        public void AddTemplate(ITemplate template)
        {
            CurrentUser.Templates.Add(template);
        }

        public void RemoveTemplate(ITemplate template)
        {
            CurrentUser.Templates.Remove(template);
        }
        public void UpdateTemplate(ITemplate template)
        {

        }
    }
    public class User
    {
        public string Name { get; set; }
        public List<ITemplate> Templates { get; set; }
        public DssState State { get; set; }
        public bool Loginned { get; set; }

        public User()
        {
            Name = "Гость";
            Loginned = false;
            Templates = new List<ITemplate>();
        }
    }
}
