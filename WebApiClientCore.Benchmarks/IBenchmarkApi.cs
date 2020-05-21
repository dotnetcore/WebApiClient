using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks
{
    public interface IBenchmarkApi
    {
        [HttpGet("/benchmarks/{id}")]
        public Task<BenchmarkModel> GetAsModelAsync([PathQuery] string id);
    }
}
