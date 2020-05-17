namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值作为multipart/form-data表单
    /// </summary>
    public class FormDataContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        protected override void SetHttpContent(ApiParameterContext context)
        {
            var keyValues = context.SerializeParameterToKeyValues();
            context.HttpContext.RequestMessage.AddFormDataText(keyValues);
        }
    }
}
