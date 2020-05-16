using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对作为x-www-form-urlencoded表单
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
            var form = this.TransformForm(keyValues);
            await context.HttpContext.RequestMessage.AddFormFieldAsync(form).ConfigureAwait(false);
        }

        /// <summary>
        /// 变换表单内容      
        /// 比如可以实现字段排序、插入签名字段、相同key的字段合并等
        /// </summary>
        /// <param name="form">表单</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValue> TransformForm(IEnumerable<KeyValue> form)
        {
            return form;
        }
    }
}
