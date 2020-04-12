using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Datastore.V1;
using Newtonsoft.Json;
using Xunit;
using Value = Google.Cloud.Datastore.V1.Value;


namespace shome.fulfillment.store.gcp.datastore.integrationtests
{
    public class GcpDatastoreMqttIntentStoreTests:IClassFixture<GcpDataStoreFixture>
    {
        private readonly GcpDataStoreFixture _fixture;

        public GcpDatastoreMqttIntentStoreTests(GcpDataStoreFixture fixture)
        {
            _fixture = fixture;
        }

        private MqttIntent GenIntent(string name)
        {
            return new MqttIntent
            {
                IntentName = name,
                ParamMapMqtt = new []
                {
                    new ParamMap
                    {
                        Param = "param1",
                        From = "on",
                        To = "1"
                    },
                    new ParamMap
                    {
                        Param = "param2",
                        From = "off",
                        To = "0"
                    }
                },
                ParamMapText = new []
                {
                    new ParamMap
                    {
                        Param = "param1",
                        From = "on",
                        To = "включен"
                    },
                    new ParamMap
                    {
                        Param = "param2",
                        From = "off",
                        To = "выключен"
                    }
                },
                Message = "@state",
                Topic = "lr/light/e/main"
            };

        }

        private Entity MapToEntity(KeyFactory keyFactory, MqttIntent intent)
        {
            var key = keyFactory.CreateKey(intent.IntentName);
            _fixture.EntityKeys.Add(key);
            var paramMapMqtt = new ArrayValue();
            paramMapMqtt.Values.Add(intent.ParamMapMqtt.Select(x => new Value
            {
                StringValue = $"{x.Param}@{x.From}::{x.To}"
            }));

            var paramMapText = new ArrayValue();
            paramMapText.Values.Add(intent.ParamMapText.Select(x => new Value
            {
                StringValue = $"{x.Param}@{x.From}::{x.To}"
            }));

            var e = new Entity
            {
                Key = key,
                ["topic"] = intent.Topic,
                ["message"] = intent.Message,
                ["paramMapMqtt"] = paramMapMqtt,
                ["paramMapText"] = paramMapText
            };
            return e;
        }

        [Fact]
        public async Task ConnectTest()
        {
            var e = MapToEntity(_fixture.KeyFactory, GenIntent("test0"));
            await _fixture.Db.UpsertAsync(e);
        }

        [Fact]
        public async Task IntentExists_FindAsync_ReturnedIntent()
        {
            var intentName = "test intent 1";
            
            var e = MapToEntity(_fixture.KeyFactory, GenIntent(intentName));
            await _fixture.Db.UpsertAsync(e);

            var i = await _fixture.MqttIntentStore.FindAsync(intentName);
            Assert.NotNull(i);
        }

        [Fact]
        public async Task IntentExists_FindAsync_ReturnedTheSameAsCreated()
        {
            var intentName = "test intent 2";
            var expectedIntent = GenIntent(intentName);
            var e = MapToEntity(_fixture.KeyFactory, expectedIntent);
            await _fixture.Db.UpsertAsync(e);

            var actualIntent = await _fixture.MqttIntentStore.FindAsync(intentName);
            Assert.NotNull(actualIntent);
            Assert.Equal(JsonConvert.SerializeObject(expectedIntent, Formatting.Indented), JsonConvert.SerializeObject(actualIntent, Formatting.Indented));
        }

        [Fact]
        public async Task FewIntentExists_FindAsync_ReturnedOnlyMatched()
        {
            var intentName = "test intent 3";
            
            var expectedIntent = GenIntent(intentName);
            var e1 = MapToEntity(_fixture.KeyFactory, expectedIntent);
            var e2 = MapToEntity(_fixture.KeyFactory, GenIntent("test intent 4"));
            var e3 = MapToEntity(_fixture.KeyFactory, GenIntent("test intent 5"));
            await _fixture.Db.UpsertAsync(e1, e2, e3);

            var actualIntent = await _fixture.MqttIntentStore.FindAsync(intentName);
            Assert.NotNull(actualIntent);
            Assert.Equal(JsonConvert.SerializeObject(expectedIntent, Formatting.Indented), JsonConvert.SerializeObject(actualIntent, Formatting.Indented));
        }


        [Fact]
        public async Task FewIntentExists_GetAllAsync_AllReturned()
        {
            var expectedIntent = GenIntent("test intent 6");
            var e1 = MapToEntity(_fixture.KeyFactory, expectedIntent);
            var e2 = MapToEntity(_fixture.KeyFactory, GenIntent("test intent 7"));
            var e3 = MapToEntity(_fixture.KeyFactory, GenIntent("test intent 8"));
            var expectedIntents = new[] {e1, e2, e3};
            await _fixture.Db.UpsertAsync(expectedIntents);

            var actualIntents = await _fixture.MqttIntentStore.GetAllAsync();
            Assert.NotNull(actualIntents);
            Assert.All(expectedIntents, expectedIntent =>
            {
                Assert.Contains(actualIntents, 
                    x =>string.Equals(expectedIntent.Key.Path[0].Name,x.IntentName, StringComparison.InvariantCulture));
            });
        }
    }
}
