using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示xml内容的模型结果
    /// </summary>
    public class XmlResultAttribute : StrongTypeResultAttribute
    {
        /// <summary>
        /// xml内容的强类型模型结果特性
        /// </summary>
        public XmlResultAttribute()
            : base(XmlContent.MediaType)
        {
        }

        /// <summary>
        /// 设置强类型模型结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override async Task SetStrongTypeResultAsync(ApiActionContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var dataType = context.ApiAction.Return.DataType.Type;

            var formatter = context.HttpContext.Services.GetRequiredService<IXmlFormatter>();
            var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            context.Result = formatter.Deserialize(xml, dataType);
        }
    }
}
