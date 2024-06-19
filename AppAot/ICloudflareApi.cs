using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace AppAot
{
    [LoggingFilter]
    [HttpHost("https://www.cloudflare-cn.com")]   
    public interface ICloudflareApi
    {
        [HttpGet("/page-data/app-data.json")]
        Task<AppData> GetAppDataAsync(); 
    }
}
