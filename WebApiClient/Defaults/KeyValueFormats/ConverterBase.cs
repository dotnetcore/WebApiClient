using System;
using System.Collections.Generic;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 表示转换器的抽象类
    /// </summary>
    public abstract class ConverterBase : IConverter
    {
        /// <summary>
        /// 第一个转换器
        /// </summary>
        private IConverter first;

        /// <summary>
        /// 最高递归的层数     
        /// </summary>
        private int maxDepth = 16;

        /// <summary>
        /// 获取下一个转换器
        /// </summary>
        protected IConverter Next { get; private set; }

        /// <summary>
        /// 设置第一个转换器
        /// </summary>
        IConverter IConverter.First
        {
            set
            {
                this.first = value;
            }
        }

        /// <summary>
        /// 设置下一个转换器
        /// </summary>
        IConverter IConverter.Next
        {
            set
            {
                this.Next = value;
            }
        }

        /// <summary>
        /// 设置最高递归的层数
        /// </summary>
        int IConverter.MaxDepth
        {
            set
            {
                this.maxDepth = value;
            }
        }

        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public abstract IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context);

        /// <summary>
        /// 递归执行转换
        /// 调用头转换器进行解析context
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        protected IEnumerable<KeyValuePair<string, string>> Recurse(ConvertContext context)
        {
            if (context.Depths >= this.maxDepth)
            {
                throw new NotSupportedException("转换的层数已超过设置的MaxDepth");
            }

            context.Depths = context.Depths + 1;
            return this.first.Invoke(context);
        }
    }
}
