using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.Services
{
    public interface ILocalStorage
    {
        ValueTask<string> GetValueAsync(string key);
        ValueTask SetPropAsync(string key, object item);
        ValueTask RemovePropAsync(string key);
        ValueTask ClearAll();
    }
    public class LocalStorage : ILocalStorage
    {
        private readonly IJSRuntime jSRuntime;

        public LocalStorage(IJSRuntime runtime)
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
