using System.Text.Json;
using WebApiClientCore.Masking.Strategies;
using Xunit;

namespace WebApiClientCore.Masking.Test
{
    public class JsonMaskerTest
    {
        private static readonly MaskStrategyRegistry registry = new MaskStrategyRegistry();

        private static MaskRule Rule(string path, string strategy = ReplaceMaskStrategy.StrategyName, bool ignoreCase = false)
        {
            return new MaskRule(path, registry.Get(strategy), ignoreCase);
        }

        [Fact]
        public void 同名任意层级的字段都会脱敏()
        {
            var json = """{"phone":"13812345678","user":{"phone":"13912345678"},"remark":"13812345678"}""";

            var result = JsonMasker.Mask(json, new[] { Rule("phone", PhoneMaskStrategy.StrategyName) })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal("138****5678", document.RootElement.GetProperty("phone").GetString());
            Assert.Equal("139****5678", document.RootElement.GetProperty("user").GetProperty("phone").GetString());
            Assert.Equal("13812345678", document.RootElement.GetProperty("remark").GetString());
        }

        [Fact]
        public void 包含点的路径按层级精确匹配()
        {
            var json = """{"phone":"13812345678","user":{"phone":"13912345678"}}""";

            var result = JsonMasker.Mask(json, new[] { Rule("user.phone", PhoneMaskStrategy.StrategyName) })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal("13812345678", document.RootElement.GetProperty("phone").GetString());
            Assert.Equal("139****5678", document.RootElement.GetProperty("user").GetProperty("phone").GetString());
        }

        [Fact]
        public void 支持星号通配路径和数组元素()
        {
            var json = """
                {
                  "data": {
                    "user": { "phone": "13812345678" },
                    "list": [ { "phone": "13912345678" } ]
                  }
                }
                """;

            var result = JsonMasker.Mask(json, new[] { Rule("data.*.phone", PhoneMaskStrategy.StrategyName) })!;
            using var document = JsonDocument.Parse(result);

            var data = document.RootElement.GetProperty("data");
            Assert.Equal("138****5678", data.GetProperty("user").GetProperty("phone").GetString());
            Assert.Equal("139****5678", data.GetProperty("list")[0].GetProperty("phone").GetString());
        }

        [Fact]
        public void 数字类型的字段脱敏后输出为字符串()
        {
            var json = """{"phone":13812345678}""";

            var result = JsonMasker.Mask(json, new[] { Rule("phone", PhoneMaskStrategy.StrategyName) })!;
            using var document = JsonDocument.Parse(result);

            var phone = document.RootElement.GetProperty("phone");
            Assert.Equal(JsonValueKind.String, phone.ValueKind);
            Assert.Equal("138****5678", phone.GetString());
        }

        [Fact]
        public void 对象字段可以整体替换()
        {
            var json = """{"user":{"name":"张三","phone":"13812345678"}}""";

            var result = JsonMasker.Mask(json, new[] { Rule("user") })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal("***", document.RootElement.GetProperty("user").GetString());
        }

        [Fact]
        public void 值为null的字段不会被脱敏()
        {
            var json = """{"phone":null}""";

            var result = JsonMasker.Mask(json, new[] { Rule("phone", PhoneMaskStrategy.StrategyName) })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal(JsonValueKind.Null, document.RootElement.GetProperty("phone").ValueKind);
        }

        [Fact]
        public void 没有匹配的规则时保留原始文本()
        {
            var json = """
                {
                  "name": "张三",
                  "phone": "123"
                }
                """;

            var result = JsonMasker.Mask(json, new[] { Rule("password") });

            Assert.Equal(json, result);
        }

        [Fact]
        public void 忽略大小写时字段名不区分大小写()
        {
            var json = """{"Phone":"13812345678"}""";

            var result = JsonMasker.Mask(json, new[] { Rule("phone", PhoneMaskStrategy.StrategyName, ignoreCase: true) })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal("138****5678", document.RootElement.GetProperty("Phone").GetString());
        }

        [Fact]
        public void 中文字符不会被转义()
        {
            var json = """{"name":"张三","password":"123456"}""";

            var result = JsonMasker.Mask(json, new[] { Rule("password") })!;

            Assert.Contains("张三", result);
            Assert.Contains("\"password\":\"***\"", result);
        }

        [Fact]
        public void 字符串中的转义字符能正确处理()
        {
            var json = """{"password":"a\"b"}""";

            var result = JsonMasker.Mask(json, new[] { Rule("password") })!;
            using var document = JsonDocument.Parse(result);

            Assert.Equal("***", document.RootElement.GetProperty("password").GetString());
        }

        [Fact]
        public void 非法的JSON回退为正则匹配()
        {
            var json = """{ "phone": "13812345678", }""";

            var result = JsonMasker.Mask(json, new[] { Rule("phone", PhoneMaskStrategy.StrategyName) })!;

            Assert.Contains("138****5678", result);
        }

        [Fact]
        public void 规则为空时返回原始文本()
        {
            var json = """{"phone":"13812345678"}""";

            var result = JsonMasker.Mask(json, System.Array.Empty<MaskRule>());

            Assert.Equal(json, result);
        }

        [Fact]
        public void 文本为空时返回原始值()
        {
            Assert.Null(JsonMasker.Mask(null, new[] { Rule("phone") }));
            Assert.Equal(string.Empty, JsonMasker.Mask(string.Empty, new[] { Rule("phone") }));
        }
    }
}
