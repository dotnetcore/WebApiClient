using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiClientCore.Parameters
{
    /// <summary>
    /// 表示将自身作为x-www-form-urlencoded的字段
    /// </summary>
    [DebuggerDisplay("{stringValue}")]
    public class FormField : IApiParameter
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private readonly string stringValue;

        /// <summary>
        /// 将自身作为x-www-form-urlencoded的字段
        /// </summary>     
        /// <param name="value">文本内容</param>
        public FormField(object value)
        {
            this.stringValue = value?.ToString();
        }

        /// <summary>
        /// x-www-form-urlencoded的字段
        /// 如果有[FormContent]的参数，FormField需要放在其后
        /// </summary>     
        /// <param name="value">文本内容</param>
        public FormField(string value)
        {
            this.stringValue = value;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public Task OnRequestAsync(ApiParameterContext context)
        {
            return context.HttpContext.RequestMessage.AddFormFieldAsync(context.Parameter.Name, this.stringValue);
        }

        /// <summary>
        /// 从string类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(string value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从int类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(int value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从int?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(int? value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从decimal类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(decimal value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从decimal?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(decimal? value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从float类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(float value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从float?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(float? value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从double类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(double value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从double?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(double? value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从DateTime类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(DateTime value)
        {
            return new FormField(value);
        }

        /// <summary>
        /// 从DateTime?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormField(DateTime? value)
        {
            return new FormField(value);
        }
    }
}
