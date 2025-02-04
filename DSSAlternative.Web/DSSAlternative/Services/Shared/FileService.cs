using DSSAlternative.AHP.Logs;
using DSSAlternative.AHP.Templates;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DSSAlternative.Web.Services
{
    /// <summary>
    /// Интерфейс сервиса по работе с файлами
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Сохранить файл с указанным именем (включая расширения) и текстовым содержанием
        /// </summary>
        Task SaveFile(string fileName, string content);

        /// <summary>
        /// Сохранить файл с указанным именем (включая расширения), содержанием будет JSON-формат объекта
        /// </summary>
        Task SaveFile(string fileName, object obj);

        /// <summary>
        /// Прочитать текстовый файл и попытаться десериализовать JSON-объект указанного типа
        /// </summary>
        Task<T> TryReadFile<T>(IBrowserFile file);
    }

    /// <summary>
    /// Сервис работы с файлами по JS
    /// </summary>
    public class FileService : IFileService
    {
        private readonly IJsonService jsonService;
        private readonly IJSRuntime jsRuntime;
        private readonly ILogger logger;
        public FileService(IJSRuntime jSRuntime, ILogger logger, IJsonService jsonService)
        {
            this.jsRuntime = jSRuntime;
            this.logger = logger;
            this.jsonService = jsonService;
        }

        public async Task SaveFile(string fileName, string content)
        {
            try
            {
                await jsRuntime.InvokeAsync<object>("saveAsFile", fileName, content);
                logger.AddInfo(this, "Файл успешно сохранен", $"Записан файл с текстом длиной {content.Length}", LogCategory.Files);
            }
            catch (Exception ex)
            {
                logger.AddError(this, "Ошибка сохранения в файл", ex.Message, LogCategory.Files);
            }
        }

        public Task SaveFile(string fileName, object obj)
        {
            return SaveFile(fileName, jsonService.ConvertToJson(obj));
        }

        public async Task<T> TryReadFile<T>(IBrowserFile file)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                await file.OpenReadStream().CopyToAsync(ms);
                var bytes = ms.ToArray();
                var res = System.Text.Encoding.Default.GetString(bytes);
                logger.AddInfo(this, "Файл успешно прочитан", $"Прочитан текст длиной {res.Length}", LogCategory.Files);
                return jsonService.CreateFromJson<T>(res);
            }
            catch (Exception ex)
            {
                logger.AddError(this, "Ошибка чтения файла", ex.Message, LogCategory.Files);
            }
            finally
            {
                ms.Close();
            }
            return default;
        }
    }
}
