using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用JsonSerializer序列化参数值得到的json文本作为application/json请求
    /// 每个Api只能注明于其中的一个参数
    /// </summary>
    public class JsonContentAttribute : HttpContentAttribute
    {
        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected override Task SetHttpContentAsync(ApiParameterContext context)
        {
            var bufferWriter = new BufferWriter<byte>();
            try
            {
                context.SerializeToJson(bufferWriter);
                context.HttpContext.RequestMessage.Content = new JsonContent(bufferWriter);
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                // 如果遇到异常则回收bufferWriter
                bufferWriter.Dispose();
                throw;
            }
        }
    }
}
