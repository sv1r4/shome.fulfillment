using System.Collections.Generic;

namespace shome.fulfillment.store
{
    public class MqttIntent
    {
        public string IntentName { get; set; }
        public string Topic { get; set; }

        public string Message { get; set; }
        public IList<ParamMap> ParamMapMqtt { get; set; }

        public IList<ParamMap> ParamMapText { get; set; }

    }


    public class ParamMap
    {
        public string Param { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

}
