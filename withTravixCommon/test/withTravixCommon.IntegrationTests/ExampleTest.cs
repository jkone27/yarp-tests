using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using withTravixCommon.WebService;
using Xunit;

namespace withTravixCommon.IntegrationTests
{
    public class ExampleTest
    {
        private readonly TestServer server;

        public ExampleTest()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        [Fact]
        public async Task ExampleGet_Called_HelloReturned()
        {
            using (var client = server.CreateClient())
            {
                var response = await client.GetAsync("/example");

                var content = await response.Content.ReadAsStringAsync();

                Assert.Equal("Hello!", content);
            }
        }
    }
}
