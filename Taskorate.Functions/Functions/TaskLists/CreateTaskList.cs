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
using Taskorate.Functions.Functions.TaskLists.Tasks;
using Taskorate.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Taskorate.Functions.Functions.TaskLists
{
    class CreateTaskList
    {
        // HTTP routes
        // https://docs.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2#route-constraints

        [FunctionName(nameof(CreateTaskList))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "task-lists")] HttpRequest req,
            [CosmosDB("TaskorateDb", "tasks", ConnectionStringSetting = Constants.CosmosDbConnectionStringSetting)] DocumentClient documentClient,
            ILogger log)
        {
            // Parse body
            var json = await req.ReadAsStringAsync();
            var taskList = JsonSerializer.Deserialize<QuickTaskList>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            taskList.Id = IdentifierGenerator.Generate();
            taskList.Tasks = new List<QuickTask>();

            var uri = UriFactory.CreateDocumentCollectionUri("TaskorateDb", "tasks");
            var requestOptions = new RequestOptions{JsonSerializerSettings = new JsonSerializerSettings{ContractResolver = new CamelCasePropertyNamesContractResolver()}};
            await documentClient.CreateDocumentAsync(uri, taskList, requestOptions);
            
            return new OkObjectResult(taskList);
        }
    }
}
