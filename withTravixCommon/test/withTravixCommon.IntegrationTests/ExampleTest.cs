using System;
using System.Threading.Tasks;
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

        [Fact]
        public async Task Proxy_Configured_Ok()
        {
            TestProxyHandler.RequestPredicate = r => r.RequestUri.ToString() == "https://127.0.0.1/hello/test-proxy";

            var client = testAppFactory.CreateClient();

            var response = await client.GetAsync("/test-proxy");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Proxy_NonConfiguredRoute_Fails()
        {
            TestProxyHandler.RequestPredicate = r => r.RequestUri.ToString() == "https://127.0.0.1/not-configured/test-proxy";

            var client = testAppFactory.CreateClient();

            var response = await client.GetAsync("/not-configured");

            Assert.ThrowsAny<Exception>(() => response.EnsureSuccessStatusCode());
        }
    }
}
