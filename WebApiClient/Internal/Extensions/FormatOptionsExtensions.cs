using Newtonsoft.Json;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 提供FormatOptions的扩展
    /// </summary>
    static class FormatOptionsExtensions
    {
        /// <summary>
        /// 转换为序列化配置项     
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <param name="formatScope">序列化范围</param>
        /// <returns></returns>
        public static JsonSerializerSettings ToSerializerSettings(this FormatOptions options, FormatScope formatScope)
        {
            var useCamelCase = options == null ? false : options.UseCamelCase;
            var contractResolver = AnnotationsContractResolver.GetResolver(formatScope, useCamelCase);
            var settings = new JsonSerializerSettings { ContractResolver = contractResolver };

            if (options != null)
            {
                settings.DateFormatString = options.DateTimeFormat;
                settings.NullValueHandling = options.IgnoreNullProperty ? NullValueHandling.Ignore : NullValueHandling.Include;
            }
            return settings;
        }
    }
}
