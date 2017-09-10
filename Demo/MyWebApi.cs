using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace Demo
{
    [Logger] // 记录请求日志
    [JsonReturn] // 默认返回内容为Json
    [HttpHost("http://www.mywebapi.com")] // 可以在Implement传Url覆盖
    public interface MyWebApi
    {
        // GET webapi/user/id001
        // Return json内容
        [HttpGet("/webapi/user/{id}")]
        Task<HttpResponseMessage> GetUserByIdAsync(string id);

        // GET webapi/user?account=laojiu
        // Return json内容
        [HttpGet("/webapi/user")]
        Task<string> GetUserByAccountAsync(string account);


        // POST webapi/user  
        // Body:Account=laojiu&Password=123456
        // Return json内容
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);

        // POST webapi/user   
        // Body:{"Account":"laojiu","Password":"123456"}
        // Return json内容
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithJsonAsync([JsonContent] UserInfo user);

        // POST webapi/user   
        // Body: xml内容
        // Return xml内容
        [XmlReturn]
        [HttpPost("/webapi/user")]
        Task<UserInfo> UpdateUserWithXmlAsync([XmlContent] UserInfo user);
    }
}
