using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

using DSSAlternative.Services;
namespace DSSAlternative.AppComponents
{
    public interface IAccount
    {
        User CurrentUser { get; }
        void LoadState();
        Task SaveState();
        Task RemoveState();

        Task SetTemplate(IProject project);
        Task RemoveTemplate(ITemplate template);
    }
    public class Account : IAccount
    {
        private readonly IDssProjects DssProjects;
        private readonly IDssJson Json;
        private readonly ILocalStorage Storage;

        public User CurrentUser { get; private set; }


        public Account(IDssProjects app, ILocalStorage storage, IDssJson json)
        {
            DssProjects = app;
            CurrentUser = new User();
            Storage = storage;
            Json = json;
            LoadStored();
        }
        private async Task LoadStored()
        {
            CurrentUser.State = await GetState();
            CurrentUser.Templates = (await GetTemplates()).ToList();
            LoadState();
        }
        private async Task<DssState> GetState()
        {
            var state = await ReadFromJson<DssState>(SavedState);
            if(state == null)
            {
                return new DssState();
            }
            return state;
        }
        private async Task<IEnumerable<Template>> GetTemplates()
        {
            var list = await ReadFromJson<List<Template>>(Templates);
            if(list == null)
            {
                list = new List<Template>();
            }
            return list;
        }


        //Состояние
        const string SavedState = "DssState";
        public void LoadState()
        {
            DssState savedState = CurrentUser.State;
            DssProjects.LoadState(savedState);
        }
        public async Task SaveState()
        {
            await SaveState(new DssState(DssProjects));
        }
        public async Task RemoveState()
        {
            DssState newState = new DssState() { Saved = DateTime.Now };
            await SaveState(newState);
        }
        private async Task SaveState(DssState state)
        {
            CurrentUser.State = state;
            await WriteJson(SavedState, state);
        }

        //Шаблоны
        const string Templates = "Templates";
        public async Task SetTemplate(IProject project)
        {
            await SetTemplate(new Template(project));
        }
        public async Task RemoveTemplate(ITemplate template)
        {
            CurrentUser.Templates.Remove(template as Template);
            await SaveTemplates();
        }
        private async Task SetTemplate(ITemplate template)
        {
            if(template is Template save)
            {
                CurrentUser.Templates.Add(save);
                await SaveTemplates();
            }
        }
        private async Task SaveTemplates()
        {
            await WriteJson(Templates, CurrentUser.Templates);
        }


        //Json
        private async Task WriteJson(string key, object obj)
        {
            string json = Json.ToJson(obj);
            if (!string.IsNullOrEmpty(json))
            {
                await Storage.SetPropAsync(key, json);
            }
        }
        private async Task<T> ReadFromJson<T>(string key)
        {
            string json = await Storage.GetValueAsync(key);
            T obj = Json.FromJson<T>(json);
            return obj;
        }
    }
}
