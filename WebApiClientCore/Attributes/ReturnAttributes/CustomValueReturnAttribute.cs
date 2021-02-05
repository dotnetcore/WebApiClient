namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将结果设置为返回类型的自定义值抽象特性
    /// </summary> 
    public abstract class CustomValueReturnAttribute : SpecialReturnAttribute
    {
        /// <summary>
        /// 将结果设置为返回类型的自定义值抽象特性
        /// </summary>
        public CustomValueReturnAttribute()
            : base()
        {
        }

        /// <summary>
        /// 将结果设置为返回类型的自定义值抽象特性
        /// </summary>
        /// <param name="acceptQuality">accept的质比</param>
        public CustomValueReturnAttribute(double acceptQuality)
            : base(acceptQuality)
        {
        }

        /// <summary>
        /// 指示是否可以设置结果
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected sealed override bool CanSetResult(ApiResponseContext context)
        {
            return context.ActionDescriptor.Return.DataType.IsRawType == false;
        } 
    }
}
