using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Cloud.Datastore.V1;
using Microsoft.Extensions.Options;
using shome.fulfillment.store.gcp.datastore.config;
using shome.fulfillment.store.gcp.datastore.util;

namespace shome.fulfillment.store.gcp.datastore
{
    public class GcpDatastoreMqttIntentStore:IMqttIntentStore
    {
        private readonly DatastoreDb _db;
        private readonly KeyFactory _keyFactory;
        private readonly DataStoreKind _kind;
        private readonly IMapper _mapper;

        public GcpDatastoreMqttIntentStore(IOptions<GcpDatastoreConfig> config, DataStoreKind kind, IMapper mapper)
        {
            _kind = kind;
            _mapper = mapper;
            _db = DatastoreDb.Create(config.Value.ProjectId, client: string.IsNullOrWhiteSpace(config.Value.KeyJson)
                ? null
                : new DatastoreClientBuilder
                {
                    JsonCredentials = config.Value.KeyJson
                }.Build());
            _keyFactory = _db.CreateKeyFactory(kind.MqttIntentKind);
        }

        
        public async Task<MqttIntent> FindAsync(string intentName)
        {
            var q = new Query(_kind.MqttIntentKind)
            {
                Filter = Filter.Equal("__key__", _keyFactory.CreateKey(intentName))
            };
            var queryResult = await _db.RunQueryAsync(q);
            var entity = queryResult.Entities.FirstOrDefault();
            return entity == null ? null : _mapper.Map<Entity, MqttIntent>(entity);
        }
    }
}
