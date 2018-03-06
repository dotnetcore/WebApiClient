using System;
using System.Net.Http;
using WebApiClient;

namespace WebApiClientTest.Attributes
{
    public interface IMyApi : IDisposable
    {
        ITask<HttpResponseMessage> PostAsync(string name);
    }
}
