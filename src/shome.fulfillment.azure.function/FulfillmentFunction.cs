using System;
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
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            //return new OkObjectResult(responseMessage);


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
