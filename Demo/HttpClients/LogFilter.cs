using System;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace Demo.HttpClients
{
    /// <summary>
    /// 记录日志的过滤器
    /// </summary>
    class LogFilter : ApiActionFilterAttribute
    {
        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnBeginRequestAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            var dateTime = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine("{0} {1} {2}", dateTime, request.Method, request.RequestUri);

            return base.OnBeginRequestAsync(context);
        }

        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnEndRequestAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            var dateTime = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.WriteLine("{0} {1} {2}完成", dateTime, request.Method, request.RequestUri.AbsolutePath);

            // 输入返回结果
            var result = await context.ResponseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            Console.WriteLine();
        }
    }
}
