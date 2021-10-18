using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace DSSAlternative.AppComponents
{
    public class Account
    {
        private readonly DSS App;
        private readonly LocalStorage Storage;

        public User CurrentUser { get; private set; }


        public Account(DSS app, LocalStorage storage)
        {
            App = app;
            CurrentUser = new User();
            Storage = storage;
            JsonOptions = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            };
            LoadStored();
        }
        private async void LoadStored()
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
            return await ReadFromJson<List<Template>>(Templates);
        }


        //Состояние
        const string SavedState = "DssState";
        public void LoadState()
        {
            DssState savedState = CurrentUser.State;
            App.LoadState(savedState);
        }
        public async void SaveState()
        {
            await SaveState(new DssState(App));
        }
        public async void RemoveState()
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
        public void SetTemplate(IProject project)
        {
            SetTemplate(new Template(project));
        }
        public async void RemoveTemplate(ITemplate template)
        {
            CurrentUser.Templates.Remove(template as Template);
            await SaveTemplates();
        }
        private async void SetTemplate(ITemplate template)
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
        private readonly JsonSerializerOptions JsonOptions;
        private async Task WriteJson(string key, object obj)
        {
            try
            {
                string json = JsonSerializer.Serialize(obj, JsonOptions);
                await Storage.SetPropAsync(key, json);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Не удалось сериализовать и сохранить объект [{key}]: {ex.Message}");
            }
        }
        private async Task<T> ReadFromJson<T>(string key)
        {
            try
            {
                string json = await Storage.GetValueAsync(key);
                if(json == null)
                {
                    return default;
                }

                T obj = JsonSerializer.Deserialize<T>(json, JsonOptions);
                return obj;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Не удалось десериализовать объект [{key}]: {ex.Message}");
                return default;
            }
        }
    }
}
