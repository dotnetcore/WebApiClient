using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController(ILogger<TodosController> logger) : ControllerBase
    {
        [HttpGet("test/{id}")]
        public IResult OnGetAsync(string id)
        {
            string auth = GetAuthorization(HttpContext);

            var result = $"{id}\t{auth}\t{auth.Contains($"{id}_")}";

            logger.LogInformation("{result}", result);

            return Results.Ok();
        }

        [HttpPost("token")]
        public IResult OnPostToken([FromForm] string app_id, [FromForm] string app_secret)
        {
            //ģ����֤ app_id+app_secret ���� Access_token����Ч�� 3 ��
            return Results.Json(new { Access_token = $"{app_id}_{app_secret}_TOKEN_{DateTime.Now:yyyyMMddHHmmssffffff}", Expires_in = 3 });
        }

        static string GetAuthorization(HttpContext context)
        {
            return context.Request.Headers.TryGetValue("Authorization", out var values)
                     ? string.Join(" ", [.. values])
                     : string.Empty;
        }
    }
}
