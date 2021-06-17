using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Yarp.ReverseProxy.Service.Proxy.Infrastructure;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using withTravixCommon.WebService;

namespace withTravixCommon.IntegrationTests
{
    public class TestProxyHandler : DelegatingHandler
    {
        public TestProxyHandler(HttpClientHandler handler) : base(handler) { }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (RequestPredicate(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.RequestMessage = request;
                response.ReasonPhrase = "OK";
                return Task.FromResult<HttpResponseMessage>(response);
            }
            else
                throw new Exception("failure");
            //return base.SendAsync(request, cancellationToken);
        }

        public static Predicate<HttpRequestMessage> RequestPredicate { get; set; }
    }

    public sealed class TestProxyHttpClientFactory : IProxyHttpClientFactory
    {
        public HttpMessageInvoker CreateClient(ProxyHttpClientContext context)
        {
            return new HttpMessageInvoker(httpClientHandler, false);
        }

        static TestProxyHttpClientFactory()
        {
            httpClientHandler = new TestProxyHandler(new HttpClientHandler());
        }

        public static readonly TestProxyHandler httpClientHandler;
    }

    //public sealed class DefaultHttpClientFactory : IHttpClientFactory
    //{
    //    public HttpClient CreateClient(string name) => httpClient;

    //    static DefaultHttpClientFactory()
    //    {
    //        httpClient = new HttpClient(DefaultProxyHttpClientFactory.httpClientHandler, false);
    //        httpClient.BaseAddress = new Uri("https://127.0.0.1", UriKind.Absolute);
    //    }

    //    public static readonly HttpClient httpClient;
    //}

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
                        var serviceDescriptors = s.Where(x => x.ServiceType.FullName.Contains("ttpClientFactory")).ToList();
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            s.Remove(serviceDescriptor);
                        }

                        s.AddSingleton<IProxyHttpClientFactory, TestProxyHttpClientFactory>();

                        s.AddHttpClient();
                        //s.AddSingleton<IHttpClientFactory, DefaultHttpClientFactory>();
                    }
                );
        }

        //protected override void ConfigureClient(HttpClient client)
        //{
        //    client.BaseAddress = new System.Uri("https://127.0.0.1");
        //    base.ConfigureClient(client);
        //}
    }
}
