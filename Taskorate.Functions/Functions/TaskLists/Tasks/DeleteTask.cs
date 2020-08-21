using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
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
    class DeleteTask
    {
        // HTTP routes
        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

        [FunctionName(nameof(DeleteTask))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "task-lists/{taskListId:guid}/tasks/{taskId:guid}")] HttpRequest req,
            Guid taskListId,
            Guid taskId,
            [CosmosDB("TaskorateDb","tasks", ConnectionStringSetting = "CosmosDB")] DocumentClient documentClient,
            [SignalR(HubName = "tasks", ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            // Parse body
            // var json = await req.ReadAsStringAsync();
            // var task = JsonSerializer.Deserialize<QuickTask>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Send new task to everyone
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = $"{taskListId}/deletedTask",
                Arguments = new object[] { taskId }
            });

            // Get entire task list
            var uri = UriFactory.CreateDocumentUri("TaskorateDb", "tasks", taskListId.ToString());
            var requestOptions = new RequestOptions{ PartitionKey = new PartitionKey(taskListId.ToString()), JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() } };
            var taskList = await documentClient.ReadDocumentAsync<QuickTaskList>(uri, requestOptions);
            
            // Add task to task list
            if (taskList.Document == null) return new NotFoundResult();
            if (taskList.Document.Tasks == null) return new BadRequestObjectResult("No tasks found for this list.");

            var index = taskList.Document.Tasks.FindIndex(t => t.Id == taskId);
            if (index == -1) return new BadRequestObjectResult($"Task with id {taskId} not found on this list.");

            taskList.Document.Tasks.RemoveAt(index);

            await documentClient.ReplaceDocumentAsync(uri, taskList.Document, requestOptions);

            return new OkResult();
        }
    }
}
