using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using shome.fulfillment.mqtt.mqttnet.extensions;
using shome.fulfillment.store.gcp.datastore.extensions;

[assembly: FunctionsStartup(typeof(shome.fulfillment.azure.function.Startup))]
namespace shome.fulfillment.azure.function
{
    public class Startup:FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddScoped<IWebHookHandler, MqttWebHookHandler>()
                .AddMqttNetPublisher(ServiceLifetime.Singleton)
                .AddCacheableGcpDatastore()
                ;
        }
    }
}
