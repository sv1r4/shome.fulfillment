using System;
using System.Collections.Generic;
using System.Linq;
using Google.Cloud.Datastore.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using shome.fulfillment.store.gcp.datastore.config;
using shome.fulfillment.store.gcp.datastore.extensions;
using shome.fulfillment.store.gcp.datastore.util;

namespace shome.fulfillment.store.gcp.datastore.integrationtests
{
    public class GcpDataStoreFixture:IDisposable
    {
        public DatastoreDb Db { get; }
        public KeyFactory KeyFactory { get; }
        public GcpDatastoreMqttIntentStore MqttIntentStore { get; }
        public string Kind { get; } = $"{nameof(MqttIntent)}_test";

        public IList<Key> EntityKeys { get; } = new List<Key>();
        public GcpDataStoreFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var config = builder.Build();
           

            var services = new ServiceCollection();
            services.AddGcpDatastore();
            services.AddSingleton<IConfiguration>(config);
            var toRemove = services.Single(s => s.ServiceType == typeof(DataStoreKind));

            services.Remove(toRemove);
            services.AddSingleton(new DataStoreKind(Kind));

            var sp = services.BuildServiceProvider();

            var gcpConfig = sp.GetRequiredService<IOptions<GcpDatastoreConfig>>().Value;

            Db =  DatastoreDb.Create(gcpConfig.ProjectId);
            KeyFactory = Db.CreateKeyFactory(Kind);
            MqttIntentStore = (GcpDatastoreMqttIntentStore)sp.GetRequiredService<IMqttIntentStore>();
        }

        public void Dispose()
        {
            Db.Delete(EntityKeys);
        }
    }
}
