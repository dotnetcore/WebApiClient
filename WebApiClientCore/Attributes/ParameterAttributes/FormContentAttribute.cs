using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueSerializer序列化参数值得到的键值对作为x-www-form-urlencoded表单
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute, ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        protected override async Task SetHttpContentAsync(ApiParameterContext context)
        {
            var keyValues = this.SerializeToKeyValues(context);
            await context.HttpContext.RequestMessage.AddFormFieldAsync(keyValues).ConfigureAwait(false);
        }

        /// <summary>
        /// 序列化参数为keyValue
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValue> SerializeToKeyValues(ApiParameterContext context)
        {
            return context.SerializeToKeyValues().CollectAs(this.CollectionFormat);
        }
    }
}
