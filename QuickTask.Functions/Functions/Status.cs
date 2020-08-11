using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace QuickTask.Functions.Functions
{
    public static class Status
    {
        [FunctionName("status")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            ILogger log)
        {
            // TODO: Validate connection with CosmosDB & SignalR Service
            return new OkObjectResult("Ok!");
        }
    }
}
