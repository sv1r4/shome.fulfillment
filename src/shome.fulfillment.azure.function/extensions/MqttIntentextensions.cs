using System.Collections.Generic;
using System.Linq;
using Google.Apis.Dialogflow.v2.Data;
using Google.Apis.Download;
using shome.fulfillment.store;

namespace shome.fulfillment.web.extensions
{
    public static class MqttIntentExtensions
    {
        public static string TranslateMqttMessage(this MqttIntent mqttIntent, IDictionary<string, object> queryResultParameters)
        {
            return TranslateString(mqttIntent.Message, mqttIntent.ParamMapMqtt, queryResultParameters);
        }

       
        public static string TranslateResponseMessage(this MqttIntent mqttIntent, GoogleCloudDialogflowV2QueryResult queryResult)
        {
            return TranslateString(queryResult.FulfillmentMessages.FirstOrDefault()?.Text?.Text?.FirstOrDefault(), mqttIntent.ParamMapText, queryResult.Parameters);
        }

        private static string TranslateString(string message, IList<ParamMap> paramMap, IDictionary<string, object> queryResultParameters)
        {
            if (queryResultParameters?.Any() != true)
            {
                return message;
            }

            
            foreach (var queryParam in queryResultParameters)
            {
                var param = queryParam.Key;
                var from = queryParam.Value.ToString();
                var to = paramMap
                    .Where(x => x.Param == param && x.From == from)
                    .Select(x => x.To)
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(to))
                {
                    message = message.Replace($"@{param}", to);
                }
                else
                {
                    message = message.Replace($"@{param}", queryParam.Value.ToString());
                }
                
            }

            return message;

        }

    }
}
