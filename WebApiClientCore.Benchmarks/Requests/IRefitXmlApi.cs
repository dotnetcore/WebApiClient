using Refit;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public interface IRefitXmlApi
    { 
        [Post("/benchmarks")]
        Task<User> PostXmlAsync([Body(BodySerializationMethod.Serialized)] User model); 
    }
}
