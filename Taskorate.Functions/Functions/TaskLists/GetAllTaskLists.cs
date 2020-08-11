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
    public static class GetAllTaskLists
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp
        [FunctionName(nameof(GetAllTaskLists))]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Admin, "get", Route = "task-lists")] HttpRequest req,
            [CosmosDB("TaskorateDb","tasks", ConnectionStringSetting = "CosmosDB")]
            IEnumerable<QuickTaskList> taskLists,
            ILogger log)
        {

            return new OkObjectResult(taskLists);
        }
    }
}
