using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    class HttpApiOptionsNotifer<THttpApi> : IHttpApiOptionsNotifer<THttpApi>
    {
        private readonly IHttpApiOptionsChangeTokenSource[] changeTokenSources;

        public HttpApiOptionsNotifer(IEnumerable<IOptionsChangeTokenSource<HttpApiOptions>> changeTokenSources)
        {
            this.changeTokenSources = changeTokenSources.OfType<IHttpApiOptionsChangeTokenSource>().ToArray();
        }

        public void ReloadHttpApiOptions()
        {
            foreach (var item in this.changeTokenSources)
            {
                item.NotifyChanged(typeof(THttpApi));
            }
        }
    }
}
