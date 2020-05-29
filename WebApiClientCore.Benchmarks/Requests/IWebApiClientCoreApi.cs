using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IWebApiClientCoreApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task<Model> GetAsyc(string id);

        [HttpPost("/benchmarks")]
        Task<Model> PostJsonAsync([JsonContent] Model model);

        [HttpPost("/benchmarks")]
        Task<Model> PostFormAsync([FormContent] Model model);
    }
}
