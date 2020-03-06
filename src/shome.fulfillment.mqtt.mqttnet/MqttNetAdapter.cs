using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using shome.fulfillment.mqtt.config;

namespace shome.fulfillment.mqtt.mqttnet
{
    public class MqttNetAdapter : IMqttPublisher
    {
        private readonly IMqttClient _mqtt;
        private readonly MqttConfig _mqttConfig;

        public MqttNetAdapter(IMqttFactory mqttFactory, IOptions<MqttConfig> mqttConfig)
        {
            _mqtt = mqttFactory.CreateMqttClient();
            _mqttConfig = mqttConfig.Value;
        }

        public async Task<bool> ConnectMqttAsync()
        {
            var connectResult = await _mqtt.ConnectAsync(GetConnectOptions());
            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                return false;
            }

            return true;
        }

        public async Task PublishAsync(string topic, string message)
        {
            if (!_mqtt.IsConnected)
            {
                await ConnectMqttAsync();
            }

            await _mqtt.PublishAsync(topic, Encoding.UTF8.GetBytes(message));
            
        }

        private IMqttClientOptions GetConnectOptions()
        {
            var mqttOptionsBuilder = new MqttClientOptionsBuilder()
                .WithCleanSession()
                .WithClientId(Guid.NewGuid().ToString());
            if (!string.IsNullOrWhiteSpace(_mqttConfig.User))
            {
                mqttOptionsBuilder = mqttOptionsBuilder.WithCredentials(_mqttConfig.User, _mqttConfig.Password);
            }
            return mqttOptionsBuilder.WithTcpServer(_mqttConfig.Host, _mqttConfig.Port)
                .WithTls(tlsParameters =>
                {
                    tlsParameters.UseTls = _mqttConfig.Tls;
                }).Build();
        }


        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            Console.WriteLine("disposoing");
            try
            {
                if (_mqtt != null)
                {
                    await _mqtt.DisconnectAsync();
                }
            }
            finally
            {
                _mqtt?.Dispose();
            }
        }
    }
}
