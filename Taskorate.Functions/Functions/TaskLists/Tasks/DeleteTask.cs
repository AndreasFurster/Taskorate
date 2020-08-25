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
using Taskorate.Models.Messages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Taskorate.Functions.Functions.TaskLists.Tasks
{
    class DeleteTask
    {
        // HTTP routes
        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

        [FunctionName(nameof(DeleteTask))]
        public static async Task Run(
            [SignalRTrigger(Constants.SignalRTasksHubName, "messages", DeleteTaskMessage.MethodName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] InvocationContext invocationContext,
            [SignalRParameter] DeleteTaskMessage argument,
            
            [CosmosDB(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, ConnectionStringSetting = Constants.CosmosDbConnectionStringSetting)] DocumentClient documentClient,
            [SignalR(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var taskListId = argument.TaskListId;
            var taskId = argument.QuickTaskId;

            // Send new task to everyone
            await signalRMessages.AddAsync(new SignalRMessage
            {
              GroupName = taskListId,
              Target = GotDeletedTaskMessage.MethodName,
              Arguments = new object[] { new GotDeletedTaskMessage(taskId) { IgnoreConnectionId = invocationContext.ConnectionId }}
            });

            // Get entire task list
            var uri = UriFactory.CreateDocumentUri(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, taskListId.ToString());
            var requestOptions = new RequestOptions{ PartitionKey = new PartitionKey(taskListId.ToString()), JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() } };
            var taskList = await documentClient.ReadDocumentAsync<QuickTaskList>(uri, requestOptions);
            
            // // Add task to task list
            // TODO: handle error
            // if (taskList.Document == null) return new NotFoundResult();
            // if (taskList.Document.Tasks == null) return new BadRequestObjectResult("No tasks found for this list.");

            var index = taskList.Document.Tasks.FindIndex(t => t.Id == taskId);
            // TODO: handle error
            // if (index == -1) return new BadRequestObjectResult($"Task with id {taskId} not found on this list.");

            taskList.Document.Tasks.RemoveAt(index);

            await documentClient.ReplaceDocumentAsync(uri, taskList.Document, requestOptions);
        }
    }
}
