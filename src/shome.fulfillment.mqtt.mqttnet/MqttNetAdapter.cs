using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger _logger;

        public MqttNetAdapter(ILogger<MqttNetAdapter> logger, IMqttFactory mqttFactory, IOptions<MqttConfig> mqttConfig)
        {
            _logger = logger;
            _mqtt = mqttFactory.CreateMqttClient();
            _mqttConfig = mqttConfig.Value;
        }

        public async Task<bool> ConnectMqttAsync()
        {
            var connectResult = await _mqtt.ConnectAsync(GetConnectOptions());
            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                _logger.LogError("Error connect to MQTT server {{ResultCode}} {{ConnectResult}}", connectResult.ResultCode, connectResult);
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
            return new MqttClientOptionsBuilder()
                .WithCleanSession()
                .WithClientId(Guid.NewGuid().ToString())
                .WithCredentials(_mqttConfig.User, _mqttConfig.Password)
                .WithTcpServer(_mqttConfig.Host, _mqttConfig.Port)
                .WithTls(tlsParameters =>
                {
                    tlsParameters.UseTls = true;
                }).Build();
        }

        public void Dispose()
        {
            try
            {
                _mqtt?.DisconnectAsync().GetAwaiter().GetResult();
            }
            finally
            {
                _mqtt?.Dispose();
            }
            
            
        }
    }
}
