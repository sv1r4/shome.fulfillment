﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using shome.fulfillment.mqtt.config;

namespace shome.fulfillment.mqtt.mqttnet.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttNetPublisher(this IServiceCollection services, ServiceLifetime mqttLifetime = ServiceLifetime.Scoped)
        {
            services.Add(new ServiceDescriptor(typeof(IMqttPublisher), typeof(MqttNetAdapter), mqttLifetime));
            services.AddSingleton<IMqttFactory>(new MqttFactory());
            services.AddOptions<MqttConfig>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(MqttConfig)).Bind(settings);
            });

            return services;
        }
    }
}
