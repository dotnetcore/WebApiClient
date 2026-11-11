using WebApiClientCore.Masking.Strategies;
using Xunit;

namespace WebApiClientCore.Masking.Test
{
    public class MaskStrategyTest
    {
        [Fact]
        public void Replace替换为默认值()
        {
            var strategy = new ReplaceMaskStrategy();

            Assert.Equal("Replace", strategy.Name);
            Assert.Equal("***", strategy.Mask("123456"));
        }

        [Fact]
        public void Replace支持自定义替换值和名称()
        {
            var strategy = new ReplaceMaskStrategy("[已脱敏]", "CustomReplace");

            Assert.Equal("CustomReplace", strategy.Name);
            Assert.Equal("[已脱敏]", strategy.Mask("123456"));
        }

        [Fact]
        public void Truncate保留前后指定位数()
        {
            var strategy = new TruncateMaskStrategy(keepStart: 3, keepEnd: 4);

            Assert.Equal("123***7890", strategy.Mask("1234567890"));
        }

        [Fact]
        public void Truncate在长度不足时返回原值()
        {
            var strategy = new TruncateMaskStrategy(keepStart: 3, keepEnd: 4);

            Assert.Equal("1234567", strategy.Mask("1234567"));
        }

        [Fact]
        public void Phone只处理11位手机号()
        {
            var strategy = new PhoneMaskStrategy();

            Assert.Equal("138****5678", strategy.Mask("13812345678"));
            Assert.Equal("1381234567", strategy.Mask("1381234567"));
            Assert.Equal("abcdefghijk", strategy.Mask("abcdefghijk"));
        }

        [Fact]
        public void Email保留首字符和域名()
        {
            var strategy = new EmailMaskStrategy();

            Assert.Equal("z***@example.com", strategy.Mask("zhangsan@example.com"));
            Assert.Equal("not-an-email", strategy.Mask("not-an-email"));
        }

        [Fact]
        public void IdCard支持18位和15位()
        {
            var strategy = new IdCardMaskStrategy();

            Assert.Equal("110101********1234", strategy.Mask("110101199001011234"));
            Assert.Equal("110101*****1123", strategy.Mask("110101900101123"));
            Assert.Equal("123456", strategy.Mask("123456"));
        }
    }
}
