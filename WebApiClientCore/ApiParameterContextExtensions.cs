using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebApiClientCore.Internals;
using WebApiClientCore.Serialization;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供ApiParameterContext的扩展
    /// </summary>
    public static class ApiParameterContextExtensions
    {
        /// <summary>
        /// 序列化参数值为utf8编码的Json
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static byte[] SerializeToJson(this ApiParameterContext context)
        {
            return context.SerializeToJson(Encoding.UTF8);
        }

        /// <summary>
        /// 序列化参数值为指定编码的Json
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public static byte[] SerializeToJson(this ApiParameterContext context, Encoding encoding)
        {
            using var bufferWriter = new RecyclableBufferWriter<byte>();
            context.SerializeToJson(bufferWriter);

            if (Encoding.UTF8.Equals(encoding) == true)
            {
                return bufferWriter.WrittenSpan.ToArray();
            }
            else
            {
                var utf8Json = bufferWriter.WrittenSegment;
                return Encoding.Convert(Encoding.UTF8, encoding, utf8Json.Array, utf8Json.Offset, utf8Json.Count);
            }
        }

        /// <summary>
        /// 序列化参数值为utf8编码的Json
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bufferWriter">buffer写入器</param>
        public static void SerializeToJson(this ApiParameterContext context, IBufferWriter<byte> bufferWriter)
        {
            var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
            JsonBufferSerializer.Serialize(bufferWriter, context.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为Xml
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding">xml的编码</param>
        /// <returns></returns>
        public static string? SerializeToXml(this ApiParameterContext context, Encoding? encoding)
        {
            var options = context.HttpContext.HttpApiOptions.XmlSerializeOptions;
            if (encoding != null && encoding.Equals(options.Encoding) == false)
            {
                options = options.Clone();
                options.Encoding = encoding;
            }

            return XmlSerializer.Serialize(context.ParameterValue, options);
        }

        /// <summary>
        /// 序列化参数值为键值对
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IList<KeyValue> SerializeToKeyValues(this ApiParameterContext context)
        {
            var options = context.HttpContext.HttpApiOptions.KeyValueSerializeOptions;
            return KeyValueSerializer.Serialize(context.ParameterName, context.ParameterValue, options);
        }

        readonly static Regex paramterTemplate = new Regex(@"\{([^\}]+)\}");

        /// <summary>
        /// 动态参数格式化
        /// </summary>
        public static string ParameterFormat(this ApiRequestContext context, string template)
        {
            foreach (Match item in paramterTemplate.Matches(template).Cast<Match>())
            {
                if (context.TryGetArgument<string>(item.Groups[1].Value, out var t1))
                {
                    template = template.Replace(item.Groups[0].Value, t1);
                }
            }
            return template;
        }
    }
}
