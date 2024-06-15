using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 使用KeyValueSerializer序列化参数值得到的键值对作为x-www-form-urlencoded表单
    /// </summary>
    public class FormContentAttribute : HttpContentAttribute, ICollectionFormatable
    {
        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

        /// <summary>
        /// 设置参数到http请求内容
        /// </summary>
        /// <param name="context">上下文</param>
#if NET5_0_OR_GREATER
        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "<Pending>")]
#endif
        protected override async Task SetHttpContentAsync(ApiParameterContext context)
        {
            var keyValues = this.SerializeToKeyValues(context).CollectAs(this.CollectionFormat);
            await context.HttpContext.RequestMessage.AddFormFieldAsync(keyValues).ConfigureAwait(false);
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
    }
}
