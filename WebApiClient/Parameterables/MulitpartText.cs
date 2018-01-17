using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClient.Contexts;
using WebApiClient.Interfaces;

namespace WebApiClient.Parameterables
{
    /// <summary>
    /// 表示将自身作为multipart/form-data的一个文本项
    /// </summary>
    [DebuggerDisplay("{stringValue}")]
    public class MulitpartText : IApiParameterable
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private readonly string stringValue;

        /// <summary>
        /// 将自身作为multipart/form-data的一个文本项
        /// </summary>     
        /// <param name="value">文本内容</param>
        public MulitpartText(object value)
        {
            this.stringValue = value == null ? string.Empty : value.ToString();
        }

        /// <summary>
        /// multipart/form-data的一个文本项
        /// </summary>     
        /// <param name="value">文本内容</param>
        public MulitpartText(string value)
        {
            this.stringValue = value;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        async Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.RequestMessage.AddMulitpartText(parameter.Name, this.stringValue);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// 从string类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(string value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从int类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(int value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从int?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(int? value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从decimal类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(decimal value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从decimal?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(decimal? value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从double类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(double value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从double?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(double? value)
        {
            return new MulitpartText(value);
        }


        /// <summary>
        /// 从DateTime类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(DateTime value)
        {
            return new MulitpartText(value);
        }

        /// <summary>
        /// 从DateTime?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator MulitpartText(DateTime? value)
        {
            return new MulitpartText(value);
        }
    }
}
