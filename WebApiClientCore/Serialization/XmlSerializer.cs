using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// Xml序列化工具
    /// </summary>
    public static class XmlSerializer
    {
        private static readonly XmlReaderSettings readerSettings = new();
        private static readonly XmlWriterSettings writerSettings = new();

        /// <summary>
        /// 将对象序列化为Xml文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置选项</param> 
        /// <returns></returns>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static string? Serialize(object? obj, XmlWriterSettings? options)
        {
            if (obj == null)
            {
                return null;
            }

            var settings = options ?? writerSettings;
            using var writer = new EncodingStringWriter(settings.Encoding);
            SerializeCore(writer, obj, settings);
            return writer.ToString();
        }

        /// <summary>
        /// 将对象序列化为Xml文本
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param> 
        /// <param name="obj">对象</param>
        /// <param name="options">配置选项</param> 
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static void Serialize(IBufferWriter<byte> bufferWriter, object? obj, XmlWriterSettings? options)
        {
            if (obj == null)
            {
                return;
            }

            var settings = options ?? writerSettings;
            using var writer = new BufferTextWriter(bufferWriter, settings.Encoding);
            SerializeCore(writer, obj, settings);
        }

        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        private static void SerializeCore(TextWriter textWriter, object obj, XmlWriterSettings settings)
        {
            using var xmlWriter = XmlWriter.Create(textWriter, settings);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            xmlSerializer.Serialize(xmlWriter, obj);
        }

        /// <summary>
        /// 将Xml文本反序列化对象
        /// </summary>
        /// <param name="xml">xml文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">配置选项</param>
        /// <returns></returns>
        [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
        public static object? Deserialize(string? xml, Type objType, XmlReaderSettings? options)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (string.IsNullOrEmpty(xml))
            {
                return objType.DefaultValue();
            }

            var settings = options ?? readerSettings;
            using var reader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(reader, settings);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(objType);
            return xmlSerializer.Deserialize(xmlReader);
        }


        private class BufferTextWriter : TextWriter
        {
            private readonly IBufferWriter<byte> bufferWriter;

            public override Encoding Encoding { get; }

            public BufferTextWriter(IBufferWriter<byte> bufferWriter, Encoding encoding)
            {
                this.bufferWriter = bufferWriter;
                this.Encoding = encoding;
            }
            public override Task FlushAsync()
            {
                return Task.CompletedTask;
            }

            public override void Write(ReadOnlySpan<char> buffer)
            {
                this.Encoding.GetBytes(buffer, this.bufferWriter);
            }

            public override void Write(char value)
            {
                Span<char> buffer = [value];
                this.Write(buffer);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                this.Write(buffer.AsSpan(index, count));
            }
            public override void Write(string? value)
            {
                this.Write(value.AsSpan());
            }

            public override Task WriteAsync(string? value)
            {
                this.Write(value.AsSpan());
                return Task.CompletedTask;
            }

            public override Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
            {
                this.Write(buffer.Span);
                return Task.CompletedTask;
            }

            public override Task WriteAsync(char value)
            {
                Span<char> buffer = [value];
                this.Write(buffer);
                return Task.CompletedTask;
            }

            public override Task WriteAsync(char[] buffer, int index, int count)
            {
                this.Write(buffer.AsSpan(index, count));
                return Task.CompletedTask;
            }

            public override void WriteLine(ReadOnlySpan<char> buffer)
            {
                this.Write(buffer);
                WriteLine();
            }

            public override Task WriteLineAsync(string? value)
            {
                this.Write(value.AsSpan());
                WriteLine();
                return Task.CompletedTask;
            }
            public override Task WriteLineAsync(char value)
            {
                Span<char> buffer = [value];
                this.Write(buffer);
                WriteLine();
                return Task.CompletedTask;
            }

            public override Task WriteLineAsync(char[] buffer, int index, int count)
            {
                this.Write(buffer.AsSpan(0, count));
                WriteLine();
                return Task.CompletedTask;
            }

            public override Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default)
            {
                this.Write(buffer.Span);
                WriteLine();
                return Task.CompletedTask;
            }
        }


        /// <summary>
        /// 表示可指定编码文本写入器
        /// </summary>
        private class EncodingStringWriter : StringWriter
        {
            /// <summary>
            /// 获取编码
            /// </summary>
            public override Encoding Encoding { get; }

            /// <summary>
            /// 可指定编码文本写入器
            /// </summary>
            /// <param name="encoding">编码</param>
            public EncodingStringWriter(Encoding encoding)
            {
                this.Encoding = encoding;
            }
        }
    }
}
