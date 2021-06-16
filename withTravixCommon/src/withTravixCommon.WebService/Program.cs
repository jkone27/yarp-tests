using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace withTravixCommon.WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    config
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // 5000 is the main port we use for the application, and 9090 is for the Prometheus metrics endpoint.
                    webBuilder.UseStartup<Startup>().UseUrls("http://*:5000/", "http://*:9090/");
                });
    }
}
