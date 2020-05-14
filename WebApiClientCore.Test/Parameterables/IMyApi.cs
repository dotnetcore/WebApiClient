using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Test.Parameterables
{
    public interface IMyApi : IDisposable
    {
        Task<HttpResponseMessage> PostAsync(string name);
    }
}
