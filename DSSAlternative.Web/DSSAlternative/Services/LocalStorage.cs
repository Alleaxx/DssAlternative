using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.Web.Services
{
    /// <summary>
    /// Интерфейс сервиса, работающего с LocalStorage браузера
    /// </summary>
    public interface ILocalStorageService
    {
        /// <summary>
        /// Получить значение из хранилища по ключу
        /// </summary>
        ValueTask<string> GetValueAsync(string key);

        /// <summary>
        /// Установить значение из хранилища по ключу
        /// </summary>
        ValueTask SetPropAsync(string key, object item);

        /// <summary>
        /// Удалить значение из хранилища по ключу
        /// </summary>
        ValueTask RemovePropAsync(string key);

        /// <summary>
        /// Очистить всё хранилище
        /// </summary>
        ValueTask ClearAll();
    }
    
    /// <summary>
    /// Cервиса, работающий с LocalStorage браузера
    /// </summary>
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime jSRuntime;

        public LocalStorageService(IJSRuntime runtime)
        {
            jSRuntime = runtime;
        }

        public ValueTask<string> GetValueAsync(string key)
        {
            return jSRuntime.InvokeAsync<string>($"localStorage.getItem", key);
        }
        public ValueTask SetPropAsync(string key, object item)
        {
            return jSRuntime.InvokeVoidAsync($"localStorage.setItem", key, item);
        }
        public ValueTask RemovePropAsync(string key)
        {
            return jSRuntime.InvokeVoidAsync($"localStorage.removeItem", key);
        }
        public ValueTask ClearAll()
        {
            return jSRuntime.InvokeVoidAsync($"localStorage.clear");
        }
    }
}
