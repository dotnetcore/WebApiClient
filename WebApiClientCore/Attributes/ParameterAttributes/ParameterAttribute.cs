using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ParameterAttribute : Attribute, IApiParameterAttribute
    {
        private static readonly IApiParameterAttribute pathQuery = new PathQueryAttribute();

        private static readonly IApiParameterAttribute headers = new HeadersAttribute();

        private static readonly IApiParameterAttribute formContent = new FormContentAttribute();

        private static readonly IApiParameterAttribute formDataContent = new FormDataContentAttribute();

        private static readonly IApiParameterAttribute jsonContent = new JsonContentAttribute();

        private static readonly IApiParameterAttribute xmlContent = new XmlContentAttribute();

        /// <summary>
        /// 获取参数类型
        /// </summary>
        public Kind Kind { get; }

        /// <summary>
        /// 请求参数特性
        /// </summary>
        /// <param name="kind">参数类型</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ParameterAttribute(Kind kind)
        {
            if (Enum.IsDefined(typeof(Kind), kind) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }
            this.Kind = kind;
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiParameterContext context,Func<Task> next)
        {
            switch (this.Kind)
            {
                case Kind.Path:
                    await pathQuery.BeforeRequestAsync(context,next).ConfigureAwait(false);
                    break;

                case Kind.Header:
                    await headers.BeforeRequestAsync(context, next).ConfigureAwait(false);
                    break;

                case Kind.Form:
                    await formContent.BeforeRequestAsync(context, next).ConfigureAwait(false);
                    break;

                case Kind.FormData:
                    await formDataContent.BeforeRequestAsync(context, next).ConfigureAwait(false);
                    break;

                case Kind.JsonBody:
                    await jsonContent.BeforeRequestAsync(context, next).ConfigureAwait(false);
                    break;

                case Kind.XmlBody:
                    await xmlContent.BeforeRequestAsync(context, next).ConfigureAwait(false);
                    break;
            }
        }
    }

    /// <summary>
    /// 表示参数类型
    /// </summary>
    public enum Kind
    {
        /// <summary>
        /// Uri路径参数
        /// 等效PathQueryAttribute
        /// </summary>
        Path = 0,

        /// <summary>
        /// Uri Query
        /// 等效PathQueryAttribute
        /// </summary>
        Query = 0,

        /// <summary>
        /// Header
        /// 等效HeaderAttribute
        /// </summary>
        Header = 1,

        /// <summary>
        /// x-www-form-urlencoded
        /// 等效FormContentAttribute
        /// </summary>
        Form = 2,

        /// <summary>
        /// multipart/form-data
        /// 等效MulitpartContentAttribute
        /// </summary>
        FormData = 3,

        /// <summary>
        /// application/json
        /// 等效JsonContentAttribute
        /// </summary>
        JsonBody = 4,

        /// <summary>
        /// application/xml
        /// 等效XmlContentAttribute
        /// </summary>
        XmlBody = 5
    }
}
