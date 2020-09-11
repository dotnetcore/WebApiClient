using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [TokenFilter]
    public class UsersController : ControllerBase
    {
        [HttpGet("{account}")]
        public User Get(string account)
        {  
            return new User { Account = account, Password = "password" };
        }


        [HttpPost("body")]
        public User PostByBody([FromBody] User bodyUser)
        {
            return bodyUser;
        }

        [HttpPost("form")]
        public User PostByForm([FromForm] User formUser)
        {
            return formUser;
        }

        [HttpPost("formdata")]
        public User PostByFormData([FromForm] User formDatauser, IFormFile file)
        {
            return formDatauser;
        }

        [HttpDelete("{account}")]
        public void Delete(string account)
        {
        }
    }
}
