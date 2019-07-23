using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shome.fulfillment.mqtt.mqttnet.extensions;

namespace shome.fulfillment.mqtt.mqttnet.integrationtests
{
    public class MqttFixture
    {
        public Func<IMqttPublisher> MqttPublisherFactory { get; }

        public MqttFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var config = builder.Build();
           

            var services = new ServiceCollection();
            services.AddMqttNetPublisher(config);
            services.AddLogging();

            var sp = services.BuildServiceProvider();

            MqttPublisherFactory = () => sp.GetRequiredService<IMqttPublisher>();
        }
    }
}
