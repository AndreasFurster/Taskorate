using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Taskorate.Functions.Functions
{
  public static class SignalRNegotiate
  {
    [FunctionName("negotiate")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        IBinder binder)
    // [SignalRConnectionInfo(HubName = Constants.SignalRTasksHubName, ConnectionStringSetting = Constants.SignalRConnectionStringSetting)] SignalRConnectionInfo connectionInfo)
    {

      SignalRConnectionInfoAttribute attribute = new SignalRConnectionInfoAttribute
      {
        HubName = Constants.SignalRTasksHubName,
        ConnectionStringSetting = Constants.SignalRConnectionStringSetting,
        // Use random user id for anonymous usage
        UserId = $"userid:{Guid.NewGuid().ToString()}"
        // UserId = "myUserId"
      };


      // This style is an example of imperative attribute binding; the mechanism for declarative binding described below does not work
      // UserId = "{headers.x-my-custom-header}" https://docs.microsoft.com/en-us/azure/azure-signalr/signalr-concept-serverless-development-config
      SignalRConnectionInfo connectionInfo = await binder.BindAsync<SignalRConnectionInfo>(attribute);

      return new OkObjectResult(connectionInfo);
    }
  }
}
