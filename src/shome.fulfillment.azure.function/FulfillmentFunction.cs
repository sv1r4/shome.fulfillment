using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
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
        
        private readonly IWarmupHandler _warmupHandler;


        public FulfillmentFunction(IWebHookHandler handler, IWarmupHandler warmupHandler)
        {
            _handler = handler;
            _warmupHandler = warmupHandler;
        }

        [FunctionName("fulfillment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            //if get warmup
            //if post handle dialogflow request
            if (string.Equals(req.Method, "get", StringComparison.OrdinalIgnoreCase))
            {
                return await HandleWarmup(log);
            }
            return await HandleHook(req, log);
        }

        private async Task<IActionResult> HandleHook(HttpRequest req, ILogger log)
        {
            try
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
            catch (Exception ex)
            {
                log.LogError(ex, "Error handle request {function}", nameof(FulfillmentFunction));
                return new ExceptionResult(ex, true);
            }
        }

        private async Task<IActionResult> HandleWarmup(ILogger log)
        {
            try
            {
                await _warmupHandler.HandleAsync();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error handle request {function}", nameof(FulfillmentFunction));
                return new ExceptionResult(ex, true);
            }

            return new OkResult();
        }
    }
}
