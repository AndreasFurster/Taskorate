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
  class UpdateTask
  {
    // HTTP routes
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

    [FunctionName(nameof(UpdateTask))]
    public static async Task<IActionResult> Run(
        [SignalRTrigger(Constants.SignalRTasksHubName, "messages", UpdateTaskMessage.MethodName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] InvocationContext invocationContext,
        [SignalRParameter] UpdateTaskMessage argument,

        [CosmosDB(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, ConnectionStringSetting = Constants.CosmosDbConnectionStringSetting)] DocumentClient documentClient,
        [SignalR(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] IAsyncCollector<SignalRMessage> signalRMessages,
        ILogger log)
    {
      // Parse body
      var taskListId = argument.TaskListId;
      var task = argument.QuickTask;

      // Send new task to everyone in group
      await signalRMessages.AddAsync(new SignalRMessage
      {
        GroupName = taskListId,
        Target = GotUpdatedTaskMessage.MethodName,
        Arguments = new object[] { new GotUpdatedTaskMessage(task) { IgnoreConnectionId = invocationContext.ConnectionId }}
      });

      // Get entire task list
      var uri = UriFactory.CreateDocumentUri("TaskorateDb", "tasks", taskListId.ToString());
      var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(taskListId.ToString()), JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() } };
      var taskList = await documentClient.ReadDocumentAsync<QuickTaskList>(uri, requestOptions);

      // Add task to task list
      if (taskList.Document == null) return new NotFoundResult();
      if (taskList.Document.Tasks == null) return new BadRequestObjectResult("No tasks found for this list.");

      var index = taskList.Document.Tasks.FindIndex(t => t.Id == task.Id);
      if (index == -1) return new BadRequestObjectResult($"Task with id {task.Id} not found on this list.");

      taskList.Document.Tasks[index] = task;

      await documentClient.ReplaceDocumentAsync(uri, taskList.Document, requestOptions);

      return new OkObjectResult(task);
    }
  }
}
