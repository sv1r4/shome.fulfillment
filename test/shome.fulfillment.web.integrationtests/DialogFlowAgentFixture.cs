using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using shome.fulfillment.web.config;

namespace shome.fulfillment.web.integrationtests
{
    public class DialogFlowAgentFixture
    {
        public DialogFlowConfig DialogFlowConfig { get; }

        public DialogFlowAgentFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            var config = builder.Build();

            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<DialogFlowConfig>(config.GetSection(nameof(DialogFlowConfig)));

            var sp = services.BuildServiceProvider();

            DialogFlowConfig = sp.GetRequiredService<IOptions<DialogFlowConfig>>().Value;
        }
    }
}
