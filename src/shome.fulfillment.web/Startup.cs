using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shome.fulfillment.mqtt.mqttnet.extensions;
using shome.fulfillment.store.gcp.datastore.extensions;
using shome.fulfillment.web.authentication;
using shome.fulfillment.web.extensions;


namespace shome.fulfillment.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddGcpDatastore(Configuration);

            services.AddMqttNetPublisher(Configuration);

            services.AddScoped<IWebHookHandler, MqttWebHookHandler>();

            services.AddOptions();
            
            //todo stack driver log format

            services.AddAuthentication(FulfillmentAuthDefaults.Scheme)
                .AddFulfillmentCustomAuthentication(FulfillmentAuthDefaults.Scheme,
                    "Dialog Flow Fulfillment Authentication",
                    options =>
                    {
                        Configuration.GetSection(nameof(FulfillmentAuthenticationOptions)).Bind(options);
                    })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
          
            
            app.UseRouting();
            app.UseAuthentication();
			app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
