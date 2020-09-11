using System;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求参数特性
    /// </summary>
    public class ParameterAttribute : ApiParameterAttribute
    {
        private static readonly ApiParameterAttribute pathQuery = new PathQueryAttribute();

        private static readonly ApiParameterAttribute headers = new HeadersAttribute();

        private static readonly ApiParameterAttribute formContent = new FormContentAttribute();

        private static readonly ApiParameterAttribute formDataContent = new FormDataContentAttribute();

        private static readonly ApiParameterAttribute jsonContent = new JsonContentAttribute();

        private static readonly ApiParameterAttribute xmlContent = new XmlContentAttribute();

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
        /// <returns></returns>
        public override async Task OnRequestAsync(ApiParameterContext context)
        {
            switch (this.Kind)
            {
                case Kind.Path:
                    await pathQuery.OnRequestAsync(context).ConfigureAwait(false);
                    break;

                case Kind.Header:
                    await headers.OnRequestAsync(context).ConfigureAwait(false);
                    break;

                case Kind.Form:
                    await formContent.OnRequestAsync(context).ConfigureAwait(false);
                    break;

                case Kind.FormData:
                    await formDataContent.OnRequestAsync(context).ConfigureAwait(false);
                    break;

                case Kind.Json:
                    await jsonContent.OnRequestAsync(context).ConfigureAwait(false);
                    break;

                case Kind.Xml:
                    await xmlContent.OnRequestAsync(context).ConfigureAwait(false);
                    break;
            }
        }
    }
}
