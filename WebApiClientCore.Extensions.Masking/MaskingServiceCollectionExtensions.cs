using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WebApiClientCore.Masking;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供脱敏的依赖注入扩展
    /// </summary>
    public static class MaskingServiceCollectionExtensions
    {
        /// <summary>
        /// 注册脱敏策略注册表
        /// 注册后 <see cref="WebApiClientCore.Attributes.MaskedLoggingFilterAttribute"/> 会优先使用注册表中的策略
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">脱敏配置</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddMasking(this IServiceCollection services, Action<MaskingOptions>? configure = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new MaskingOptions();
            configure?.Invoke(options);
            services.TryAddSingleton(options.Strategies);
            return services;
        }
    }
}
