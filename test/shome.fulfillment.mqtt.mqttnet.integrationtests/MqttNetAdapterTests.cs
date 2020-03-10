using System;
using System.Threading.Tasks;
using Xunit;

namespace shome.fulfillment.mqtt.mqttnet.integrationtests
{
    public class MqttNetAdapterTests:IClassFixture<MqttFixture>, IDisposable
    {
        private readonly IMqttPublisher _mqtt;

        public MqttNetAdapterTests(MqttFixture fixture)
        {
            _mqtt = fixture.MqttPublisherFactory();
        }

        [Fact]
        public async Task ValidConfig_ConnectAsync_ReturnedTrue()
        {
            Assert.True(await _mqtt.ConnectMqttAsync());
        }

        [Fact]
        public async Task Connected_PublishAsync_NoException()
        {
            await _mqtt.ConnectMqttAsync();
            await _mqtt.PublishAsync("test", "test");
            Assert.True(true);
        }

        [Fact]
        public async Task NotConnected_PublishAsync_NoException()
        {
            await _mqtt.PublishAsync("test", "test");
            Assert.True(true);
        }

        public void Dispose()
        {
            _mqtt?.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}
