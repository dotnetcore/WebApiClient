using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.DataAnnotations;

namespace UWPApp
{
    [HttpHost("https://www.baidu.com/")]
    interface IBaiduApi : IHttpApi
    {
        [HttpGet("/")]
        ITask<string> HomeAsync();
    }
}
