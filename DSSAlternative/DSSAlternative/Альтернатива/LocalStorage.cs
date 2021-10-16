using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSSAlternative.AppComponents
{
    public class LocalStorage
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
