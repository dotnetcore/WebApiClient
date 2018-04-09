using System.Net.Http;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用JsonFormatter序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 将参数值作为application/json请求
        /// </summary>
        public JsonContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 将参数体作为application/json请求
        /// </summary>
        /// <param name="datetimeFormat">日期时间格式</param>
        public JsonContentAttribute(string datetimeFormat)
        {
            this.DateTimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected override void SetHttpContent(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var formatter = context.HttpApiConfig.JsonFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var json = formatter.Serialize(parameter.Value, options);
            context.RequestMessage.Content = new JsonContent(json, Encoding.UTF8);
        }
    }
}
