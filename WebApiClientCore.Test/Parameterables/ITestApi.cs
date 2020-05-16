using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Test.Parameterables
{
    public interface ITestApi 
    {
        Task<HttpResponseMessage> PostAsync(object value);
    }
}
