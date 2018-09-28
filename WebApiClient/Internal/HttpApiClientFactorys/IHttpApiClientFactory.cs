using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    interface IHttpApiClientFactory
    {
        TimeSpan Lifetime { get; set; }

        void OnEntryDeactivate(ActiveHandlerEntry entry);
    }
}
