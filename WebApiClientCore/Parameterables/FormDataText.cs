using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiClientCore.Parameterables
{
    /// <summary>
    /// 表示form-data表单的一个文本项
    /// </summary>
    [DebuggerDisplay("{stringValue}")]
    public class FormDataText : IApiParameterable
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        private readonly string stringValue;

        /// <summary>
        /// form-data表单的一个文本项
        /// </summary>     
        /// <param name="value">文本内容</param>
        public FormDataText(object value)
        {
            this.stringValue = value?.ToString();
        }

        /// <summary>
        /// form-data表单的一个文本项
        /// </summary>     
        /// <param name="value">文本内容</param>
        public FormDataText(string value)
        {
            this.stringValue = value;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        async Task IApiParameterable.OnRequestAsync(ApiParameterContext context)
        {
            context.HttpContext.RequestMessage.AddFormDataText(context.Parameter.Name, this.stringValue);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 从string类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(string value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从int类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(int value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从int?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(int? value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从decimal类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(decimal value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从decimal?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(decimal? value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从float类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(float value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从float?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(float? value)
        {
            return new FormDataText(value);
        }


        /// <summary>
        /// 从double类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(double value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从double?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(double? value)
        {
            return new FormDataText(value);
        }


        /// <summary>
        /// 从DateTime类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(DateTime value)
        {
            return new FormDataText(value);
        }

        /// <summary>
        /// 从DateTime?类型隐式转换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator FormDataText(DateTime? value)
        {
            return new FormDataText(value);
        }
    }
}
