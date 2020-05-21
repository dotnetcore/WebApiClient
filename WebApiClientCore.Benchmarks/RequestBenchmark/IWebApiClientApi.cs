using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClientCore.Benchmarks.RequestBenchmark
{
    public interface IWebApiClientApi : WebApiClient.IHttpApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task<Model> GetAsyc([PathQuery] string id);
    }
}
