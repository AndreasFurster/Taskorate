using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using QuickTask.Models;
using QuickTask = QuickTask.Models.QuickTaskModel;

namespace QuickTask.Functions
{
    public static class TasklistModifiedFunction
    {
        //[FunctionName("TasklistModifiedFunction")]
        //public static void HandleTaskDbChange([CosmosDBTrigger(
        //    databaseName: "quicktaskDb",
        //    collectionName: "tasks",
        //    ConnectionStringSetting = "CosmosDBConnectionString",
        //    LeaseCollectionName = "leases", 
        //    CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
        //    [SignalR(HubName = "tasks", ConnectionStringSetting = "Azure__SignalR__ConnectionString")] IAsyncCollector<SignalRMessage> signalRMessages, 
        //    ILogger log)
        //{
        //    if (input != null && input.Count > 0)
        //    {
        //        log.LogInformation("Documents modified " + input.Count);
        //        log.LogInformation("First document Id " + input[0].Id);
        //    }
        //}
    }
}
