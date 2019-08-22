using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace shome.fulfillment.web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IWebHookHandler _handler;

        public FulfillmentController(ILogger<FulfillmentController> logger, IWebHookHandler handler)
        {
            _logger = logger;
            _handler = handler;
        }

        [HttpPost]
        public async Task Post()
        {
            using (var sr = new StreamReader(Request.Body))
            {
                var json = await sr.ReadToEndAsync();
                _logger.LogDebug("Got {request}", json);
                var hookResponse = await _handler.HandleAsync(json);
                
                Response.ContentType = "application/json; charset=utf-8";
                await Response.WriteAsync(JsonConvert.SerializeObject(hookResponse));

            }
        }

    }
}