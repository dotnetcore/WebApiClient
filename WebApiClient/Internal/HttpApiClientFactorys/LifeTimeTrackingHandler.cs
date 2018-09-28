using System.Net.Http;

namespace WebApiClient
{
    class LifeTimeTrackingHandler : DelegatingHandler
    {
        public LifeTimeTrackingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected sealed override void Dispose(bool disposing)
        {
        }
    }
}
