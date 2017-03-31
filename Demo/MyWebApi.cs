using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace Demo
{
    [JsonReturn]
    [HttpHost("http://www.mywebapi.com")]
    [MyFilter]
    public interface MyWebApi
    {
        [HttpGet("/webapi/{type}/about")] // GET webapi/typeValue/about
        Task<ApiResult<string>> GetAboutAsync(string type);


        [HttpGet("/webapi/user")]  // GET webapi/user?userName=aa&nickName=bb&BeginTime=cc&EndTime=dd
        Task<ApiResult<UserInfo>> GetUserAsync(string userName, string nickName, TimeFilter timeFilter);


        [HttpPut("/webapi/user")] // PUT webapi/user
        Task<ApiResult<bool>> UpdateUserAsync([JsonContent] UserInfo loginInfo);


        [HttpDelete("/webapi/user")] // DELETE  webapi/user?id=idValue
        Task<ApiResult<bool>> DeleteUserAsync(string id);


        [HttpDelete("/webapi/user/{id}")] // DELETE  webapi/user/idValue
        Task<ApiResult<bool>> DeleteUser2Async(string id);

        [DefaultReturn]
        [HttpPost("/webapi/test/{action}")] // POST webapi/test/actionValue?id=id1&id=id2
        Task<string> TestAsync(string action, int[] id, [FormContent] TimeFilter timerFilter);
    }
}
