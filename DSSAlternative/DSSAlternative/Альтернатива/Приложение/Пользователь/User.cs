using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DSSAlternative.AHP;

namespace DSSAlternative.AppComponents
{
    public class Account
    {
        private readonly DSS App;
        private readonly LocalStorage Storage;
        public User CurrentUser { get; set; }
        public Account(DSS app, LocalStorage storage)
        {
            App = app;
            CurrentUser = new User();
            Storage = storage;
        }

        public void LoadState()
        {
            App.LoadState(CurrentUser.State);
        }
        public async void SaveState()
        {
            CurrentUser.State = new DssState(App);
            await Storage.SetPropAsync("testKeyDate", DateTime.Now);
            Console.WriteLine("Что записали:");
            string test = await Storage.GetValueAsync("testKeyDate");
            Console.WriteLine(test);
            await Storage.ClearAll();
            Console.WriteLine("Очистили хранилище:");
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
