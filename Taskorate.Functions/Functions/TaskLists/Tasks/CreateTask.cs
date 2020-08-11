using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Taskorate.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Taskorate.Functions.Functions.TaskLists.Tasks
{
    class CreateTask
    {
        // HTTP routes
        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

        [FunctionName(nameof(CreateTask))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task-lists/{taskListId:guid}/tasks")] HttpRequest req,
            Guid taskListId,
            [CosmosDB("TaskorateDb","tasks", ConnectionStringSetting = "CosmosDB")] DocumentClient documentClient,
            [SignalR(HubName = "tasks", ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            // Parse body
            var json = await req.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<QuickTask>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Assign id to new task
            task.Id = Guid.NewGuid();

            // Send new task to everyone
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = $"{taskListId}/newTask",
                Arguments = new object[] { task }
            });

            // Get entire task list
            var uri = UriFactory.CreateDocumentUri("TaskorateDb", "tasks", taskListId.ToString());
            var requestOptions = new RequestOptions{ PartitionKey = new PartitionKey(taskListId.ToString()), JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() } };
            var taskList = await documentClient.ReadDocumentAsync<QuickTaskList>(uri, requestOptions);
            
            // Add task to task list
            taskList.Document.Tasks ??= new List<QuickTask>();
            taskList.Document.Tasks.Add(task);

            await documentClient.ReplaceDocumentAsync(uri, taskList.Document, requestOptions);

            return new OkObjectResult(task);
        }
    }
}
