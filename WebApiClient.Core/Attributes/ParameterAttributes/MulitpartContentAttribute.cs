using System.Linq;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对分别作为multipart/form-data表单的一个文本项 
    /// </summary>
    public class MulitpartContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 将参数值作为multipart/form-data表单或表单的一个项
        /// </summary>
        public MulitpartContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 将参数值作为multipart/form-data表单或表单的一个项
        /// </summary>
        /// <param name="datetimeFormat">时期时间格式</param>
        public MulitpartContentAttribute(string datetimeFormat)
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
            if (this.WillIgnore(parameter.Value) == true)
            {
                return;
            }
            var formatter = context.HttpApiConfig.KeyValueFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var keyValues = formatter.Serialize(parameter, options);
            context.RequestMessage.AddMulitpartText(keyValues);
        }

        /// <summary>
        /// 返回是否应该忽略提交 
        /// </summary>
        /// <param name="val">值</param>
        /// <returns></returns>
        private bool WillIgnore(object val)
        {
            return this.IgnoreWhenNull == true && val == null;
        }
    }
}
