using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using DSSAlternative.AHP;
using DSSAlternative.Web;
using DSSAlternative.Web.AppComponents;
using DSSAlternative.AHP.Ratings;
using DSSAlternative.AHP.Logs;
using DSSAlternative.Web.Services;

namespace DSSAlternative
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<ILogger, LoggerService>();
            builder.Services.AddScoped<IDssJson, DssJson>();

            builder.Services.AddScoped<IDssProjects, DssProjects>();
            builder.Services.AddScoped<IDssTemplates, DssTemplates>();
            builder.Services.AddScoped<IRatingSystem, RatingSystem>();

            builder.Services.AddScoped<IClipboard, Clipboard>();
            builder.Services.AddScoped<IAccount, Account>();
            builder.Services.AddScoped<ILocalStorage, LocalStorage>();

            await builder.Build().RunAsync();
        }
    }
}
