using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameterables;

namespace App.Clients
{
    /// <summary>
    /// 用户操作接口
    /// </summary>
    [LoggerFilter]
    [HttpHost("http://localhost:6000/")]
    public interface IUserApi : IHttpApi
    {
        [HttpGet("api/users/{account}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        Task<HttpResponseMessage> GetAsync([Required]string account);

        [HttpGet("api/users/{account}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        Task<string> GetAsStringAsync([Required]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        Task<byte[]> GetAsByteArrayAsync([Required]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        Task<Stream> GetAsStreamAsync([Required]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        Task<User> GetAsModelAsync([Required]string account, CancellationToken token = default);




        [HttpPost("api/users/body")]
        Task<User> PostByJsonAsync([Required, JsonContent]User user, CancellationToken token = default);

        [HttpPost("api/users/body")]
        Task<User> PostByXmlAsync([Required, XmlContent]User user, CancellationToken token = default);


        [HttpPost("api/users/form")]
        Task<User> PostByFormAsync([Required, FormContent]User user, CancellationToken token = default);

        [HttpPost("api/users/formdata")]
        Task<User> PostByFormDataAsync([Required, FormDataContent]User user, FormDataFile file, CancellationToken token = default);


        [HttpDelete("api/users/{account}")]
        Task DeleteAsync([Required] string account);
    }
}
