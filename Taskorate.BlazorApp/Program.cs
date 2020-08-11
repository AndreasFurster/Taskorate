using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Taskorate.BlazorApp.Services;

namespace Taskorate.BlazorApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Current url: builder.HostEnvironment.BaseAddress

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            // TODO: Env variables?
            var apiUrl = builder.HostEnvironment.IsDevelopment() ? "http://localhost:7071/api/" : "https://quick-task.azurewebsites.net/api/";

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
            builder.Services.AddScoped(sp => new HubConnectionBuilder().WithUrl(apiUrl).Build());
            builder.Services.AddScoped<QuickTaskService>();
            builder.Services.AddScoped<TaskListService>();


            await builder.Build().RunAsync();
        }
    }
}
