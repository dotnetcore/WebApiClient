using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示自动处理多种结果的特性
    /// 支持json模型、xml模型、string、byte[]、Stream和HttpResponseMessage
    /// </summary>
    public class AutoResultAttribute : ApiResultAttribute
    {
        /// <summary>
        /// 配置请求头的accept
        /// </summary>
        /// <param name="accept">请求头的accept</param>
        protected override void ConfigureAccept(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept)
        {
            accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.1d));
            accept.Add(new MediaTypeWithQualityHeaderValue(JsonContent.MediaType, 0.6d));
            accept.Add(new MediaTypeWithQualityHeaderValue(XmlContent.MediaType, 0.3d));
        }

        /// <summary>
        /// 设置结果值
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override async Task SetResultAsync(ApiActionContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var dataType = context.ApiAction.Return.DataType;

            if (dataType.IsHttpResponseMessage == true)
            {
                context.Result = response;
            }
            else if (dataType.IsString == true)
            {
                context.Result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else if (dataType.IsByteArray == true)
            {
                context.Result = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            else if (dataType.IsStream == true)
            {
                context.Result = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            else
            {
                var contentType = new ContentType(response.Content.Headers.ContentType);
                if (contentType.IsApplicationJson() == true)
                {
                    var formatter = context.HttpContext.Services.GetRequiredService<IJsonFormatter>();
                    var json = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    var options = context.HttpContext.Options.JsonDeserializeOptions;
                    context.Result = formatter.Deserialize(json, dataType.Type, options);
                }
                if (contentType.IsApplicationXml() == true)
                {
                    var formatter = context.HttpContext.Services.GetRequiredService<IXmlFormatter>();
                    var xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    context.Result = formatter.Deserialize(xml, dataType.Type);
                }
            }
        }
    }
}
