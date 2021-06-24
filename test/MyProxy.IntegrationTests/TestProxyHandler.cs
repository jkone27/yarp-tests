using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Net;

namespace withTravixCommon.IntegrationTests
{
    public class TestProxyHandler : DelegatingHandler
    {
        //settable predicate for tests
        private readonly Predicate<HttpRequestMessage> requestPredicate = _ => true;

        public TestProxyHandler(Predicate<HttpRequestMessage> requestPredicate = null) 
        : base(new HttpClientHandler()) { 
            this.requestPredicate = requestPredicate ?? (_ => true);
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (requestPredicate(request))
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.RequestMessage = request;
                response.ReasonPhrase = "OK";
                return Task.FromResult<HttpResponseMessage>(response);
            }
            else
                throw new Exception("failure");

        }
    }
}
