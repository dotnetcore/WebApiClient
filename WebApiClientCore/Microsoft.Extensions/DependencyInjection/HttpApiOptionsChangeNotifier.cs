using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 表示HttpApiOptions变化通知者
    /// </summary>
    class HttpApiOptionsChangeNotifier : IHttpApiOptionsChangeNotifier
    {
        /// <summary>
        /// 所有变化通知者
        /// </summary>
        private readonly IHttpApiOptionsChangeNotifier[] changeNotifers;

        /// <summary>
        /// HttpApiOptions变化通知者
        /// </summary>
        /// <param name="changeTokenSources"></param>
        public HttpApiOptionsChangeNotifier(IEnumerable<IOptionsChangeTokenSource<HttpApiOptions>> changeTokenSources)
        {
            this.changeNotifers = changeTokenSources.OfType<IHttpApiOptionsChangeNotifier>().ToArray();
        }

        /// <summary>
        /// 通知HttpApiOptions变化
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        public void NotifyChanged(Type httpApiType)
        {
            foreach (var item in this.changeNotifers)
            {
                item.NotifyChanged(httpApiType);
            }
        }
    }
}
