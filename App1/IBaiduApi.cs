using System;
using WebApiClient;
using WebApiClient.Attributes;

namespace App1
{
    public interface IBaiduApi : IDisposable
    {
        [HttpGet]
        ITask<string> GetAsync([Url] string uri);
    }
}
