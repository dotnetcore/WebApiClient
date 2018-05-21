using System;
using System.Net.Http;
using WebApiClient;

namespace WebApiClient.Test.Attributes
{
    public interface IMyApi : IDisposable
    {
        ITask<HttpResponseMessage> PostAsync(string name);
    }
}
