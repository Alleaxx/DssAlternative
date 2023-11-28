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

            //Служебные
            builder.Services.AddScoped<ILogger, LoggerService>();
            builder.Services.AddScoped<IJsonService, JsonServiceDefault>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IClipboardService, ClipboardService>();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

            //DSS-сервисы
            builder.Services.AddScoped<IProjectsCollection, ProjectsCollection>();
            builder.Services.AddScoped<IRatingSystem, RatingSystem>();
            builder.Services.AddScoped<IAccount, Account>();

            await builder.Build().RunAsync();
        }
    }
}
