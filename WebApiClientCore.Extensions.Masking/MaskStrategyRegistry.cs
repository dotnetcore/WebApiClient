using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WebApiClientCore.Masking.Strategies;

namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示脱敏策略的注册表
    /// 创建实例时会自动注册内置策略
    /// </summary>
    public sealed class MaskStrategyRegistry
    {
        private readonly ConcurrentDictionary<string, IMaskStrategy> strategies;

        /// <summary>
        /// 获取全局默认的策略注册表
        /// </summary>
        public static MaskStrategyRegistry Default { get; } = new MaskStrategyRegistry();

        /// <summary>
        /// 脱敏策略的注册表
        /// </summary>
        public MaskStrategyRegistry()
            : this(null)
        {
        }

        /// <summary>
        /// 脱敏策略的注册表
        /// </summary>
        /// <param name="strategies">需要追加注册的策略</param>
        public MaskStrategyRegistry(IEnumerable<IMaskStrategy>? strategies)
        {
            this.strategies = new ConcurrentDictionary<string, IMaskStrategy>(StringComparer.OrdinalIgnoreCase);
            this.RegisterDefaults();

            if (strategies != null)
            {
                foreach (var strategy in strategies)
                {
                    this.Register(strategy);
                }
            }
        }

        /// <summary>
        /// 获取已注册的策略数量
        /// </summary>
        public int Count => this.strategies.Count;

        /// <summary>
        /// 获取已注册的所有策略
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IMaskStrategy> GetStrategies()
        {
            return new List<IMaskStrategy>(this.strategies.Values);
        }

        /// <summary>
        /// 注册策略
        /// 同名的策略会被覆盖
        /// </summary>
        /// <param name="strategy">脱敏策略</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Register(IMaskStrategy strategy)
        {
            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            if (string.IsNullOrWhiteSpace(strategy.Name) == true)
            {
                throw new ArgumentException("策略的名称不能为空", nameof(strategy));
            }

            this.strategies[strategy.Name] = strategy;
        }

        /// <summary>
        /// 移除指定名称的策略
        /// </summary>
        /// <param name="name">策略名称</param>
        /// <returns></returns>
        public bool Unregister(string name)
        {
            return string.IsNullOrWhiteSpace(name) == false && this.strategies.TryRemove(name, out _);
        }

        /// <summary>
        /// 获取指定名称的策略
        /// </summary>
        /// <param name="name">策略名称</param>
        /// <param name="strategy">策略</param>
        /// <returns></returns>
        public bool TryGet(string name, out IMaskStrategy? strategy)
        {
            strategy = null;
            return string.IsNullOrWhiteSpace(name) == false && this.strategies.TryGetValue(name, out strategy);
        }

        /// <summary>
        /// 获取指定名称的策略
        /// </summary>
        /// <param name="name">策略名称</param>
        /// <exception cref="MaskStrategyNotFoundException"></exception>
        /// <returns></returns>
        public IMaskStrategy Get(string name)
        {
            return this.TryGet(name, out var strategy) == true
                ? strategy!
                : throw new MaskStrategyNotFoundException(name);
        }

        /// <summary>
        /// 注册内置策略
        /// </summary>
        private void RegisterDefaults()
        {
            this.Register(new ReplaceMaskStrategy());
            this.Register(new TruncateMaskStrategy());
            this.Register(new PhoneMaskStrategy());
            this.Register(new EmailMaskStrategy());
            this.Register(new IdCardMaskStrategy());
        }
    }
}
