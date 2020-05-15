namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对分别作为multipart/form-data表单的一个文本项 
    /// </summary>
    public class FormDataContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        protected override void SetHttpContent(ApiParameterContext context)
        {
            var keyValues = context.SerializeToKeyValues();
            context.HttpContext.RequestMessage.AddFormDataText(keyValues);
        }
    }
}
