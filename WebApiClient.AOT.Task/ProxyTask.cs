using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 代理任务
    /// </summary>
    public class ProxyTask : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// 插入代理的程序集
        /// </summary>
        [Required]
        public string TargetAssembly { get; set; }

        /// <summary>
        /// 引用的程序集
        /// 逗号分隔
        /// </summary>
        [Required]
        public string References { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                var searchPaths = this.GetSearchPaths().Distinct().ToArray();
                using (var assembly = new CeAssembly(this.TargetAssembly, searchPaths))
                {
                    if (assembly.WirteProxyTypes() > 0)
                    {
                        assembly.Save();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 返回搜索的搜索目录
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetSearchPaths()
        {
            yield return Path.GetDirectoryName(this.TargetAssembly);

            if (string.IsNullOrEmpty(this.References) == true)
            {
                yield break;
            }

            foreach (var file in this.References.Split(';'))
            {
                if (string.IsNullOrEmpty(file) == true)
                {
                    continue;
                }

                var path = Path.GetDirectoryName(file);
                if (Directory.Exists(path) == true)
                {
                    yield return path;
                }
            }
        }

    }
}
