using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Pages
{
    public class IndexModel(ITestApi baseApi, ILogger<IndexModel> logger) : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            foreach (var item in from index in Enumerable.Range(1, 100000)
                                 let item = $"app{Random.Shared.Next(1, 10)}"
                                 select item)
            {
                try
                {
                    var result = await baseApi.GetAsync(item, item);
                    result.EnsureSuccessStatusCode();

                    logger.LogInformation("{item}\tOK", item);
                }
                catch (HttpRequestException ex)
                {
                    logger.LogInformation("{item}\t{message}", item, (ex.InnerException ?? ex).Message);
                }
            }

            return new OkResult();
        }
    }
}
