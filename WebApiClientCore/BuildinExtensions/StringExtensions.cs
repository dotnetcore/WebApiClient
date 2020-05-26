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
            using var owner = ArrayPool.Rent<char>(strSpan.Length);
            var strLowerSpan = owner.Array.AsSpan();
            var length = strSpan.ToLowerInvariant(strLowerSpan);
            strLowerSpan = strLowerSpan.Slice(0, length);

            var oldValueLowerSpan = oldValue.ToLowerInvariant().AsSpan();
            var newValueSpan = newValue.AsSpan();

            var replaced = false;
            using var writer = new BufferWriter<char>(strSpan.Length);

            while (strLowerSpan.Length > 0)
            {
                var index = strLowerSpan.IndexOf(oldValueLowerSpan);
                if (index > -1)
                {
                    // 左边未替换的
                    var left = strSpan.Slice(0, index);
                    writer.Write(left);

                    // 替换的值
                    writer.Write(newValueSpan);

                    // 切割长度
                    var sliceLength = index + oldValueLowerSpan.Length;

                    // 原始值与小写值同步切割
                    strSpan = strSpan.Slice(sliceLength);
                    strLowerSpan = strLowerSpan.Slice(sliceLength);

                    replaced = true;
                }
                else
                {
                    // 替换过剩下的原始值
                    if (replaced == true)
                    {
                        writer.Write(strSpan);
                    }

                    // 再也无匹配替换值，退出
                    break;
                }
            }

            replacedString = replaced ? writer.GetWrittenSpan().ToString() : str;
            return replaced;
        }
    }
}
