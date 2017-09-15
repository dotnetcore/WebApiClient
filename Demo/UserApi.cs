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
    [HttpHost("http://localhost:9999")] // 可以在Implement传Url覆盖
    public interface UserApi
    {
        // GET webapi/user/GetById?id=id001
        // Return HttpResponseMessage
        [HttpGet("/webapi/user/GetById?id={id}")]
        Task<HttpResponseMessage> GetByIdAsync(string id);

        // GET webapi/user/GetByAccount?account=laojiu
        // Return 原始string内容
        [HttpGet("/webapi/user/GetByAccount")]
        Task<string> GetByAccountAsync(string account);


        // POST webapi/user/UpdateWithForm  
        // Body Account=laojiu&Password=123456
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithForm")]
        Task<UserInfo> UpdateWithFormAsync([FormContent] UserInfo user);

        // POST webapi/user/UpdateWithJson
        // Body {"Account":"laojiu","Password":"123456"}
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithJson")]
        Task<UserInfo> UpdateWithJsonAsync([JsonContent] UserInfo user);

        // POST webapi/user/UpdateWithXml 
        // Body <?xml version="1.0" encoding="utf-8"?><UserInfo><Account>laojiu</Account><Password>123456</Password></UserInfo>
        // Return xml内容
        [XmlReturn]
        [HttpPost("/webapi/user/UpdateWithXml")]
        Task<UserInfo> UpdateWithXmlAsync([XmlContent] UserInfo user);

        // POST webapi/user/UpdateWithMulitpart  
        // Body multipart/form-data
        // Return json或xml内容
        [HttpPost("/webapi/user/UpdateWithMulitpart")]
        Task<UserInfo> UpdateWithMulitpartAsync([MulitpartContent] UserInfo user, params MulitpartFile[] files);
    }
}
