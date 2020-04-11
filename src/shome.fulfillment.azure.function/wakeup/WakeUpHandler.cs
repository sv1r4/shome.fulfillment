using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shome.fulfillment.mqtt;

namespace shome.fulfillment.azure.function.wakeup
{
    public class WakeUpHandler : IWakeupHandler
    {
        private readonly ILogger _logger;
        private readonly IMqttPublisher _mqtt;

        public WakeUpHandler(IMqttPublisher mqtt, ILogger<WakeUpHandler> logger)
        {
            _mqtt = mqtt;
            _logger = logger;
        }


        public async Task HandleAsync()
        {
            await _mqtt.ConnectMqttAsync();
            _logger.LogDebug("wake up request handled");
        }
    }
}
