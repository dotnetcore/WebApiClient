namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 定义PasswordCredentialsOptions的创建器
    /// </summary>
    public interface IPasswordCredentialsOptionsBuilder
    {
        /// <summary>
        /// 获取服务集合
        /// </summary>
        IServiceCollection Services { get; }
    }

    /// <summary>
    /// PasswordCredentialsOptions的创建器
    /// </summary>
    class PasswordCredentialsOptionsBuilder : IPasswordCredentialsOptionsBuilder
    {
        public IServiceCollection Services { get; }

        public PasswordCredentialsOptionsBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
    }
}
