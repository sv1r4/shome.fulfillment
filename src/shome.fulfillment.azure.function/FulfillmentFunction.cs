using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace shome.fulfillment.azure.function
{
    public class FulfillmentFunction
    {
        private readonly IWebHookHandler _handler;

        public FulfillmentFunction(IWebHookHandler handler)
        {
            _handler = handler;
        }

        [FunctionName("fulfillment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            using var sr = new StreamReader(req.Body);

            var json = await sr.ReadToEndAsync();
            log.LogDebug("Got {request}", json);
            var hookResponse = await _handler.HandleAsync(json);

            var response = new ContentResult
            {
                ContentType = "application/json; charset=utf-8",
                Content = JsonConvert.SerializeObject(hookResponse)
            };
            return response;
        }


    }
}
