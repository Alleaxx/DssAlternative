using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using DSSAlternative.AppComponents;
using DSSAlternative.Services;

namespace DSSAlternative
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<DSS>();
            builder.Services.AddScoped<Clipboard>();
            builder.Services.AddScoped<Account>();
            builder.Services.AddScoped<LocalStorage>();

            await builder.Build().RunAsync();
        }
    }
}
