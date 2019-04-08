using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Parameterables;

namespace Demo.HttpClients
{
    /// <summary>
    /// 用户操作接口
    /// </summary>
    [TraceFilter]   
    [HttpHost("http://localhost:9999/")] // HttpHost可以在Config传入覆盖
    public interface IUserApi : IHttpApi
    {
        // GET {url}?account={account}&password={password}&something={something}
        [HttpGet]
        [Cache(10 * 1000)]
        Task<string> GetAboutAsync(
            [Uri] string url,
            UserInfo user,
            string something);

        // /GET webapi/user/GetById?id=id001
        // Return HttpResponseMessage
        [HttpGet("webapi/user/GetById/{id}")]
        [BasicAuth("userName", "password")]
        [Timeout(10 * 1000)] // 10s超时
        ITask<HttpResponseMessage> GetByIdAsync(
            [Required]string id,
            CancellationToken token);

        // GET webapi/user/GetByAccount?account=laojiu
        // Return 原始string内容
        [HttpGet("webapi/user/GetByAccount")]
        ITask<string> GetByAccountAsync(
            [Required]string account,
            CancellationToken token);


        // POST webapi/user/UpdateWithForm  
        // Body Account=laojiu&Password=123456&name=value&nickName=老九&age=18
        // Return json或xml内容
        [HttpPost("webapi/user/UpdateWithForm")]
        [FormField("name", "value")] // 固定的参数值可以这么写
        ITask<UserInfo> UpdateWithFormAsync(
            [FormContent, Required] UserInfo user,
            [FormField] string nickName,
            [FormField, Range(1, 100)] int? age);

        // POST webapi/user/UpdateWithJson
        // Body {"Account":"laojiu","Password":"123456"}
        // Return json或xml内容
        [HttpPost("webapi/user/UpdateWithJson")]
        ITask<UserInfo> UpdateWithJsonAsync(
            [JsonContent, Required] UserInfo user);

        // POST webapi/user/UpdateWithXml 
        // Body <?xml version="1.0" encoding="utf-8"?><UserInfo><Account>laojiu</Account><Password>123456</Password></UserInfo>
        // Return xml内容       
        [HttpPost("webapi/user/UpdateWithXml")]
        ITask<UserInfo> UpdateWithXmlAsync(
            [XmlContent, Required] UserInfo user);

        // POST webapi/user/UpdateWithMulitpart  
        // Body multipart/form-data
        // Return json或xml内容
        [HttpPost("webapi/user/UpdateWithMulitpart")]
        [TraceFilter(Enable = false)]
        ITask<UserInfo> UpdateWithMulitpartAsync(
            [MulitpartContent, Required] UserInfo user,
            [MulitpartText] string nickName,
            [MulitpartText] int age,
            MulitpartFile file);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="uri">文件相对或绝对路径</param>
        /// <returns></returns>
        [HttpGet]
        [TraceFilter(Enable = false)]
        ITask<HttpResponseFile> DownloadFileAsync([Uri, Required] string uri);
    }
}
