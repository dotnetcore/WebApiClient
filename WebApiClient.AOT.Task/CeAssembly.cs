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
        /// 程序集
        /// </summary>
        private readonly AssemblyDefinition assembly;

        /// <summary>
        /// 获取文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 程序集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="searchPaths">依赖项搜索目录</param>
        /// <exception cref="FileNotFoundException"></exception>
        public CeAssembly(string fileName, string[] searchPaths)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("找不到文件", fileName);
            }

            var resolver = new DefaultAssemblyResolver();
            foreach (var path in searchPaths)
            {
                resolver.AddSearchDirectory(path);
            }

            var parameter = new ReaderParameters
            {
                ReadWrite = true,
                ReadSymbols = true,
                InMemory = true,
                AssemblyResolver = resolver
            };

            this.FileName = fileName;
            this.assembly = AssemblyDefinition.ReadAssembly(fileName, parameter);
        }

        /// <summary>
        /// 写入代理类型
        /// 返回受影响的接口数
        /// </summary>
        /// <param name="logger">日志</param>
        /// <returns></returns>
        public int WirteProxyTypes(Action<string> logger)
        {
            var httpApiInterfaces = this.assembly
                .MainModule
                .GetTypes()
                .Select(item => new CeInterface(item))
                .Where(item => item.IsHttpApiInterface())
                .ToArray();

            var write = 0;
            foreach (var @interface in httpApiInterfaces)
            {
                var proxyType = new CeProxyType(@interface);
                if (proxyType.IsDefinded() == false)
                {
                    logger?.Invoke($"正在写入{@interface.Type.FullName}代理IL指令");
                    this.assembly.MainModule.Types.Add(proxyType.Build());
                    write = write + 1;
                }
            }

            return write;
        }


        /// <summary>
        /// 插入代理并保存
        /// </summary>
        public void Save()
        {
            var parameters = new WriterParameters
            {
                WriteSymbols = true
            };
            this.assembly.Write(this.FileName, parameters);
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
