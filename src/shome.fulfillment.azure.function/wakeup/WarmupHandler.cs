using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shome.fulfillment.mqtt;
using shome.fulfillment.store;

namespace shome.fulfillment.azure.function.wakeup
{
    public class WarmupHandler : IWarmupHandler
    {
        private readonly ILogger _logger;
        private readonly IMqttPublisher _mqtt;
        private readonly IMqttIntentStore _mqttIntentStore;

        public WarmupHandler(IMqttPublisher mqtt, ILogger<WarmupHandler> logger, IMqttIntentStore mqttIntentStore)
        {
            _mqtt = mqtt;
            _logger = logger;
            _mqttIntentStore = mqttIntentStore;
        }


        public async Task HandleAsync()
        {
            var getIntentsTask = _mqttIntentStore.GetAllAsync();
            var mqttConnectTask = _mqtt.ConnectMqttAsync();

            await getIntentsTask;
            await mqttConnectTask;
            _logger.LogDebug("Warmed up");
        }
    }
}
