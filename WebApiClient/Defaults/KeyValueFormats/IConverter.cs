using System.Collections.Generic;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 定义转换器的接口
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// 设置第一个转换器
        /// </summary>
        IConverter First { set; }

        /// <summary>
        /// 设置下一个转换器
        /// </summary>
        IConverter Next { set; }

        /// <summary>
        /// 设置最高递归的层数
        /// </summary>
        int MaxDepth { set; }

        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context); 
    }
}
