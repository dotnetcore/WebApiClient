using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (context.Type.IsInheritFrom<IEnumerable>() == true)
            {
                var array = context.Value as IEnumerable;

                // 递归转换数组里各个元素
                return array.Cast<object>()
                    .SelectMany(item => base.RecurseConvert(context.Name, item, context.Options));
            }
            return this.Next.Invoke(context);
        }
    }
}
