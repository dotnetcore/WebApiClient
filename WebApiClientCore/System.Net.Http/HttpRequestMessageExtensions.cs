using System.Text;

namespace System.Net.Http
{
    /// <summary>
    /// HttpRequestMessage扩展
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// 读取请求头
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetHeadersString(this HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var builder = new StringBuilder();

            if (uri != null && uri.IsAbsoluteUri)
            {
                const string host = "Host";
                builder.AppendLine($"{request.Method} {uri.PathAndQuery} HTTP/{request.Version}");

                if (request.Headers.Contains(host) == false)
                {
                    builder.AppendLine($"{host}: {uri.Authority}");
                }
            }

            builder.Append(request.Headers.ToString());
            if (request.Content != null)
            {
                builder.Append(request.Content.Headers.ToString());
            }

            return builder.ToString();
        }
    }
}
