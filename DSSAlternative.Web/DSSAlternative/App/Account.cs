using DSSAlternative.AHP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

using DSSAlternative.AHP.Templates;
using DSSAlternative.Web.Services;
using System.Net.Http;
using DSSAlternative.AHP.Logs;

namespace DSSAlternative.Web.AppComponents
{
    /// <summary>
    /// Интерфейс аккаунта, хранит сохраненные и предустановленные шаблоны, позволяет сохранять активные задачи
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Возникает при завершении первоначальной загрузки пресетов, сохраненных шаблонов и состояния
        /// </summary>
        event Action OnLoadEnded;
        /// <summary>
        /// Возникает при изменении списка сохраненных шаблонов
        /// </summary>
        event Action OnUserTemplatesChanged;

        /// <summary>
        /// Список предустановленных шаблонов
        /// </summary>
        IReadOnlyCollection<ITemplateProject> TemplatesPresets { get; }
        /// <summary>
        /// Список сохраненных пользователем шаблонов
        /// </summary>
        IReadOnlyCollection<ITemplateProject> TemplatesUserSaved { get; }

        /// <summary>
        /// Восстановить последнее сохраненное состояние
        /// </summary>
        void LoadLastState();
        /// <summary>
        /// Сохранить состояние с текущими активными проектами
        /// </summary>
        Task SaveStateWithActiveProjects();
        /// <summary>
        /// Очистить сохраненное состояние
        /// </summary>
        Task ClearSavedState();

        Task AddUserTemplate(IProject project);
        Task RemoveUserTemplate(ITemplateProject template);

