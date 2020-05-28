using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using WebApiClientCore.Serialization;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api参数上下文
    /// </summary>
    public class ApiParameterContext : ApiRequestContext
    {
        /// <summary>
        /// 参数索引
        /// </summary>
        private readonly int index;

        /// <summary>
        /// 获取参数描述
        /// </summary>
        public ApiParameterDescriptor Parameter => this.ApiAction.Parameters[index];

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object? ParameterValue => this.Arguments[index];

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameterIndex">参数索引</param>
        public ApiParameterContext(ApiRequestContext context, int parameterIndex)
            : base(context.HttpContext, context.ApiAction, context.Arguments, context.Properties, context.CancellationTokens)
        {
            this.index = parameterIndex;
        }

        /// <summary>
        /// 序列化参数值为Json
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        public void SerializeToJson(IBufferWriter<byte> bufferWriter)
        {
            var options = this.HttpContext.Options.JsonSerializeOptions;
            this.HttpContext.Services
                .GetRequiredService<IJsonSerializer>()
                .Serialize(bufferWriter, this.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为Json
        /// </summary>
        /// <returns></returns>
        public byte[] SerializeToJson()
        {
            var options = this.HttpContext.Options.JsonSerializeOptions;
            return this.HttpContext.Services
                .GetRequiredService<IJsonSerializer>()
                .Serialize(this.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为Xml
        /// </summary>
        /// <param name="encoding">xml编码</param>
        /// <returns></returns>
        public string? SerializeToXml(Encoding encoding)
        {
            return this.HttpContext.Services
                .GetRequiredService<IXmlSerializer>()
                .Serialize(this.ParameterValue, encoding);
        }

        /// <summary>
        /// 序列化参数值为键值对
        /// </summary>
        /// <returns></returns>
        public IList<KeyValue> SerializeToKeyValues()
        {
            var options = this.HttpContext.Options.KeyValueSerializeOptions;
            return this.HttpContext.Services
                .GetRequiredService<IKeyValueSerializer>()
                .Serialize(this.Parameter.Name, this.ParameterValue, options);
        }
    }
}
