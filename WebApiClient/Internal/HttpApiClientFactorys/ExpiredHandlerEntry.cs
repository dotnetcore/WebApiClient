using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    class ExpiredHandlerEntry : IDisposable
    {
        private readonly IDisposable innerHandler;

        private readonly WeakReference weakReference;

        public Type ApiType { get; private set; }

        public ExpiredHandlerEntry(ActiveHandlerEntry active)
        {
            this.innerHandler = active.InnerHandler;
            this.weakReference = new WeakReference(active.HttpApiConfig);
            this.ApiType = active.ApiType;
        }

        public bool CanDispose()
        {
            return weakReference.IsAlive == false;
        }

        public void Dispose()
        {
            if (this.CanDispose() == true)
            {
                this.innerHandler.Dispose();
            }
        }
    }
}
