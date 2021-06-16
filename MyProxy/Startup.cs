using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MyProxy
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the reverse proxy to capability to the server
            var proxyBuilder = services.AddReverseProxy();
            // Initialize the reverse proxy from the "ReverseProxy" section of configuration
            proxyBuilder.LoadFromConfig(configuration.GetSection("SomeOtherName"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            // Enable endpoint routing, required for the reverse proxy
            app.UseRouting();
            // Register the reverse proxy routes
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapReverseProxy();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/why", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
