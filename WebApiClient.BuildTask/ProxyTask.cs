using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// 表示插入代理IL任务
    /// </summary>
    public class ProxyTask : Task
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
                void logger(string message) => this.Log.LogMessage(MessageImportance.High, message);

                logger(this.GetType().AssemblyQualifiedName);
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