        /// <summary>
        /// Загрузить пресеты, сохраненные шаблоны и задачи
        /// </summary>
        Task LoadAll();
        /// <summary>
        /// Состояние загрузки пресетов, шаблонов и задач.
        /// null - не начиналась. 
        /// false - началась, но не завершилась.
        /// true - завершилась
        /// </summary>
        bool? IsLoaded { get; }
    }


    /// <summary>
    /// Интерфейс аккаунта, хранит сохраненные и предустановленные шаблоны, позволяет сохранять активные задачи
    /// </summary>
    public class Account : IAccount
    {
        public event Action OnLoadEnded;
        public event Action OnUserTemplatesChanged;


        private readonly IProjectsCollection ProjectsCollection;
        private readonly IJsonService JsonService;
        private readonly ILocalStorageService LocalStorage;
        private readonly HttpClient HttpClient;
        private readonly ILogger Logger;

        public Account(IProjectsCollection app, ILocalStorageService storage, IJsonService json, HttpClient httpClient, ILogger logger)
        {
            ProjectsCollection = app;
            LocalStorage = storage;
            JsonService = json;
            HttpClient = httpClient;
            Logger = logger;

            TemplatesPresetsList = new List<ITemplateProject>();
            TemplatesSavedList = new List<ITemplateProject>();
        }

        public bool? IsLoaded { get; private set; }
        public async Task LoadAll()
        {
            IsLoaded = false;
            Logger.AddInfo(this, "Загружаем пресеты, сохраненные шаблоны, открытые задачи", cate: LogCategory.Account);
            try
            {
                await LoadTemplatePresets();
                await LoadSavedTemplates();
                await LoadStoredState();
            }
            catch(Exception ex)
            {
                Logger.AddError(this, "Не удалось загрузить что-то загрузить", ex.Message, cate: LogCategory.Account);
            }
            IsLoaded = true;
            OnLoadEnded?.Invoke();
        }


        #region Предустановленные шаблоны

        public IReadOnlyCollection<ITemplateProject> TemplatesPresets => TemplatesPresetsList;
        private List<ITemplateProject> TemplatesPresetsList;

        private readonly string[] LoadPathes = new string[]
        {
            "legacy-study.json",
            "legacy-study-advanced.json",
            "legacy-riscs.json",
            "project-team-task.json",
            "project-tools-task.json",
            "work-task.json",
            "buy-task.json",
        };
        private async Task LoadTemplatePresets()
        {
            TemplatesPresetsList = new List<ITemplateProject>();
            foreach (var path in LoadPathes)
            {
                ITemplateProject template = await LoadTemplate($"sample-data/{path}");
                if (template != null)
                {
                    TemplatesPresetsList.Add(template);
                }
            }
            Logger.AddInfo(this, "Предустановленные шаблоны успешно загружены", $"Загружено {TemplatesPresetsList.Count} пресетов: {string.Join("; ",TemplatesPresetsList.Select(t => t.Name))}", LogCategory.Account);
        }

        #endregion

        #region Сохраненные шаблоны

        public IReadOnlyCollection<ITemplateProject> TemplatesUserSaved => TemplatesSavedList;
        private List<ITemplateProject> TemplatesSavedList;

        const string TemplatesKey = "Templates";
        private async Task LoadSavedTemplates()
        {
            var savedTemplates = await ReadJsonFromStorage<List<TemplateProject>>(TemplatesKey);
            if(savedTemplates != null)
            {
                foreach (var template in savedTemplates)
                {
                    TemplatesSavedList.Add(template);
                }
            }
            Logger.AddInfo(this, "Сохраненные шаблоны успешно загружены", $"Загружено {TemplatesSavedList.Count} пресетов: {string.Join("; ", TemplatesSavedList.Select(t => t.Name))}", LogCategory.Account);

        }
        private async Task<ITemplateProject> LoadTemplate(string path)
        {
            string json = await HttpClient.GetStringAsync($"{path}");
            var template = JsonService.CreateFromJson<TemplateProject>(json);
            return template;
        }
        public async Task AddUserTemplate(IProject project)
        {
            var newTemplate = new TemplateProject(project);
            if (newTemplate is TemplateProject save)
            {
                TemplatesSavedList.Add(save);
                await SaveStorageTemplates();
                OnUserTemplatesChanged?.Invoke();
            }
        }
        public async Task RemoveUserTemplate(ITemplateProject template)
        {
            TemplatesSavedList.Remove(template as TemplateProject);
            await SaveStorageTemplates();
            OnUserTemplatesChanged?.Invoke();
        }
        private async Task SaveStorageTemplates()
        {
            await WriteAsJsonToStorage(TemplatesKey, TemplatesSavedList);
        }

        #endregion

        #region Состояние

        private ProjectsSavedState LastUsedState { get; set; }
        const string SavedStateKey = "DssState";
        private async Task LoadStoredState()
        {
            if (LastUsedState == null)
            {
                string json = await LocalStorage.GetValueAsync(SavedStateKey);
                var state = JsonService.CreateFromJson<ProjectsSavedState>(json);
                LastUsedState = state;
            }
            LoadLastState();
            var projectsActive = ProjectsCollection.ActiveProjects;
            Logger.AddInfo(this, "Открытые задачи успешно восстановлены", $"Загружено {projectsActive.Count} задач: {string.Join("; ", projectsActive.Select(t => t.Name))}", LogCategory.Account);

        }
        public void LoadLastState()
        {
            ProjectsCollection.LoadProjectsFromState(LastUsedState);
        }
        public async Task SaveStateWithActiveProjects()
        {
            var newState = new ProjectsSavedState(ProjectsCollection);
            await SaveStorageState(newState);
        }
        public async Task ClearSavedState()
        {
            var newState = new ProjectsSavedState();
            await SaveStorageState(newState);
        }
        private async Task SaveStorageState(ProjectsSavedState state)
        {
            LastUsedState = state;
            await WriteAsJsonToStorage(SavedStateKey, state);
        }
        
        #endregion

        private async Task WriteAsJsonToStorage(string key, object obj)
        {
            string json = JsonService.ConvertToJson(obj);
            if (!string.IsNullOrEmpty(json))
            {
                await LocalStorage.SetPropAsync(key, json);
            }
        }
        private async Task<T> ReadJsonFromStorage<T>(string key)
        {
            string json = await LocalStorage.GetValueAsync(key);
            T obj = JsonService.CreateFromJson<T>(json);
            return obj;
        }
    }
}
