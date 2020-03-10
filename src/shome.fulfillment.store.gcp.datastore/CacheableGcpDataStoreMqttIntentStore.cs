using System.Collections.Concurrent;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using shome.fulfillment.store.gcp.datastore.config;
using shome.fulfillment.store.gcp.datastore.util;

namespace shome.fulfillment.store.gcp.datastore
{
    public class CacheableGcpDataStoreMqttIntentStore:IMqttIntentStore
    {
        private readonly GcpDatastoreMqttIntentStore _gcpStore;
        private readonly ConcurrentDictionary<string, MqttIntent> _cache = new ConcurrentDictionary<string, MqttIntent>();
        public CacheableGcpDataStoreMqttIntentStore(IOptions<GcpDatastoreConfig> config, DataStoreKind kind, IMapper mapper)
        {
            _gcpStore = new GcpDatastoreMqttIntentStore(config, kind, mapper);
        }

         
        public Task<MqttIntent> FindAsync(string intentName)
        {
            return Task.FromResult(_cache.GetOrAdd(intentName, intent => _gcpStore.FindAsync(intent).GetAwaiter().GetResult()));

        }
    }
}
