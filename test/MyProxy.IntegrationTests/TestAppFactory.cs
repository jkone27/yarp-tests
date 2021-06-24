using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Service.Proxy.Infrastructure;
using MyProxy;

namespace withTravixCommon.IntegrationTests
{
    public class TestAppFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseContentRoot(System.IO.Directory.GetCurrentDirectory())
                .UseUrls("http://*:5000/", "http://*:9090/")
                .ConfigureAppConfiguration(cfg =>
                    cfg.AddJsonFile("appsettings.json").AddEnvironmentVariables()
                ).ConfigureServices(s =>
                    {
                        s.AddSingleton<IProxyHttpClientFactory, TestProxyHttpClientFactory>();
                        s.AddHttpClient();
                    }
                );
        }
    }
}
