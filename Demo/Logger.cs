using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace Demo
{
    /// <summary>
    /// 记录日志
    /// </summary>
    class Logger : ApiActionFilterAttribute
    {
        public override Task OnBeginRequestAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            Console.WriteLine("{0} {1} {2}", DateTime.Now.ToString("HH:mm:ss.fff"), request.Method, request.RequestUri);
            return base.OnBeginRequestAsync(context);
        }

        public override Task OnEndRequestAsync(ApiActionContext context)
        {
            var request = context.RequestMessage;
            Console.WriteLine("{0} {1} {2}完成", DateTime.Now.ToString("HH:mm:ss.fff"), request.Method, request.RequestUri.AbsolutePath);
            return base.OnEndRequestAsync(context);
        }
    }
}
