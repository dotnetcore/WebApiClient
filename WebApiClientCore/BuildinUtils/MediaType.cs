using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供媒体类型比较
    /// </summary>
    static class MediaType
    {
        /// <summary>
        /// 是否与目标匹配
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsMatch(string? source, string? target)
        {
            return IsMatch(source.AsSpan(), target.AsSpan());
        }

        /// <summary>
        /// 是否与目标匹配
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool IsMatch(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        {
            var index = source.IndexOf('/');
            var sourceMain = index >= 0 ? source.Slice(0, index) : source;
            var sourceSub = index >= 0 ? source.Slice(index + 1) : ReadOnlySpan<char>.Empty;

            index = target.IndexOf('/');
            var targetMain = index >= 0 ? target.Slice(0, index) : target;
            var targetSub = index >= 0 ? target.Slice(index + 1) : ReadOnlySpan<char>.Empty;

            return MediaTypeMatch(sourceMain, targetMain) && MediaTypeMatch(sourceSub, targetSub);
        }


        /// <summary>
        /// MediaType是否匹配
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static bool MediaTypeMatch(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        {
            if (source.Length == 1 && source[0] == '*')
            {
                return true;
            }

            if (target.Length == 1 && target[0] == '*')
            {
                return true;
            }

            if (source.Length != target.Length)
            {
                return false;
            }

            for (var i = 0; i < source.Length; i++)
            {
                var s = source[i];
                var t = target[i];
                if (char.IsUpper(s))
                {
                    s = char.ToLowerInvariant(s);
                }
                if (char.IsUpper(t))
                {
                    t = char.ToLowerInvariant(t);
                }

                if (s.Equals(t) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
