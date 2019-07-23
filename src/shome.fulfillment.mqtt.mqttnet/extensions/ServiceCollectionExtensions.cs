using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using shome.fulfillment.mqtt.config;

namespace shome.fulfillment.mqtt.mqttnet.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttNetPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<MqttConfig>(configuration.GetSection(nameof(MqttConfig)));
            services.AddScoped<IMqttPublisher, MqttNetAdapter>();
            services.AddSingleton<IMqttFactory, MqttFactory>();

            return services;
        }
    }
}
