using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Taskorate.Models;
using Taskorate.Models.Messages;

namespace Taskorate.Functions.Functions.TaskLists.Tasks
{
  public static class GetSingleTaskList
  {
    // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp
    [FunctionName(nameof(GetSingleTaskList))]
    public static void Run(
        [SignalRTrigger(Constants.SignalRTasksHubName, "messages", GetSingleTaskListMessage.MethodName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] InvocationContext invocationContext,
        [SignalRParameter] GetSingleTaskListMessage argument,
        [CosmosDB(Constants.CosmosDbDatabase, Constants.CosmosDbTasksCollection, Id = "{argument.Id}", PartitionKey = "{argument.Id}", ConnectionStringSetting = Constants.CosmosDbConnectionStringSetting)] QuickTaskList taskList,
        [SignalR(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] IAsyncCollector<SignalRMessage> signalRMessages,
        [SignalR(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
        ILogger log)
    {


      if (taskList == null)
      {
        log.LogWarning($"Could not find tasklist: {argument.Id}");
        // TODO: Send message when no tasklist is found
        return;
      }

        signalRMessages.AddAsync(new SignalRMessage
        {
          ConnectionId = invocationContext.ConnectionId,
          Target = GotSingleTaskListMessage.MethodName,
          Arguments = new [] { new GotSingleTaskListMessage(taskList) }
        });

        log.LogInformation($"User {invocationContext.UserId} is added to group. ");

        signalRGroupActions.AddAsync(new SignalRGroupAction
        {
          UserId = invocationContext.UserId,
          GroupName = taskList.Id,
          Action = GroupAction.Add
        });
    

      // Console.WriteLine("Found one!");

      // // var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
      // // signalRGroupActions.AddAsync(new SignalRGroupAction
      // // {
      // //     UserId = userIdClaim.Value,
      // //     GroupName = id,
      // //     Action = GroupAction.Add
      // // });


      // return new OkObjectResult(taskList);

    }
  }
}
