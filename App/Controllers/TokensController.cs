using Microsoft.AspNetCore.Mvc;
using WebApiClientCore.Extensions.OAuths;
using WebApiClientCore.Extensions.OAuths.TokenProviders;

namespace App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        [HttpPost]
        public TokenResult CreateToken([FromForm] ClientCredentials credentials)
        {
            return new TokenResult
            {
                Access_token = $"Access_token_{credentials.Client_id}_{credentials.Client_secret}",
                Expires_in = 60 * 60,
                Id_token = "id",
                Token_type = "Bearer"
            };
        }
    }
}
