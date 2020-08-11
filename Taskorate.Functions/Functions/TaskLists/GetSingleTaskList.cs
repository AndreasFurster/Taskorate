using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Taskorate.Models;

namespace Taskorate.Functions.Functions.TaskLists.Tasks
{
    public static class GetSingleTaskList
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp
        [FunctionName(nameof(GetSingleTaskList))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "task-lists/{id:guid}")] HttpRequest req,
            [CosmosDB("TaskorateDb","tasks", Id = "{id}", PartitionKey = "{id}")] QuickTaskList taskList,
            ILogger log)
        {
            if(taskList == null) return new NotFoundResult();

            return new OkObjectResult(taskList);
        }
    }
}
