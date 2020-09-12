namespace WebApiClientCore.Extensions.OAuths.TokenClients
{
    /// <summary>
    /// 用于处理指定接口类型的自定义token客户端
    /// </summary>
    /// <typeparam name="THttpApi">处理的接口类型</typeparam>
    interface ICustomTokenClient<THttpApi> : ICustomTokenClient
    {
    }
}
