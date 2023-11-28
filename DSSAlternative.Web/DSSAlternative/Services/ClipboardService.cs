using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace DSSAlternative.Web.Services
{
    /// <summary>
    /// Интерфейс сервиса по работе с буфером обмена
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// Прочитать текст из буфера обмена
        /// </summary>
        /// <returns></returns>
        ValueTask<string> ReadTextAsync();

        /// <summary>
        /// Записать текст в буфер обмена
        /// </summary>
        ValueTask WriteTextAsync(string text);
    }

    /// <summary>
    /// JS-сервис по работе с буфером обмена
    /// </summary>
    public class ClipboardService : IClipboardService
    {
        private readonly IJSRuntime jSRuntime;

        public ClipboardService(IJSRuntime runtime)
        {
            jSRuntime = runtime;
        }

        public ValueTask<string> ReadTextAsync()
        {
            return jSRuntime.InvokeAsync<string>("navigator.clipboard.readText");
        }
        public ValueTask WriteTextAsync(string text)
        {
            return jSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
