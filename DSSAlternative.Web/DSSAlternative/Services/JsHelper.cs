using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace DSSAlternative.Web.Services
{
    public static class JsHelper
    {
        public static async Task SaveAs(IJSRuntime jsRuntime, string fileName, string content)
        {
            try
            {
                await jsRuntime.InvokeAsync<object>("saveAsFile", fileName, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
