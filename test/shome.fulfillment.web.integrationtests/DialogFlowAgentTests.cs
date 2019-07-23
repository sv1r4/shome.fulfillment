using System.Linq;
using System.Threading.Tasks;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Dialogflow.V2;
using Xunit;

namespace shome.fulfillment.web.integrationtests
{
    public class DialogFlowAgentTests:IClassFixture<DialogFlowAgentFixture>
    {
        private readonly DialogFlowAgentFixture _fixture;

        public DialogFlowAgentTests(DialogFlowAgentFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAgentTest()
        {
            var agentBuilder = new AgentsClientBuilder
            {
                JsonCredentials = _fixture.DialogFlowConfig.ServiceAccountKey
            };

            var agent =  await agentBuilder.BuildAsync();
            
            var a = await agent.GetAgentAsync(new ProjectName(_fixture.DialogFlowConfig.ProjectId));
            
            Assert.NotNull(a);
        }

          [Fact]
        public async Task GetEntitiesTest()
        {
            var builder = new EntityTypesClientBuilder
            {
                JsonCredentials = _fixture.DialogFlowConfig.ServiceAccountKey,
            };

            var agent =  await builder.BuildAsync();
            var ee = (await agent.ListEntityTypesAsync(new ProjectAgentName(_fixture.DialogFlowConfig.ProjectId)).ReadPageAsync(100)).ToList();
            Assert.NotNull(ee);
            Assert.NotEmpty(ee);
        }
    }
}
