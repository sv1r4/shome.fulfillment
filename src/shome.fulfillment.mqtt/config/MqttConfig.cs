namespace shome.fulfillment.mqtt.config
{
    public class MqttConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Tls { get; set; }
    }
}
