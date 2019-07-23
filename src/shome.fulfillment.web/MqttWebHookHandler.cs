using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2.Data;
using Microsoft.Extensions.Logging;
using shome.fulfillment.mqtt;
using shome.fulfillment.store;
using shome.fulfillment.web.extensions;

namespace shome.fulfillment.web
{
    public class MqttWebHookHandler : IWebHookHandler
    {
        private readonly ILogger _logger;
        private readonly IMqttPublisher _mqtt;
        private readonly IMqttIntentStore _intentStore;

        public MqttWebHookHandler(ILogger<MqttWebHookHandler> logger, IMqttPublisher mqtt, IMqttIntentStore intentStore)
        {
            _logger = logger;
            _mqtt = mqtt;
            _intentStore = intentStore;
        }

        public async Task<GoogleCloudDialogflowV2WebhookResponse> HandleAsync(GoogleCloudDialogflowV2WebhookRequest request)
        {
            var intentName = request?.QueryResult?.Intent?.DisplayName;
            _logger.LogDebug("{IntentName}", intentName);
            if (string.IsNullOrWhiteSpace(intentName))
            {
                _logger.LogWarning("No intent name in request");
                
                return new GoogleCloudDialogflowV2WebhookResponse
                {
                    FulfillmentText = request?.QueryResult?.FulfillmentText ?? string.Empty
                };
            }

            var mqttIntent = await _intentStore.FindAsync(intentName);
            if (mqttIntent == null)
            {
                _logger.LogWarning("No Mqtt Intent found in store for {name}", intentName);
                
                return new GoogleCloudDialogflowV2WebhookResponse
                {
                    FulfillmentText = request.QueryResult?.FulfillmentText ?? string.Empty
                };
            }

            await _mqtt.PublishAsync(mqttIntent.Topic, mqttIntent.TranslateMqttMessage(request.QueryResult.Parameters));

            var response = new GoogleCloudDialogflowV2WebhookResponse
            {
                FulfillmentText = mqttIntent.TranslateResponseMessage(request.QueryResult)
            };

            

            return response;
        }

       
    }
}
