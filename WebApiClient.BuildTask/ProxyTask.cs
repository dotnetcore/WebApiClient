using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Represents inserting a proxy IL task
    /// </summary>
    public class ProxyTask : Task
    {
        /// <summary>
        /// Inserting the assembly of the agent
        /// </summary>
        [Required]
        public string TargetAssembly { get; set; }

        /// <summary>
        /// Referenced assembly
        /// Comma separated
        /// </summary>
        [Required]
        public ITaskItem[] References { get; set; }

        /// <summary>
        /// Perform tasks
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var logger = new Logger(this.Log);
            if (File.Exists(this.TargetAssembly) == false)
            {
                logger.Message($"Cannot find assembly for file compilation output {this.TargetAssembly}");
                return true;
            }

            try
            {
                logger.Message(this.GetType().AssemblyQualifiedName);
                var searchDirectories = this.GetSearchDirectories().Distinct().ToArray();
                using (var assembly = new CeAssembly(this.TargetAssembly, searchDirectories, logger))
                {
                    assembly.WirteProxyTypes();
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Back to dependent search directory
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
