using Refit;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IRefitApi
    {
        [Get("/benchmarks/{id}")]
        Task<Model> GetAsyc(string id);

        [Post("/benchmarks")]
        Task<Model> PostAsync(Model model);
    }
}
