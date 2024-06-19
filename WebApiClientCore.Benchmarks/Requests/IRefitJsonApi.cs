using Refit;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IRefitJsonApi
    {
        [Get("/benchmarks/{id}")]
        Task GetAsync(string id);

        [Get("/benchmarks/{id}")]
        Task<User> GetJsonAsync(string id);

        [Post("/benchmarks")]
        Task<User> PostJsonAsync([Body(BodySerializationMethod.Serialized)] User model);

        [Put("/benchmarks/{id}")]
        Task<User> PutFormAsync(string id, [Body(BodySerializationMethod.UrlEncoded)] User model);
    }
}
