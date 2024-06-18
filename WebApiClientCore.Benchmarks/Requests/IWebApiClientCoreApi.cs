using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks.Requests
{
    [JsonReturn]
    public interface IWebApiClientCoreApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task<User> GetJsonAsync(string id);

        [HttpPost("/benchmarks")]
        Task<User> PostJsonAsync([JsonContent] User model);

        [HttpPut("/benchmarks/{id}")]
        Task<User> PutFormAsync(string id, [FormContent] User model);
    }
}
