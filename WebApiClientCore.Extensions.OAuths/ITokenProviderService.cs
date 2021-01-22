namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义http接口的token提供者服务
    /// </summary>
    interface ITokenProviderService
    {
        /// <summary>
        /// 获取token提供者
        /// </summary>
        ITokenProvider TokenProvider { get; }
    }
}