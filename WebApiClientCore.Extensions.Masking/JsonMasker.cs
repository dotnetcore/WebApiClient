using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示 JSON 文本的脱敏器
    /// </summary>
    public static class JsonMasker
    {
        /// <summary>
        /// 正则回退时使用的匹配超时时间
        /// </summary>
        private static readonly TimeSpan regexTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 按规则对 JSON 文本中的字段执行脱敏
        /// 当文本不是合法的 JSON 时，回退为按字段名的正则匹配
        /// </summary>
        /// <param name="text">JSON 文本</param>
        /// <param name="rules">脱敏规则</param>
        /// <returns>脱敏后的文本</returns>
        public static string? Mask(string? text, IEnumerable<MaskRule>? rules)
        {
            if (string.IsNullOrEmpty(text) == true || rules == null)
            {
                return text;
            }

            var ruleList = rules as IReadOnlyList<MaskRule> ?? new List<MaskRule>(rules);
            if (ruleList.Count == 0)
            {
                return text;
            }

            try
            {
                return MaskJson(text, ruleList);
            }
            catch (JsonException)
            {
                return MaskByRegex(text, ruleList);
            }
        }

        /// <summary>
        /// 按 JSON 结构执行脱敏
        /// </summary>
        /// <param name="text">JSON 文本</param>
        /// <param name="rules">脱敏规则</param>
        /// <returns></returns>
        private static string MaskJson(string text, IReadOnlyList<MaskRule> rules)
        {
            using var document = JsonDocument.Parse(text);
            using var stream = new MemoryStream(text.Length);

            var processor = new MaskProcessor(rules);
            using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }))
            {
                processor.Write(writer, document.RootElement, string.Empty);
                writer.Flush();
            }

            // 没有发生脱敏时保留原始文本的格式
            return processor.Changed == true
                ? Encoding.UTF8.GetString(stream.ToArray())
                : text;
        }

        /// <summary>
        /// 按字段名执行正则脱敏
        /// 用于不是合法 JSON 的文本，属于尽力而为的方式
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="rules">脱敏规则</param>
        /// <returns></returns>
        private static string MaskByRegex(string text, IReadOnlyList<MaskRule> rules)
        {
            foreach (var rule in rules)
            {
                var name = rule.MatchName;
                if (string.IsNullOrEmpty(name) == true || name == "*")
                {
                    continue;
                }

                var pattern = $"(\"{Regex.Escape(name)}\"\\s*:\\s*\")((?:[^\"\\\\]|\\\\.)*)(\")";
                var options = RegexOptions.CultureInvariant;
                if (rule.IgnoreCase == true)
                {
                    options |= RegexOptions.IgnoreCase;
                }

                text = Regex.Replace(text, pattern, match =>
                {
                    var value = match.Groups[2].Value;
                    var masked = rule.Strategy.Mask(value);
                    return string.Equals(masked, value, StringComparison.Ordinal) == true
                        ? match.Value
                        : $"{match.Groups[1].Value}{masked}{match.Groups[3].Value}";
                }, options, regexTimeout);
            }

            return text;
        }

        /// <summary>
        /// 表示 JSON 的脱敏处理器
        /// </summary>
        private sealed class MaskProcessor
        {
            private readonly IReadOnlyList<MaskRule> rules;
            private readonly List<MaskRule> matchedRules = new();

            /// <summary>
            /// 获取是否发生了脱敏
            /// </summary>
            public bool Changed { get; private set; }

            /// <summary>
            /// JSON 的脱敏处理器
            /// </summary>
            /// <param name="rules">脱敏规则</param>
            public MaskProcessor(IReadOnlyList<MaskRule> rules)
            {
                this.rules = rules;
            }

            /// <summary>
            /// 写入 JSON 元素
            /// </summary>
            /// <param name="writer">JSON 写入器</param>
            /// <param name="element">JSON 元素</param>
            /// <param name="path">元素的路径</param>
            public void Write(Utf8JsonWriter writer, JsonElement element, string path)
            {
                switch (element.ValueKind)
                {
                    case JsonValueKind.Object:
                        writer.WriteStartObject();
                        foreach (var property in element.EnumerateObject())
                        {
                            var childPath = path.Length == 0 ? property.Name : $"{path}.{property.Name}";
                            this.WriteProperty(writer, property, childPath);
                        }
                        writer.WriteEndObject();
                        break;

                    case JsonValueKind.Array:
                        writer.WriteStartArray();
                        foreach (var item in element.EnumerateArray())
                        {
                            // 数组不产生路径层级，元素沿用数组所在的路径
                            this.Write(writer, item, path);
                        }
                        writer.WriteEndArray();
                        break;

                    default:
                        element.WriteTo(writer);
                        break;
                }
            }

            /// <summary>
            /// 写入 JSON 字段
            /// </summary>
            /// <param name="writer">JSON 写入器</param>
            /// <param name="property">JSON 字段</param>
            /// <param name="fullPath">字段的完整路径</param>
            private void WriteProperty(Utf8JsonWriter writer, JsonProperty property, string fullPath)
            {
                this.matchedRules.Clear();
                foreach (var rule in this.rules)
                {
                    if (rule.IsMatch(property.Name, fullPath) == true)
                    {
                        this.matchedRules.Add(rule);
                    }
                }

                if (this.matchedRules.Count == 0)
                {
                    writer.WritePropertyName(property.Name);
                    this.Write(writer, property.Value, fullPath);
                    return;
                }

                var element = property.Value;
                if (element.ValueKind == JsonValueKind.Null)
                {
                    writer.WritePropertyName(property.Name);
                    writer.WriteNullValue();
                    return;
                }

                var original = element.ValueKind == JsonValueKind.String
                    ? element.GetString() ?? string.Empty
                    : element.GetRawText();

                var masked = original;
                foreach (var rule in this.matchedRules)
                {
                    masked = rule.Strategy.Mask(masked);
                }

                if (string.Equals(masked, original, StringComparison.Ordinal) == true)
                {
                    writer.WritePropertyName(property.Name);
                    if (element.ValueKind == JsonValueKind.Object || element.ValueKind == JsonValueKind.Array)
                    {
                        this.Write(writer, element, fullPath);
                    }
                    else
                    {
                        element.WriteTo(writer);
                    }
                    return;
                }

                this.Changed = true;
                writer.WriteString(property.Name, masked);
            }
        }
    }
}
