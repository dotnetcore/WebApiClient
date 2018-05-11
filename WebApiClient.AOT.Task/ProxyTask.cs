using Microsoft.Build.Framework;
using System;

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
        public string ProxyAssembly { get; set; }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                using (var assembly = new Assembly(this.ProxyAssembly))
                {
                    assembly.WirteProxyTypes();
                    assembly.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
