using System.Buffers;
using System.Collections.Generic;
using System.Text;

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
            : base(context.HttpContext, context.ApiAction, context.Arguments, context.Properties)
        {
            this.index = parameterIndex;
        }

        /// <summary>
        /// 序列化参数值为utf8编码的Json
        /// </summary>
        /// <returns></returns>
        public byte[] SerializeToJson()
        {
            using var bufferWriter = new BufferWriter<byte>();
            this.SerializeToJson(bufferWriter);
            return bufferWriter.GetWrittenSpan().ToArray();
        }

        /// <summary>
        /// 序列化参数值为utf8编码的Json
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        public void SerializeToJson(IBufferWriter<byte> bufferWriter)
        {
            var options = this.HttpContext.HttpApiOptions.JsonSerializeOptions;
            this.HttpContext
                .ServiceProvider
                .GetJsonSerializer()
                .Serialize(bufferWriter, this.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为Xml
        /// </summary>
        /// <param name="encoding">xml的编码</param>
        /// <returns></returns>
        public string? SerializeToXml(Encoding? encoding)
        {
            var options = this.HttpContext.HttpApiOptions.XmlSerializeOptions;
            if (encoding != null && encoding.CodePage != options.Encoding.CodePage)
            {
                options = options.Clone();
                options.Encoding = encoding;
            }

            return this.HttpContext
                .ServiceProvider
                .GetXmlSerializer()
                .Serialize(this.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为键值对
        /// </summary>
        /// <returns></returns>
        public IList<KeyValue> SerializeToKeyValues()
        {
            var options = this.HttpContext.HttpApiOptions.KeyValueSerializeOptions;
            return this.HttpContext
                .ServiceProvider
                .GetKeyValueSerializer()
                .Serialize(this.Parameter.Name, this.ParameterValue, options);
        }
    }
}
