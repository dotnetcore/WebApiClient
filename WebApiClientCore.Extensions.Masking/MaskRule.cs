using System;

namespace WebApiClientCore.Masking
{
    /// <summary>
    /// 表示一条脱敏规则，即字段路径与脱敏策略的组合
    /// </summary>
    public sealed class MaskRule
    {
        /// <summary>
        /// 获取字段路径
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// 获取脱敏策略
        /// </summary>
        public IMaskStrategy Strategy { get; }

        /// <summary>
        /// 获取是否忽略字段名的大小写
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// 获取路径的分段
        /// </summary>
        internal string[] PathSegments { get; }

        /// <summary>
        /// 获取用于正则回退匹配的字段名
        /// </summary>
        internal string MatchName { get; }

        /// <summary>
        /// 字段路径与脱敏策略的组合
        /// </summary>
        /// <param name="path">
        /// 字段路径，不包含点时匹配任意层级的同名字段；
        /// 包含点时按层级精确匹配，其中 * 表示匹配任意一段
        /// </param>
        /// <param name="strategy">脱敏策略</param>
        /// <param name="ignoreCase">是否忽略字段名的大小写</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public MaskRule(string path, IMaskStrategy strategy, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(path) == true)
            {
                throw new ArgumentException($"'{nameof(path)}' 不能为空", nameof(path));
            }

            this.Path = path.Trim();
            this.PathSegments = this.Path.Split('.');
            this.MatchName = this.PathSegments[this.PathSegments.Length - 1];
            this.Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            this.IgnoreCase = ignoreCase;
        }

        /// <summary>
        /// 返回字段是否匹配本规则
        /// </summary>
        /// <param name="propertyName">字段名</param>
        /// <param name="fullPath">字段的完整路径</param>
        /// <returns></returns>
        internal bool IsMatch(string propertyName, string fullPath)
        {
            var comparison = this.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var segments = this.PathSegments;

            // 单段路径：匹配任意层级的同名字段
            if (segments.Length == 1)
            {
                return string.Equals(segments[0], propertyName, comparison);
            }

            var pathSegments = fullPath.Split('.');
            if (pathSegments.Length != segments.Length)
            {
                return false;
            }

            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i] != "*" && string.Equals(segments[i], pathSegments[i], comparison) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
