using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using shome.fulfillment.azure.function.wakeup;

namespace shome.fulfillment.azure.function
{
    public class FulfillmentFunction
    {
        private readonly IWebHookHandler _handler;
        private readonly IWakeupHandler _wakeupHandler;

        public FulfillmentFunction(IWebHookHandler handler, IWakeupHandler wakeupHandler)
        {
            _handler = handler;
            _wakeupHandler = wakeupHandler;
        }

        [FunctionName("fulfillment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //todo refactor wakeup hack
            if (req.Method.Equals("get", StringComparison.OrdinalIgnoreCase))
            {
                await _wakeupHandler.HandleAsync();
                return new OkResult();
            }
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
