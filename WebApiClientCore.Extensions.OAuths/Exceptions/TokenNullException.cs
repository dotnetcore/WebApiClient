namespace WebApiClientCore.Extensions.OAuths.Exceptions
{
    /// <summary>
    /// 表示空token异常
    /// </summary>
    public class TokenNullException : TokenException
    {
        /// <summary>
        /// 空token异常
        /// </summary>
        public TokenNullException()
            : base("Unable to get token")
        {
        }
    }
}
