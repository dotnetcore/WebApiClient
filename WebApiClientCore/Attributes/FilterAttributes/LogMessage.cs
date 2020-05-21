using System;
using System.IO;
using System.Text;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示日志消息
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// 获取或设置是否记录请求
        /// </summary>
        public bool HasRequest { get; set; }

        /// <summary>
        /// 获取或设置请求时间
        /// </summary>
        public DateTime RequestTime { get; set; }

        /// <summary>
        /// 获取或设置请求头
        /// </summary>
        public string? RequestHeaders { get; set; }

        /// <summary>
        /// 获取或设置请求内容
        /// </summary>
        public string? RequestContent { get; set; }



        /// <summary>
        /// 获取或设置是否记录响应
        /// </summary>
        public bool HasResponse { get; set; }

        /// <summary>
        /// 获取或设置响应时间
        /// </summary>
        public DateTime ResponseTime { get; set; }

        /// <summary>
        /// 获取或设置响应头
        /// </summary>
        public string? ResponseHeaders { get; set; }

        /// <summary>
        /// 获取或设置响应内容
        /// </summary>
        public string? ResponseContent { get; set; }


        /// <summary>
        /// 获取或设置异常
        /// </summary>
        public Exception? Exception { get; set; }


        /// <summary>
        /// 返回不包含异常的日志消息
        /// </summary>
        /// <returns></returns>
        public LogMessage ToExcludeException()
        {
            return new LogMessage
            {
                HasRequest = this.HasRequest,
                HasResponse = this.HasResponse,
                RequestContent = this.RequestContent,
                RequestHeaders = this.RequestHeaders,
                RequestTime = this.RequestTime,
                ResponseContent = this.ResponseContent,
                ResponseHeaders = this.ResponseHeaders,
                ResponseTime = this.ResponseTime
            };
        }

        /// <summary>
        /// 转换为每行缩进的字符串
        /// </summary>
        /// <param name="spaceCount">缩进的空格数</param>
        /// <returns></returns>
        public string ToIndentedString(int spaceCount)
        {
            var message = this.ToString();
            var builder = new StringBuilder();
            var spaces = new string(' ', spaceCount);

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
        public override string ToString()
        {
            var builder = new TextBuilder();

            if (this.HasRequest == true)
            {
                builder
                    .AppendLine("[REQUEST]")
                    .AppendIfNotNull(this.RequestHeaders)
                    .AppendLineIf(this.RequestContent != null)
                    .AppendLineIfNotNull(this.RequestContent);
            }

            if (this.HasResponse == true)
            {
                builder
                    .AppendLineIfHasValue()
                    .AppendLine("[RESPONSE]")
                    .AppendIfNotNull(this.ResponseHeaders)
                    .AppendLineIf(this.ResponseContent != null)
                    .AppendLineIfNotNull(this.ResponseContent);
            }

            if (this.Exception != null)
            {
                builder
                    .AppendLineIfHasValue()
                    .AppendLine($"[EXCEPTION]")
                    .AppendLine(this.Exception.ToString());
            }

            return builder
                .AppendLineIfHasValue()
                .Append($"[ELAPSED] {this.ResponseTime.Subtract(this.RequestTime)}")
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
            /// 如果条件满足则添加换行
            /// </summary>
            /// <param name="predicate"></param>
            /// <returns></returns>
            public TextBuilder AppendLineIf(bool predicate)
            {
                if (predicate == true)
                {
                    this.builder.AppendLine();
                }
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
            public TextBuilder AppendIfNotNull(string? value)
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
            public TextBuilder AppendLineIfNotNull(string? value)
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
