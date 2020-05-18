using System.ComponentModel.DataAnnotations;
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
    [LoggingFilter]
    [HttpHost("http://localhost:6000/")]
    public interface IUserApi_ParameterStyle : IHttpApi
    {
        [HttpGet("api/users/{account}")]
        Task<HttpResponseMessage> GetAsync([Required, Parameter(Kind.Path)]string account);

        [HttpGet("api/users/{account}")]
        Task<string> GetAsStringAsync([Required, Parameter(Kind.Path)]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        Task<byte[]> GetAsByteArrayAsync([Required, Parameter(Kind.Path)]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        Task<Stream> GetAsStreamAsync([Required, Parameter(Kind.Path)]string account, CancellationToken token = default);

        [HttpGet("api/users/{account}")]
        Task<User> GetAsModelAsync([Required, Parameter(Kind.Path)]string account, CancellationToken token = default);




        [HttpPost("api/users/body")]
        Task<User> PostByJsonAsync([Required, Parameter(Kind.JsonBody)]User user, CancellationToken token = default);

        [HttpPost("api/users/body")]
        Task<User> PostByXmlAsync([Required, Parameter(Kind.XmlBody)]User user, CancellationToken token = default);


        [HttpPost("api/users/form")]
        Task<User> PostByFormAsync([Required, Parameter(Kind.Form)]User user, CancellationToken token = default);

        [HttpPost("api/users/formdata")]
        Task<User> PostByFormDataAsync([Required, Parameter(Kind.FormData)]User user, FormDataFile file, CancellationToken token = default);


        [HttpDelete("api/users/{account}")]
        Task DeleteAsync([Required] string account);
    }
}
