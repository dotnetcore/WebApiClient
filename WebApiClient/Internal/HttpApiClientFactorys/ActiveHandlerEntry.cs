using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClient
{
    class ActiveHandlerEntry
    {
        public Type ApiType { get; set; }

        public HttpApiConfig HttpApiConfig { get; set; }

        public HttpMessageHandler InnerHandler { get; set; }

        public ActiveHandlerEntry(IHttpApiClientFactory factory)
        {
            Task.Delay(factory.Lifetime)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => factory.OnEntryDeactivate(this));
        }
    }
}
