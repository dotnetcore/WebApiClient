namespace WebApiClientCore.Exceptions
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class ApiReturnNotSupportedException : ApiReturnNotSupportedExteption
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="context"></param>
        public ApiReturnNotSupportedException(ApiResponseContext context)
            : base(context)
        {
        }
    }
}
