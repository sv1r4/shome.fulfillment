namespace shome.fulfillment.store.gcp.datastore.util
{
    public class DataStoreKind
    {
        public DataStoreKind(string mqttIntentKind)
        {
            MqttIntentKind = mqttIntentKind;
        }

        public string MqttIntentKind { get;  }
    }
}
