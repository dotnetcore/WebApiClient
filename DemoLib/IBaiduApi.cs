using System;
using WebApiClient;
using WebApiClient.Attributes;

namespace DemoLib
{
    public interface IBaiduApi : IDisposable
    {
        [HttpGet]
        ITask<string> GetAsync([Url] string uri);
    }
}
