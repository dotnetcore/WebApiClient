using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApiClientCore;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 表示HttpApiOptions的Action配置触发器
    /// </summary>
    class HttpApiOptionsConfigureTrigger : IHttpApiOptionsConfigureTrigger
    {
        /// <summary>
        /// 所有触发器
        /// </summary>
        private readonly IHttpApiOptionsConfigureTrigger[] configureTriggers;

        /// <summary>
        /// HttpApiOptions的Action配置触发器
        /// </summary>
        /// <param name="changeTokenSources"></param>
        public HttpApiOptionsConfigureTrigger(IEnumerable<IOptionsChangeTokenSource<HttpApiOptions>> changeTokenSources)
        {
            this.configureTriggers = changeTokenSources.OfType<IHttpApiOptionsConfigureTrigger>().ToArray();
        }

        /// <summary>
        /// 触发HttpApiOptions的Action配置
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        public void Raise(Type httpApiType)
        {
            foreach (var item in this.configureTriggers)
            {
                item.Raise(httpApiType);
            }
        }
    }
}
