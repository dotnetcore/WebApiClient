using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace AppAot
{
    [HttpHost("https://www.cloudflare-cn.com")]
    [LoggingFilter]
    public interface ICloudflareApi
    {
        [HttpGet("/page-data/app-data.json")]
        Task<AppData> GetAppDataAsync(); 
    }
}
