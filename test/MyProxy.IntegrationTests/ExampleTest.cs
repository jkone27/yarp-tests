using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace withTravixCommon.IntegrationTests
{

    public class ExampleTest : IClassFixture<TestAppFactory>
    {
        private readonly TestAppFactory testAppFactory;

        public ExampleTest(TestAppFactory testAppFactory)
        {
            this.testAppFactory = testAppFactory;
        }

        [Theory]
        [InlineData("/test-proxy-one","https://127.0.0.1/hello/test-proxy-one")]
        [InlineData("/test-proxy-two","https://127.0.0.1/hello/test-proxy-two")]
        public async Task Proxy_Configured_Ok(string requestPath, string proxiedUri)
        {

            var handler = new TestProxyHandler(r => r.RequestUri.ToString() == proxiedUri);

            var client = testAppFactory.WithWebHostBuilder(b => 
                b.ConfigureTestServices(s => 
                    s.AddSingleton(handler)
                )
            ).CreateClient();

            var response = await client.GetAsync(requestPath);

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Proxy_NonConfiguredRoute_Fails()
        {
            var handler = new TestProxyHandler(r => 
                r.RequestUri.ToString() == "https://127.0.0.1/not-configured/test-proxy");

            var client = testAppFactory.WithWebHostBuilder(b => 
                b.ConfigureTestServices(s => 
                    s.AddSingleton(handler)
                )
            ).CreateClient();

            var response = await client.GetAsync("/not-configured");

            Assert.ThrowsAny<Exception>(() => response.EnsureSuccessStatusCode());
        }
    }
}
