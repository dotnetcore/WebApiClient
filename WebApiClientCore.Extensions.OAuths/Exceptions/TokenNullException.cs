namespace WebApiClientCore.Extensions.OAuths.Exceptions
{
    /// <summary>
    /// 表示空 token 异常
    /// </summary>
    public class TokenNullException : TokenException
    {
        /// <summary>
        /// 空 token 异常
        /// </summary>
        public TokenNullException()
            : base("Unable to get token")
        {
        }
    }
}
