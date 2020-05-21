using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClientCore.Benchmarks.RequestBenchmark
{
    public interface IWebApiClientApi : WebApiClient.IHttpApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task<Model> GetAsyc([PathQuery] string id);

        [HttpPost("/benchmarks")]
        Task<Model> PostAsync([Parameter(Kind.JsonBody)] Model model);
    }
}
