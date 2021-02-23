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
using Taskorate.Models.Messages;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Taskorate.Functions.Functions.TaskLists.Tasks
{
  class AddTask
  {
    // HTTP routes
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

    [FunctionName(nameof(AddTask))]
    public static async Task Run(
      [SignalRTrigger(Constants.SignalRTasksHubName, "messages", AddTaskMessage.MethodName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] InvocationContext invocationContext,
      [SignalRParameter] AddTaskMessage argument,
      [CosmosDB(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, ConnectionStringSetting = Constants.CosmosDbConnectionStringSetting)] DocumentClient documentClient,
      [SignalR(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] IAsyncCollector<SignalRMessage> signalRMessages,
      ILogger log)
    {
      var task = argument.QuickTask;
      var taskListId = argument.TaskListId;

      // Assign id to new task
      task.Id = IdentifierGenerator.Generate();

      log.LogInformation($"New task ({task.Name}) created by connection: {invocationContext.ConnectionId}");

      // Send new task to everyone else in the group
      await signalRMessages.AddAsync(new SignalRMessage
      {
        GroupName = taskListId,
        Target = GotNewTaskMessage.MethodName,
        Arguments = new object[] { new GotNewTaskMessage(task) },
        Except = new[] { invocationContext.ConnectionId }
      });

      // Get entire task list
      var uri = UriFactory.CreateDocumentUri(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, taskListId.ToString());
      var requestOptions = new RequestOptions { PartitionKey = new PartitionKey(taskListId.ToString()), JsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() } };
      var taskList = await documentClient.ReadDocumentAsync<QuickTaskList>(uri, requestOptions);

      // Add task to task list
      taskList.Document.Tasks ??= new List<QuickTask>();
      taskList.Document.Tasks.Add(task);

      await documentClient.ReplaceDocumentAsync(uri, taskList.Document, requestOptions);
    }
  }
}
