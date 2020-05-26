using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IWebApiClientCoreApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task<Model> GetAsyc(string id);

        [HttpPost("/benchmarks")]
        Task<Model> PostAsync([JsonContent] Model model);
    }
}
