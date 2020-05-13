using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对作为x-www-form-urlencoded请求
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        protected sealed override async Task SetHttpContentAsync(ApiParameterContext context)
        {
            var keyValues = context.SerializeToKeyValues();
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
        protected virtual IEnumerable<KeyValue> HandleForm(IEnumerable<KeyValue> form)
        {
            return form;
        }
    }
}
