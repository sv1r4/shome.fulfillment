using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Dialogflow.v2.Data;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public async Task<GoogleCloudDialogflowV2WebhookResponse> HandleAsync(string json)
        {
            //GoogleCloudDialogflowV2WebhookRequest does not contain endInteraction field 
            //this is why raw parsing needed =(
            dynamic rawJson = JObject.Parse(json);
            var isEndInteraction = rawJson?.queryResult?.intent?.endInteraction == true;


            var request = JsonConvert.DeserializeObject<GoogleCloudDialogflowV2WebhookRequest>(json);
           
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
            
            var payload = new Dictionary<string, object>
            {
                {
                    "google", new
                    {
                        expectUserResponse = !isEndInteraction
                    }
                }
            };
            var response = new GoogleCloudDialogflowV2WebhookResponse
            {
                FulfillmentText = mqttIntent.TranslateResponseMessage(request.QueryResult),
                Payload = payload
            };

            

            return response;
        }

       
    }
}
