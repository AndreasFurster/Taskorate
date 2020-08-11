using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using QuickTask.Models;

namespace QuickTask.Functions.Functions.TaskLists.Tasks
{
    class Create
    {
        [FunctionName("addTask")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName = "tasks", ConnectionStringSetting = "Azure__SignalR__ConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            [CosmosDB("quicktaskDb", "tasks", ConnectionStringSetting = "CosmosDBConnectionString")] IAsyncCollector<QuickTaskModel> outTaskDocuments,
            ILogger log)
        {
            var json = await req.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<QuickTaskModel>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            task.Guid = Guid.NewGuid();
            log.LogInformation($"New task added: {task.Name}, {json}");

            // Send new task to everyone
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = "newTask",
                Arguments = new object[] { task }
            });

            // Add the new text to CosmosDB
            await outTaskDocuments.AddAsync(task);

            return new OkObjectResult(task);
        }
    }
}
