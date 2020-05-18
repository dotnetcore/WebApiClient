using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值作为multipart/form-data表单
    /// </summary>
    public class FormDataContentAttribute : HttpContentAttribute, ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var keyValues = context.SerializeToKeyValues().CollectAs(this.CollectionFormat);
            context.HttpContext.RequestMessage.AddFormDataText(keyValues);
            return Task.CompletedTask;
        }
    }
}
