using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClientCore.Masking;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将请求和响应内容脱敏后再输出为日志的过滤器
    /// 脱敏的字段由方法上的 <see cref="MaskFieldAttribute"/> 声明
    /// </summary>
    /// <remarks>
    /// <para>• 使用的策略优先从服务容器中的 <see cref="MaskStrategyRegistry"/> 获取</para>
    /// <para>• 服务容器中未注册时使用 <see cref="MaskStrategyRegistry.Default"/></para>
    /// </remarks>
    public class MaskedLoggingFilterAttribute : LoggingFilterAttribute
    {
        /// <summary>
        /// 写日志到指定日志组件
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="logMessage">日志消息</param>
        /// <returns></returns>
        protected override Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
        {
            var declarations = MaskDeclarationCache.Get(context.ActionDescriptor.Member);
            if (declarations.Length == 0)
            {
                return base.WriteLogAsync(context, logMessage);
            }

            var registry = context.HttpContext.ServiceProvider.GetService<MaskStrategyRegistry>()
                ?? MaskStrategyRegistry.Default;

            var rules = new MaskRule[declarations.Length];
            for (var i = 0; i < declarations.Length; i++)
            {
                var declaration = declarations[i];
                rules[i] = new MaskRule(
                    declaration.FieldName,
                    registry.Get(declaration.StrategyName),
                    declaration.IgnoreCase);
            }

            logMessage.RequestContent = JsonMasker.Mask(logMessage.RequestContent, rules);
            logMessage.ResponseContent = JsonMasker.Mask(logMessage.ResponseContent, rules);

            return base.WriteLogAsync(context, logMessage);
        }

        /// <summary>
        /// 表示字段的脱敏声明
        /// </summary>
        private sealed class MaskDeclaration
        {
            public string FieldName { get; }
            public string StrategyName { get; }
            public bool IgnoreCase { get; }

            public MaskDeclaration(string fieldName, string strategyName, bool ignoreCase)
            {
                this.FieldName = fieldName;
                this.StrategyName = strategyName;
                this.IgnoreCase = ignoreCase;
            }
        }

        /// <summary>
        /// 表示脱敏声明的缓存
        /// </summary>
        private static class MaskDeclarationCache
        {
            private static readonly ConcurrentDictionary<MethodInfo, MaskDeclaration[]> cache = new();

            /// <summary>
            /// 获取方法的脱敏声明
            /// </summary>
            /// <param name="method">接口方法</param>
            /// <returns></returns>
            public static MaskDeclaration[] Get(MethodInfo method)
            {
                return cache.GetOrAdd(method, Create);
            }

            /// <summary>
            /// 创建方法的脱敏声明
            /// </summary>
            /// <param name="method">接口方法</param>
            /// <returns></returns>
            private static MaskDeclaration[] Create(MethodInfo method)
            {
                return method
                    .GetCustomAttributes<MaskFieldAttribute>(inherit: true)
                    .Select(item => new MaskDeclaration(item.FieldName, item.StrategyName, item.IgnoreCase))
                    .ToArray();
            }
        }
    }
}
