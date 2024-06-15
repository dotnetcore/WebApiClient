using System;
using WebApiClientCore.Internals;

namespace WebApiClientCore
{
    /// <summary>
    /// string扩展
    /// </summary>
    static class StringExtensions
    {
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string str, params object?[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// 不区分大小写替换字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="oldValue">要替换的值</param>
        /// <param name="newValue">替换的新值</param>
        /// <param name="replaced">是否替换成功</param> 
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string source, ReadOnlySpan<char> oldValue, ReadOnlySpan<char> newValue, out bool replaced)
        {
            replaced = false;
            if (string.IsNullOrEmpty(source) || oldValue.IsEmpty)
            {
                return source;
            }

            var index = 0;
            var sourceSpan = source.AsSpan();
            var builder = new ValueStringBuilder(stackalloc char[256]);

            while ((index = FindIndexIgnoreCase(sourceSpan, oldValue)) > -1)
            {
                builder.Append(sourceSpan[..index]);
                builder.Append(newValue);
                sourceSpan = sourceSpan[(index + oldValue.Length)..];
                replaced = true;
            }

            if (replaced == true)
            {
                builder.Append(sourceSpan);
                return builder.ToString();
            }
            return source;
        }

        /// <summary>
        /// 查找索引
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int FindIndexIgnoreCase(ReadOnlySpan<char> source, ReadOnlySpan<char> value)
        {
            int ctrSource;  // index value into source
            int ctrValue;   // index value into value
            char sourceChar;    // Character for case lookup in source
            char valueChar;     // Character for case lookup in value

            var sourceCount = source.Length;
            var valueCount = value.Length;

            if (valueCount == 0)
            {
                return 0;
            }

            if (sourceCount < valueCount)
            {
                return -1;
            }

            var lastSourceStart = sourceCount - valueCount;
            var firstValueChar = InvariantCaseFold(value[0]);
            for (ctrSource = 0; ctrSource <= lastSourceStart; ctrSource++)
            {
                sourceChar = InvariantCaseFold(source[ctrSource]);
                if (sourceChar != firstValueChar)
                {
                    continue;
                }

                for (ctrValue = 1; ctrValue < valueCount; ctrValue++)
                {
                    sourceChar = InvariantCaseFold(source[ctrSource + ctrValue]);
                    valueChar = InvariantCaseFold(value[ctrValue]);

                    if (sourceChar != valueChar)
                    {
                        break;
                    }
                }

                if (ctrValue == valueCount)
                {
                    return ctrSource;
                }
            }

            return -1;

            static char InvariantCaseFold(char c)
            {
                return (uint)(c - 'a') <= 'z' - 'a' ? (char)(c - 0x20) : c;
            }
        }
    }
}
