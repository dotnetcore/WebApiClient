using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Benchmarks.Requests
{
    [XmlReturn]
    [JsonReturn(Enable = false)]
    public interface IWebApiClientCoreXmlApi
    {
        [HttpPost("/benchmarks")]
        Task<User> PostXmlAsync([XmlContent] User model);
    }
}
