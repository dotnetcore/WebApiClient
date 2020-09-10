namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 定义ClientCredentialsOptions的创建器
    /// </summary>
    public interface IClientCredentialsOptionsBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// ClientCredentialsOptions的创建器
    /// </summary>
    class ClientCredentialsOptionsBuilder : IClientCredentialsOptionsBuilder
    {
        public IServiceCollection Services { get; }

        public ClientCredentialsOptionsBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
