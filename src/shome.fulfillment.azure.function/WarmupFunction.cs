using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using shome.fulfillment.azure.function.wakeup;

namespace shome.fulfillment.azure.function
{
    public class WarmupFunction
    {
        private readonly IWarmupHandler _warmupHandler;

        public WarmupFunction(IWarmupHandler warmupHandler)
        {
            _warmupHandler = warmupHandler;
        }

        [FunctionName("warmup")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            await _warmupHandler.HandleAsync();
            return new OkResult();
        }
    }
}
