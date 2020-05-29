using Refit;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IRefitApi
    {
        [Get("/benchmarks/{id}")]
        Task<Model> GetAsyc(string id);

        [Post("/benchmarks")]
        Task<Model> PostJsonAsync([Body(BodySerializationMethod.Serialized)]Model model);

        [Post("/benchmarks")]
        Task<Model> PostFormAsync([Body(BodySerializationMethod.UrlEncoded)]Model model);
    }
}
