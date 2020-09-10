using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    interface IHttpApiOptionsChangeTokenSource
    {
        void NotifyChanged(Type httpApiType);
    }

    class HttpApiOptionsChangeTokenSource<THttpApi> : IHttpApiOptionsChangeTokenSource, IOptionsChangeTokenSource<HttpApiOptions>
    {
        private ChangeToken changeToken = new ChangeToken();

        public string Name => HttpApi.GetName<THttpApi>();

        public IChangeToken GetChangeToken()
        {
            return this.changeToken;
        }

        public void NotifyChanged(Type httpApiType)
        {
            if (this.Name == HttpApi.GetName(httpApiType))
            {
                Interlocked.Exchange(ref this.changeToken, new ChangeToken()).NotifyChanged();
            }
        }

        private class ChangeToken : IChangeToken
        {
            private readonly CancellationTokenSource cts = new CancellationTokenSource();

            public bool ActiveChangeCallbacks => true;

            public bool HasChanged => cts.IsCancellationRequested;

            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                return cts.Token.Register(callback, state);
            }

            public void NotifyChanged()
            {
                this.cts.Cancel();
            }
        }
    }
}
