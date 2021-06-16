using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Travix.Common.Bootstrapper.Web;
using Travix.Common.Logging;

namespace withTravixCommon.WebService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        private readonly IConfiguration configuration;

        private readonly IWebHostEnvironment webHostEnvironment;

        public void ConfigureServices(IServiceCollection services)
        {
            var proxyBuilder = services.AddReverseProxy();
            var proxyConfig = configuration.GetSection("SomeOtherName");

            proxyBuilder.LoadFromConfig(proxyConfig);

            services.BootstrapEndpointsWebApplication(
                webHostEnvironment,
                configuration,
                opt =>
                {
                    opt.ApplicationInformation.ApplicationName = "testweb";
                    opt.ApplicationInformation.ApplicationGroup = "test";
                },
                pipeline => {
                    pipeline.Steps.Remove(pipeline.Steps.Single(it => it.Name == BootstrapStepNames.Diagnostics.ToString()));

                    // attach yarp proxies

                    pipeline.Steps.Single(it => it.Name == BootstrapStepNames.Endpoints.ToString())
                        .ApplicationBuilderSetup = (app, _) =>
                    {
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapReverseProxy(proxyPipeline =>
                                proxyPipeline.Use((context, next) =>
                                {
                                    //var logger = context.RequestServices.GetRequiredService<ILogger>();
                                    //logger.LogDebug(LogEvent.ProxyLog, $"{context.Request.Method} : {context.Request.Path}");

                                    return next();
                                })
                            );
                        });
                    };
                });

        }

        public void Configure(IApplicationBuilder app)
        {
            
        }
    }
}
