using System;

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
        /// <param name="str"></param>
        /// <param name="oldValue">原始值</param>
        /// <param name="newValue">新值</param>
        /// <param name="replacedString">替换后的字符中</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static bool RepaceIgnoreCase(this string str, string oldValue, string? newValue, out string replacedString)
        {
            if (string.IsNullOrEmpty(str) == true)
            {
                replacedString = str;
                return false;
            }

            if (string.IsNullOrEmpty(oldValue) == true)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            var strSpan = str.AsSpan();
            var oldValueSpan = oldValue.AsSpan();
            var newValueSpan = newValue.AsSpan();

            var replaced = false;
            var builder = new ValueStringBuilder(stackalloc char[256]);

            while (strSpan.Length > 0)
            {
                var index = FindIndexIgnoreCase(strSpan, oldValue);
                if (index > -1)
                {
                    builder.Append(strSpan.Slice(0, index));
                    builder.Append(newValueSpan);
                    strSpan = strSpan.Slice(index + oldValueSpan.Length);
                    replaced = true;
                }
                else
                {
                    if (replaced == true)
                    {
                        builder.Append(strSpan);
                    }
                    break;
                }
            }

            replacedString = replaced ? builder.ToString() : str;
            return replaced;
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
