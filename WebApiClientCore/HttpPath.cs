using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http路径
    /// </summary>
    public abstract class HttpPath
    {
        /// <summary>
        /// 合成Uri
        /// </summary>
        /// <param name="baseUri">基础uri</param>
        /// <returns></returns>
        public abstract Uri? MakeUri(Uri? baseUri);

        /// <summary>
        /// 创建HttpPath实例
        /// </summary>
        /// <param name="pathString">http路径</param>
        /// <exception cref="UriFormatException"></exception>
        /// <returns></returns>
        public static HttpPath Create(string? pathString)
        {
            if (string.IsNullOrEmpty(pathString))
            {
                return NullPath.Instance;
            }

            var path = new Uri(pathString, UriKind.RelativeOrAbsolute);
            if (path.IsAbsoluteUri == true)
            {
                return new AbsolutePath(path);
            }

            return new RelativePath(path);
        }

        /// <summary>
        /// 空路径
        /// </summary>
        private sealed class NullPath : HttpPath
        {
            /// <summary>
            /// 获取实例
            /// </summary>
            public static HttpPath Instance { get; } = new NullPath();

            public override Uri? MakeUri(Uri? baseUri)
            {
                return baseUri;
            }

            public override string ToString()
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 绝对路径
        /// </summary>
        private sealed class AbsolutePath : HttpPath
        {
            private Uri path;

            public AbsolutePath(Uri path)
            {
                this.path = path;
            }

            public override Uri? MakeUri(Uri? baseUri)
            {
                return this.path;
            }

            public override string ToString()
            {
                return this.path.ToString();
            }
        }

        /// <summary>
        /// 相对路径
        /// </summary>
        private sealed class RelativePath : HttpPath
        {
            private Uri path;

            public RelativePath(Uri path)
            {
                this.path = path;
            }

            public override Uri? MakeUri(Uri? baseUri)
            {
                if (baseUri == null)
                {
                    return this.path;
                }

                if (baseUri.IsAbsoluteUri)
                {
                    return new Uri(baseUri, this.path);
                }

                return this.path;
            }

            public override string ToString()
            {
                return this.path.ToString();
            }
        }
    }
}
