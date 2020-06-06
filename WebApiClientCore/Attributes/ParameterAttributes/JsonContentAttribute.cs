using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute, ICharSetable
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
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var jsonContent = new JsonContent();
            context.HttpContext.RequestMessage.Content = jsonContent;
            jsonContent.Headers.ContentType.CharSet = this.encoding.WebName;

            if (Encoding.UTF8.Equals(this.encoding) == true)
            {
                context.SerializeToJson(jsonContent);
            }
            else
            {
                var json = context.SerializeToJson(this.encoding);
                jsonContent.Write(json);
            }
            return Task.CompletedTask;
        }
    }
}
