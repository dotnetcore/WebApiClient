using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi创建工厂
    /// 提供HttpApi的配置注册和实例创建
    /// 并对实例的生命周期进行自动管理
    /// </summary>
    public partial class HttpApiFactory
    {
        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Register方法替代", false)]
        public static HttpApiFactory<TInterface> Add<TInterface>() where TInterface : class, IHttpApi
        {
            return HttpApi.Register<TInterface>();
        }

        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Register方法替代", false)]
        public static HttpApiFactory<TInterface> Add<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return HttpApi.Register<TInterface>(name);
        }

        /// <summary>
        /// 注册指定Api以及其工厂
        /// 返回Api工厂实例
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <param name="interfaceType">api接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Register方法替代", false)]
        public static HttpApiFactory Add(string name, Type interfaceType)
        {
            return HttpApi.Register(name, interfaceType);
        }



        /// <summary>
        /// 获取指定Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Get方法替代", false)]
        public static TInterface Create<TInterface>() where TInterface : class, IHttpApi
        {
            return HttpApi.Get<TInterface>();
        }

        /// <summary>
        /// 根据工厂名获取其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Get方法替代", false)]
        public static TInterface Create<TInterface>(string name) where TInterface : class, IHttpApi
        {
            return HttpApi.Get<TInterface>(name);
        }

        /// <summary>
        /// 根据工厂名获取其Api的代理实例
        /// 不能将该实例作全局变量引用
        /// </summary>
        /// <param name="name">工厂名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        [Obsolete("请使用HttpApi.Get方法替代", false)]
        public static HttpApiClient Create(string name)
        {
            return HttpApi.Get(name);
        }
    }
}
