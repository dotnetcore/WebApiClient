#if NETSTANDARD2_1
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// HttpContent扩展
    /// </summary>
    static class HttpContentExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<byte[]> ReadAsByteArrayAsync(this HttpContent httpContent, CancellationToken _)
        {
            return httpContent.ReadAsByteArrayAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Stream> ReadAsStreamAsync(this HttpContent httpContent, CancellationToken _)
        {
            return httpContent.ReadAsStreamAsync();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<string> ReadAsStringAsync(this HttpContent httpContent, CancellationToken _)
        {
            return httpContent.ReadAsStringAsync();
        }
    }
}
#endif