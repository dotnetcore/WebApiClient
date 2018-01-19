using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClient.Defaults.KeyValueFormats.Converters
{
    /// <summary>
    /// 表示集合转换器
    /// 负责拆解集合并递归转换
    /// </summary>
    public class EnumerableConverter : ConverterBase
    {
        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
        {
            var array = context.Data as IEnumerable;
            if (array == null)
            {
                return this.Next.Invoke(context);
            }

            var ctxs =
                from item in array.Cast<object>()
                select new ConvertContext(context.Name, item, context.Depths, context.Options);

            return ctxs.SelectMany(ctx => this.Recurse(ctx));
        }
    }
}
