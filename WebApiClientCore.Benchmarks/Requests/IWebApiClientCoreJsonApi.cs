using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks.Requests
{
    [JsonReturn]
    [XmlReturn(Enable = false)]
    public interface IWebApiClientCoreJsonApi
    {
        [HttpGet("/benchmarks/{id}")]
        Task GetAsync(string id);

        [HttpGet("/benchmarks/{id}")]
        Task<User> GetJsonAsync(string id); 

        [HttpPost("/benchmarks")]
        Task<User> PostJsonAsync([JsonContent] User model);

        [HttpPut("/benchmarks/{id}")]
        Task<User> PutFormAsync(string id, [FormContent] User model);
    }
}
