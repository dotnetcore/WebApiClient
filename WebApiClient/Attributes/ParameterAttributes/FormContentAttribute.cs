using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对作为x-www-form-urlencoded请求
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 时期时间格式
        /// </summary>
        private readonly string datetimeFormat;

        /// <summary>
        /// 将参数值作为x-www-form-urlencoded请求
        /// </summary>
        public FormContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 将参数值作为x-www-form-urlencoded请求
        /// </summary>
        /// <param name="datetimeFormat">时期时间格式</param>
        public FormContentAttribute(string datetimeFormat)
        {
            this.datetimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override async Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.KeyValueFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.datetimeFormat);
            var keyValues = formatter.Serialize(parameter, options);
            await context.RequestMessage.AddFormFieldAsync(keyValues);
        }
    }
}
