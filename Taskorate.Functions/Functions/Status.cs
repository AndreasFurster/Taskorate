using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Taskorate.Functions.Functions
{
    public static class Status
    {
        [FunctionName("status")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            [CosmosDB("TaskorateDb", "tasks", ConnectionStringSetting = "CosmosDBConnectionString")] DocumentClient documentClient,
            ILogger log)
        {
            var result = new StatusResult();
            
            // Validate connection strings present
            //var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDB");
            //result.CosmosDbConnectionStringPresent = !string.IsNullOrEmpty(cosmosDbConnectionString);

            var signalRConnectionString = Environment.GetEnvironmentVariable("AzureSignalRConnectionString");
            result.SignalRConnectionStringPresent = !string.IsNullOrEmpty(signalRConnectionString);

            var databaseUri = UriFactory.CreateDatabaseUri("TaskorateDb");
            var database = await documentClient.ReadDatabaseAsync(databaseUri);
            result.CosmosDbConnectionSuccess = database?.Resource != null;

            return new OkObjectResult(result);
        }

        private class StatusResult
        {
            public bool? CosmosDbConnectionStringPresent { get; set; }
            public bool? CosmosDbConnectionSuccess { get; set; }
            public bool? SignalRConnectionStringPresent { get; set; }
            public bool? SignalRConnectionSuccess { get; set; }
        }
    }
}
