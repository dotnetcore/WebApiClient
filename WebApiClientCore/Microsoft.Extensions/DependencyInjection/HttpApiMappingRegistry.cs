using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// httpApi映射注册器
    /// </summary>
    class HttpApiMappingRegistry
    {
        /// <summary>
        /// 类型与名称映射
        /// </summary>
        public Dictionary<string, Type> NamedHttpApiRegistrations { get; } = new Dictionary<string, Type>();
    }
}
