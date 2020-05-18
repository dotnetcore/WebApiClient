using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Test.Attributes
{
    public interface ITestApi : IHttpApi
    {
        Task<HttpResponseMessage> PostAsync(object value);
    }
}
