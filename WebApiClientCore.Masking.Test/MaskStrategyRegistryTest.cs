using System.Linq;
using WebApiClientCore.Masking.Strategies;
using Xunit;

namespace WebApiClientCore.Masking.Test
{
    public class MaskStrategyRegistryTest
    {
        [Fact]
        public void 默认包含内置策略()
        {
            var registry = new MaskStrategyRegistry();

            Assert.Equal(5, registry.Count);
            foreach (var name in new[] { "Replace", "Truncate", "Phone", "Email", "IdCard" })
            {
                Assert.True(registry.TryGet(name, out var strategy));
                Assert.Equal(name, strategy!.Name);
            }
        }

        [Fact]
        public void 注册同名策略会覆盖内置策略()
        {
            var registry = new MaskStrategyRegistry();
            registry.Register(new ReplaceMaskStrategy("[已脱敏]"));

            Assert.Equal(5, registry.Count);
            Assert.Equal("[已脱敏]", registry.Get("Replace").Mask("abc"));
        }

        [Fact]
        public void 策略名称不区分大小写()
        {
            var registry = new MaskStrategyRegistry();

            Assert.Equal("138****5678", registry.Get("phone").Mask("13812345678"));
        }

        [Fact]
        public void 可以移除策略()
        {
            var registry = new MaskStrategyRegistry();

            Assert.True(registry.Unregister("Phone"));
            Assert.False(registry.TryGet("Phone", out _));
            Assert.False(registry.Unregister("Phone"));
        }

        [Fact]
        public void 获取不存在的策略时抛出异常()
        {
            var registry = new MaskStrategyRegistry();

            var exception = Assert.Throws<MaskStrategyNotFoundException>(() => registry.Get("NotExists"));
            Assert.Equal("NotExists", exception.StrategyName);
        }

        [Fact]
        public void 构造时可以追加注册策略()
        {
            var registry = new MaskStrategyRegistry(new[] { new ReplaceMaskStrategy("[x]", "Extra") });

            Assert.Equal(6, registry.Count);
            Assert.Equal(6, registry.GetStrategies().Count());
            Assert.Equal("[x]", registry.Get("Extra").Mask("abc"));
        }
    }
}
