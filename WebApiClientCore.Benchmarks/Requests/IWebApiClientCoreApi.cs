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

        [HttpPut("/benchmarks/{id}")]
        Task<Model> PutFormAsync(string id, [FormContent] Model model);
    }
}
