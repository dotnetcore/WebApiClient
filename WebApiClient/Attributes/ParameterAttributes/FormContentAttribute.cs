using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对作为x-www-form-urlencoded请求
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute, IIgnoreWhenNullable, IDateTimeFormatable
    {
        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 序列化参数值得到的键值对作为x-www-form-urlencoded请求
        /// </summary>
        public FormContentAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 序列化参数值得到的键值对作为x-www-form-urlencoded请求
        /// </summary>
        /// <param name="datetimeFormat">时期时间格式</param>
        public FormContentAttribute(string datetimeFormat)
        {
            this.DateTimeFormat = datetimeFormat;
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        protected sealed override async Task SetHttpContentAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            if (this.IgnoreWhenNull(parameter) == true)
            {
                return;
            }

            var formatter = context.HttpApiConfig.KeyValueFormatter;
            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var keyValues = formatter.Serialize(parameter, options);
            var form = this.HandleForm(keyValues);
            await context.RequestMessage.AddFormFieldAsync(form).ConfigureAwait(false);
        }

        /// <summary>
        /// 处理表单内容
        /// 可以重写比方法
        /// 实现字段排序、插入签名字段等
        /// </summary>
        /// <param name="form">表单</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> HandleForm(IEnumerable<KeyValuePair<string, string>> form)
        {
            return form;
        }
    }
}
