using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将日志输出到控制台的追踪过滤器
    /// </summary>
    public class TraceFilterAttribute : ApiActionFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "TraceFilter";

        /// <summary>
        /// 获取或设置相关的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task OnBeginRequestAsync(ApiActionContext context)
        {
            var request = new Request
            {
                Time = DateTime.Now,
                Message = await context.RequestMessage.ToStringAsync().ConfigureAwait(false)
            };
            context.Tags.Set(tagKey, request);
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task OnEndRequestAsync(ApiActionContext context)
        {
            var request = context.Tags.Get(tagKey).As<Request>();
	    var response = new Response
            {
                Time = DateTime.Now,
                Exception = context.Exception
            };

            if (response.Exception == null)
            {
                response.Message = await context.ResponseMessage.Content.ReadAsStringAsync();
            }

            WriteLine($"[{this.Name ?? context.ApiActionDescriptor.Name}]", ConsoleColor.Green);
            WriteLine($"[Request][{request.Time}]", ConsoleColor.DarkYellow);
            WriteLine($"{request.Message.TrimEnd()}");
            WriteLine($"[Response][{response.Time}]", ConsoleColor.DarkYellow);
            WriteLine($"{response.Message}");

            if (response.Exception != null)
            {
                WriteLine($"[Exception]", ConsoleColor.Red);
                WriteLine($"{response.Exception}", ConsoleColor.Magenta);
            }
            WriteLine(null);
        }

        /// <summary>
        /// 带颜色控制器输出行
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }

        /// <summary>
        /// 请求信息
        /// </summary>
        private class Request
        {
            public DateTime Time { get; set; }

            public string Message { get; set; }
        }

        /// <summary>
        /// 响应信息
        /// </summary>
        private class Response
        {
            public DateTime Time { get; set; }

            public string Message { get; set; }

            public Exception Exception { get; set; }
        }
    }
}
