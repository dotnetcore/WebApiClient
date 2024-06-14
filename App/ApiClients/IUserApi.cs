using App.Models;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameters;

namespace App.Clients
{
    /// <summary>
    /// 用户操作接口
    /// </summary>    
    [OAuthToken]
    [LoggingFilter]
    public interface IUserApi : IHttpApi
    {
        [HttpGet("api/users/{account}")]
        ITask<HttpResponseMessage> GetAsync(string account);

        [HttpGet("api/users/{account}")]
        ITask<string> GetAsStringAsync(string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        ITask<byte[]> GetAsByteArrayAsync(string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        ITask<Stream> GetAsStreamAsync(string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        ITask<User> GetAsModelAsync(string account, CancellationToken token = default);

        [JsonReturn]
        [HttpGet("api/users/{account}")]
        ITask<User> GetExpectJsonAsync(string account, CancellationToken token = default);

        [XmlReturn]
        [HttpGet("api/users/{account}")]
        ITask<User> GetExpectXmlAsync(string account, CancellationToken token = default);



        [HttpPost("api/users/body")]
        Task<User> PostByJsonAsync([JsonContent] User user, CancellationToken token = default);

        [HttpPost("api/users/body")]
        Task<User> PostByXmlAsync([XmlContent] User user, CancellationToken token = default);

        [HttpPost("api/users/form")]
        Task<User> PostByFormAsync([FormContent] User user, CancellationToken token = default);

        [HttpPost("api/users/formdata")]
        Task<User> PostByFormDataAsync([FormDataContent] User user, FormDataFile file, CancellationToken token = default);



        [HttpDelete("api/users/{account}")]
        Task DeleteAsync(string account);
    }
}
