namespace WebApiClientCore
{
    /// <summary>
    /// 定义接口的HttpApiOptions通知者的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface IHttpApiOptionsNotifer<THttpApi>
    {
        /// <summary>
        /// 通知重新加载HttpApiOptions
        /// </summary>
        void ReloadHttpApiOptions();
    }
}
