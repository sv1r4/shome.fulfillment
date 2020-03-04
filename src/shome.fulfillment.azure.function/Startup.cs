using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using shome.fulfillment.mqtt.mqttnet.extensions;
using shome.fulfillment.store.gcp.datastore.extensions;

[assembly: FunctionsStartup(typeof(shome.fulfillment.azure.function.Startup))]
namespace shome.fulfillment.azure.function
{
    public class Startup:FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(new EmbeddedFileProvider(assembly:Assembly.GetExecutingAssembly()), "appsettings.json", true, false)
                .AddEnvironmentVariables()
                .Build();
            builder.Services
                .AddLogging(lb=>lb.AddConfiguration(config))
                .AddMqttNetPublisher(config)
                .AddGcpDatastore(config)
                .AddScoped<IWebHookHandler, MqttWebHookHandler>()
                .AddOptions();
        }
    }
}
