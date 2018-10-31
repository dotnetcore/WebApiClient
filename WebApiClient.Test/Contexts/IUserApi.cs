using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Parameterables;

namespace WebApiClient.Test.Contexts
{
    /// <summary>
    /// 用户操作接口
    /// </summary>
    [TraceFilter]
    [HttpHost("http://localhost")]
    public interface IUserApi : IHttpApi
    {
        [HttpGet]
        [Timeout(10 * 1000)]
        Task<string> Get1([Uri] string url, string something);

        [HttpGet]
        [JsonReturn]
        ITask<HttpResponseMessage> Get2([Required]string id, CancellationToken token);

        [HttpGet]
        ITask<Stream> Get3([Required]string account, CancellationToken token);

        [HttpGet] 
        ITask<HttpResponseFile> Get4([Uri, Required] string uri);


        [HttpGet] 
        ITask<object> Get5(string nickName);

        [HttpGet]
        ITask<byte[]> Get6(string nickName);
    }
}
