using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace DSSAlternative.Services
{
    public interface IClipboard
    {
        ValueTask<string> ReadTextAsync();
        ValueTask WriteTextAsync(string text);
    }
    public class Clipboard : IClipboard
    {
        private readonly IJSRuntime jSRuntime;

        public Clipboard(IJSRuntime runtime)
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
