using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace Demo
{
    /// <summary>
    /// 记录日志
    /// </summary>
    class LoggerAttribute : ApiActionFilterAttribute
    {
        public override Task OnBeginRequestAsync(ApiActionContext context)
        {
            Console.WriteLine(context.ApiActionDescriptor.Name + "准备请求");
            Console.WriteLine(context.RequestMessage.RequestUri);
            return base.OnBeginRequestAsync(context);
        }

        public override Task OnEndRequestAsync(ApiActionContext context)
        {
            Console.WriteLine(context.ApiActionDescriptor.Name + "请求完成");
            return base.OnEndRequestAsync(context);
        }
    }
}
