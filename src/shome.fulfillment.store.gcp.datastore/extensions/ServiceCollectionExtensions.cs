using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shome.fulfillment.store.gcp.datastore.config;
using shome.fulfillment.store.gcp.datastore.mapping;
using shome.fulfillment.store.gcp.datastore.util;

namespace shome.fulfillment.store.gcp.datastore.extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGcpDatastore(this IServiceCollection services)
        {
            services.AddScoped<IMqttIntentStore, GcpDatastoreMqttIntentStore>();
            
            services.AddAutoMapper(Assembly.GetAssembly(typeof(MqttIntentEntityMapperProfile)));
            services.AddSingleton(new DataStoreKind(nameof(MqttIntent)));
            services.AddOptions<GcpDatastoreConfig>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(nameof(GcpDatastoreConfig)).Bind(settings);
            });
            return services;
        }
    }
}
