using System;
using System.IO;
using System.Text;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示追踪到的消息
    /// </summary>
    public class TraceMessage
    {
        /// <summary>
        /// 获取或设置是否有请求内容
        /// </summary>
        public bool HasRequest { get; set; }

        /// <summary>
        /// 获取或设置是否有响应内容
        /// </summary>
        public bool HasResponse { get; set; }

        /// <summary>
        /// 获取或设置请求头
        /// </summary>
        public string RequestHeaders { get; set; }

        /// <summary>
        /// 获取或设置请求内容
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// 获取或设置请求时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// 获取或设置响应头
        /// </summary>
        public string ResponseHeaders { get; set; }

        /// <summary>
        /// 获取或设置响应内容
        /// </summary>
        public string ResponseContent { get; set; }

        /// <summary>
        /// 获取或设置响应时间
        /// </summary>
        public DateTime ResponseTime { get; set; }

        /// <summary>
        /// 获取或设置异常
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 转换为每行缩进的字符串
        /// </summary>
        /// <param name="spaceCount">缩进的空格数</param>
        /// <returns></returns>
        public string ToIndentedString(int spaceCount)
        {
            return this.ToIndentedString(spaceCount, includeException: true);
        }

        /// <summary>
        /// 转换为每行缩进的字符串
        /// </summary>
        /// <param name="spaceCount">缩进的空格数</param>
        /// <param name="includeException">是否包含异常消息</param>
        /// <returns></returns>
        public string ToIndentedString(int spaceCount, bool includeException)
        {
            var builder = new StringBuilder();
            var spaces = new string(' ', spaceCount);
            var message = this.ToString(includeException);

            using (var reader = new StringReader(message))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Length == 0)
                    {
                        builder.AppendLine();
                    }
                    else
                    {
                        builder.AppendLine($"{spaces}{line}");
                    }
                    line = reader.ReadLine();
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public sealed override string ToString()
        {
            return this.ToString(includeException: true);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="includeException">是否包含异常消息</param>
        /// <returns></returns>
        public virtual string ToString(bool includeException)
        {
            var builder = new TextBuilder();
            const string timeFormat = "yyyy-MM-dd HH:mm:ss.fff";

            if (this.HasRequest == true)
            {
                builder
                    .AppendLine($"[REQUEST]{this.RequestTime.ToString(timeFormat)}")
                    .AppendIfNotNull(this.RequestHeaders)
                    .AppendLineIfNotNull(this.RequestContent);
            }

            if (this.HasResponse == true)
            {
                builder
                    .AppendLineIfHasValue()
                    .AppendLine($"[RESPONSE]{this.ResponseTime.ToString(timeFormat)}")
                    .AppendIfNotNull(this.ResponseHeaders)
                    .AppendLineIfNotNull(this.ResponseContent);
            }

            if (includeException == true && this.Exception != null)
            {
                builder
                    .AppendLineIfHasValue()
                    .AppendLine($"[EXCEPTION]")
                    .AppendLine(this.Exception.ToString());
            }

            return builder
                .AppendLineIfHasValue()
                .Append($"[TIMESPAN]{this.ResponseTime.Subtract(this.RequestTime)}")
                .ToString();
        }

        /// <summary>
        /// 表示文本创建器
        /// </summary>
        private class TextBuilder
        {
            /// <summary>
            /// StringBuilder
            /// </summary>
            private readonly StringBuilder builder = new StringBuilder();

            /// <summary>
            /// 添加文本
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public TextBuilder Append(string value)
            {
                this.builder.Append(value);
                return this;
            }

            /// <summary>
            /// 添加文本并换行
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public TextBuilder AppendLine(string value)
            {
                this.builder.AppendLine(value);
                return this;
            }

            /// <summary>
            /// 如果已经有内容则添加换行
            /// </summary>
            /// <returns></returns>
            public TextBuilder AppendLineIfHasValue()
            {
                if (this.builder.Length > 0)
                {
                    this.builder.AppendLine();
                }
                return this;
            }

            /// <summary>
            /// 如果不为空则添加内容
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public TextBuilder AppendIfNotNull(string value)
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    this.builder.Append(value);
                }
                return this;
            }

            /// <summary>
            /// 如果不为空则添加内容并换行
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public TextBuilder AppendLineIfNotNull(string value)
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    this.builder.AppendLine(value);
                }
                return this;
            }

            /// <summary>
            /// 转换为字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return this.builder.ToString();
            }
        }
    }
}
