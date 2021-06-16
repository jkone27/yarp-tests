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

                    //pipeline.InsertBefore(BootstrapStepNames.Endpoints, new CorsBootstrapStep());

                    // attach yarp proxies
                    pipeline.InsertAfter(BootstrapStepNames.EndpointsRouting, CreateProxies(configuration));
                    
                });

            //// Add the reverse proxy to capability to the server
            //var proxyBuilder = services.AddReverseProxy();
            //// Initialize the reverse proxy from the "ReverseProxy" section of configuration
            //proxyBuilder.LoadFromConfig(configuration.GetSection("SomeOtherName"));
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});

            //// Enable endpoint routing, required for the reverse proxy
            //app.UseRouting();
            //// Register the reverse proxy routes
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapReverseProxy();
            //});

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/why", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }

        public static BootstrapStep CreateProxies(IConfiguration configuration)
        {

            //else
            return new BootstrapStep("ApisProxies")
            {
                ServicesSetup = (services, _) =>
                {
                    var proxyBuilder = services.AddReverseProxy();
                    var proxyConfig = configuration.GetSection("SomeOtherName");

                    proxyBuilder.LoadFromConfig(proxyConfig);

                },
                ApplicationBuilderSetup = (app, _) =>
                {
                    // Enable endpoint routing, required for the reverse proxy
                    //app.UseRouting();

                    // Register the reverse proxy routes
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapReverseProxy(proxyPipeline =>
                            proxyPipeline.Use((context, next) =>
                            {
                                //var logger = context.RequestServices.GetRequiredService<ILogger>();
                                //logger.LogDebug(LogEvent.ProxyLog, $"{context.Request.Method} : {context.Request.Path}");

                                return next();
                            })
                        );
                    });
                }
            };

        }
    }
}
