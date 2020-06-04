using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute, IEncodingable
    {
        /// <summary>
        /// utf8编码
        /// </summary>
        private static readonly Encoding utf8 = System.Text.Encoding.UTF8;

        /// <summary>
        /// 编码方式
        /// </summary>
        private Encoding encoding = utf8;

        /// <summary>
        /// 获取或设置编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string Encoding
        {
            get => this.encoding.WebName;
            set => this.encoding = System.Text.Encoding.GetEncoding(value);
        }

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            if (this.encoding.CodePage == utf8.CodePage)
            {
                var jsonContent = new JsonContent();
                context.HttpContext.RequestMessage.Content = jsonContent;
                context.SerializeToJson(jsonContent);
            }
            else
            {
                var utf8Json = context.SerializeToJson();
                var jsonText = utf8.GetString(utf8Json) ?? string.Empty;
                var jsonContent = new StringContent(jsonText, this.encoding, JsonContent.MediaType);
                context.HttpContext.RequestMessage.Content = jsonContent;
            }

            return Task.CompletedTask;
        }
    }
}
