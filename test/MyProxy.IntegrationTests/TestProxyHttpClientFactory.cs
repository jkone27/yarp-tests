using System.Net.Http;
using Yarp.ReverseProxy.Service.Proxy.Infrastructure;

namespace withTravixCommon.IntegrationTests
{
    public sealed class TestProxyHttpClientFactory : IProxyHttpClientFactory
    {
        public HttpMessageInvoker CreateClient(ProxyHttpClientContext context)
        {
            return new HttpMessageInvoker(httpClientHandler, false);
        }

        public TestProxyHttpClientFactory(TestProxyHandler testProxyHandler)
        {
            httpClientHandler = testProxyHandler;
        }

        public readonly TestProxyHandler httpClientHandler;
    }
}
