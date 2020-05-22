namespace WebApiClientCore.Extensions.OAuths.Exceptions
{
    /// <summary>
    /// 表示获取Toke的Url节点为nul的异常  
    /// </summary>
    public class TokenEndPointNullException : TokenException
    {
        /// <summary>
        /// 获取Toke的Url节点为nul的异常  
        /// </summary>
        public TokenEndPointNullException()
            : base("The Endpoint is required")
        {
        }
    }
}
