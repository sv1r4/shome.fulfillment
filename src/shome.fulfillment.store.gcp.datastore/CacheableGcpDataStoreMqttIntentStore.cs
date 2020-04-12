using System.Collections.Concurrent;
using System.Collections.Generic;
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

         
        public async Task<MqttIntent> FindAsync(string intentName)
        {
            if (_cache.ContainsKey(intentName))
            {
                return _cache[intentName];
            }

            var intent = await _gcpStore.FindAsync(intentName);
            _cache.TryAdd(intentName, intent);
            return intent;

        }

        public async Task<IReadOnlyList<MqttIntent>> GetAllAsync()
        {
            //todo refactor cacheable re init
            var intents = await _gcpStore.GetAllAsync();
            foreach (var mqttIntent in intents)
            {
                _cache.TryAdd(mqttIntent.IntentName, mqttIntent);
            }

            return intents;
        }
    }
}
