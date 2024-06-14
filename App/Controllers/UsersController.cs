using App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [TokenFilter]
    [ApiController]
    [Route("api/[controller]")]
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
        public User PostByFormData([FromForm] User formDataUser, IFormFile file)
        {
            return formDataUser;
        }

        [HttpDelete("{account}")]
        public void Delete(string account)
        {
        }
    }
}
