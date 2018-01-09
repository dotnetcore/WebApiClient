using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;

namespace Demo
{
    [Logger] // 记录请求日志
    [HttpHost("http://localhost:9999")] // HttpHost可以在Config传入覆盖    
    [AutoReturn(EnsureSuccessStatusCode = true)]
    public interface UserApi : IDisposable
    {
        // GET {url}?account={account}&password={password}&something={something}
        [HttpGet]
        [Header("Cookie", "a=1; b=2")]
        [Timeout(10 * 1000)] // 10s超时
        Task<string> GetAboutAsync(
            [Url] string url,
            [Header("Authorization")] string authorization,
            UserInfo user,
            string something);

        // /GET webapi/user/GetById?id=id001
        // Return HttpResponseMessage
        [HttpGet("/webapi/user/GetById?id={id}")]
        [BasicAuth("userName", "password")]
        ITask<HttpResponseMessage> GetByIdAsync(string id);

        // GET /webapi/user/GetByAccount?account=laojiu
        // Return 原始string内容
        [HttpGet("/webapi/user/GetByAccount")]
        ITask<string> GetByAccountAsync(string account);


        // POST /webapi/user/UpdateWithForm  
        // Body Account=laojiu&Password=123456&name=value&nickName=老九&age=18
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithForm")]
        [FormField("name", "value")] // 固定的参数值可以这么写
        ITask<UserInfo> UpdateWithFormAsync([FormContent] UserInfo user, FormField nickName, [AliasAs("age")][FormField] int? nullableAge);

        // POST /webapi/user/UpdateWithJson
        // Body {"Account":"laojiu","Password":"123456"}
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithJson")]
        ITask<UserInfo> UpdateWithJsonAsync([JsonContent] UserInfo user);

        // POST /webapi/user/UpdateWithXml 
        // Body <?xml version="1.0" encoding="utf-8"?><UserInfo><Account>laojiu</Account><Password>123456</Password></UserInfo>
        // Return xml内容
        [XmlReturn]
        [HttpPost("/webapi/user/UpdateWithXml")]
        ITask<UserInfo> UpdateWithXmlAsync([XmlContent] UserInfo user);

        // POST /webapi/user/UpdateWithMulitpart  
        // Body multipart/form-data
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithMulitpart")]
        ITask<UserInfo> UpdateWithMulitpartAsync([MulitpartContent] UserInfo user, [MulitpartText] string nickName, MulitpartText age, params MulitpartFile[] files);
    }
}
