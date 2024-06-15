using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueSerializer序列化参数值得到的键值对作为url路径参数或query参数的特性
    /// </summary>
    public class PathQueryAttribute : ApiParameterAttribute, ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ApiInvalidConfigException"></exception>
        /// <returns></returns>
#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
#endif
        public override Task OnRequestAsync(ApiParameterContext context)
        {
            var uri = context.HttpContext.RequestMessage.RequestUri;
            if (uri == null)
            {
                throw new ApiInvalidConfigException(Resx.required_HttpHost);
            }

            var keyValues = this.SerializeToKeyValues(context).CollectAs(this.CollectionFormat);
            context.HttpContext.RequestMessage.RequestUri = this.CreateUri(uri, keyValues);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 序列化参数为keyValue
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
#if NET8_0_OR_GREATER
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
#endif
        protected virtual IEnumerable<KeyValue> SerializeToKeyValues(ApiParameterContext context)
        {
            return context.SerializeToKeyValues();
        }

        /// <summary>
        /// 创建新的uri
        /// </summary>
        /// <param name="uri">原始uri</param>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        protected virtual Uri CreateUri(Uri uri, IEnumerable<KeyValue> keyValues)
        {
            var uriValue = new UriValue(uri);
            foreach (var keyValue in keyValues)
            {
                uriValue = uriValue.Replace(keyValue.Key, keyValue.Value, out var replaced);
                if (replaced == false)
                {
                    uriValue = uriValue.AddQuery(keyValue.Key, keyValue.Value);
                }
            }
            return uriValue.ToUri();
        }
    }
}
