using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示程序集
    /// </summary>
    class CeAssembly : IDisposable
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly Action<string> logger;

        /// <summary>
        /// 程序集
        /// </summary>
        private readonly AssemblyDefinition assembly;

        /// <summary>
        /// 程序集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="searchDirectories">依赖项搜索目录</param>
        /// <param name="logger">日志</param>
        /// <exception cref="FileNotFoundException"></exception>
        public CeAssembly(string fileName, string[] searchDirectories, Action<string> logger)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("找不到文件编译输出的程序集");
            }

            var resolver = new DefaultAssemblyResolver();
            foreach (var dir in searchDirectories)
            {
                logger($"添加搜索目录-> {dir}");
                resolver.AddSearchDirectory(dir);
            }

            var parameter = new ReaderParameters
            {
                ReadWrite = true,
                ReadSymbols = true,
                AssemblyResolver = resolver
            };

            this.logger = logger;
            this.assembly = AssemblyDefinition.ReadAssembly(fileName, parameter);
        }

        /// <summary>
        /// 写入代理类型
        /// </summary>
        /// <returns></returns>
        public bool WirteProxyTypes()
        {
            var httpApiInterfaces = this.assembly
                .MainModule
                .GetTypes()
                .Select(item => new CeInterface(item))
                .Where(item => item.IsHttpApiInterface())
                .ToArray();

            var willSave = false;
            foreach (var @interface in httpApiInterfaces)
            {
                var proxyType = new CeProxyType(@interface).Build();
                if (this.IsDefinded(proxyType) == true)
                {
                    continue;
                }

                this.logger($"正在写入IL-> {proxyType.FullName}");
                if (proxyType.DeclaringType != null)
                {
                    proxyType.DeclaringType.NestedTypes.Add(proxyType);
                }
                else
                {
                    this.assembly.MainModule.Types.Add(proxyType);
                }
                willSave = true;
            }

            if (willSave == true)
            {
                this.logger($"正在保存修改-> {this.assembly.FullName}");
                var parameters = new WriterParameters
                {
                    WriteSymbols = true
                };
                this.assembly.Write(parameters);
            }
            return willSave;
        }

        /// <summary>
        /// 返回类型是否在模块中已声明
        /// </summary>
        /// <param name="typeDefinition">类型</param>
        /// <returns></returns>
        public bool IsDefinded(TypeDefinition typeDefinition)
        {
            return this.assembly
                .MainModule
                .GetTypes()
                .Any(item => item.FullName == typeDefinition.FullName);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.assembly.Dispose();
        }
    }
}
