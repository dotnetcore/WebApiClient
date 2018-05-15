using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示插入代理IL任务
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
        public ITaskItem[] References { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                var searchDirectories = this.GetSearchDirectories().Distinct().ToArray();
                Action<string> logger = (message) => this.Log.LogMessage(MessageImportance.High, message);
                using (var assembly = new CeAssembly(this.TargetAssembly, searchDirectories, logger))
                {
                    assembly.WirteProxyTypes();
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 返回依赖搜索目录
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetSearchDirectories()
        {
            if (this.References == null)
            {
                yield break;
            }

            foreach (var item in this.References)
            {
                if (string.IsNullOrEmpty(item.ItemSpec) == false)
                {
                    var path = Path.GetDirectoryName(item.ItemSpec);
                    if (Directory.Exists(path) == true)
                    {
                        yield return path;
                    }
                }
            }
        }
    }
}
