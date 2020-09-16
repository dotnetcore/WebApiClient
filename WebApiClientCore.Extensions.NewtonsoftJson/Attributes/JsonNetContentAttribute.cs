using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.Extensions.NewtonsoftJson;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用Json.Net序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary> 
    public class JsonNetContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string CharSet
        {
            get => this.encoding.WebName;
            set => this.encoding = Encoding.GetEncoding(value);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var name = context.HttpContext.OptionsName;
            var options = context.HttpContext.ServiceProvider.GetService<IOptionsMonitor<JsonNetSerializerOptions>>().Get(name);
            var json = context.ParameterValue == null
                ? string.Empty
                : JsonConvert.SerializeObject(context.ParameterValue, options.JsonSerializeOptions);

            var jsonContent = new StringContent(json ?? string.Empty, this.encoding, JsonContent.MediaType);
            context.HttpContext.RequestMessage.Content = jsonContent;
            return Task.CompletedTask;
        }
    }
}
