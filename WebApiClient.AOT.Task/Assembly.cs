using Mono.Cecil;
using System;
using System.IO;
using System.Linq;
using WebApiClient;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示程序集
    /// </summary>
    class Assembly : IDisposable
    {
        /// <summary>
        /// 模块
        /// </summary>
        private ModuleDefinition module;

        /// <summary>
        /// 获取文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 程序集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <exception cref="FileNotFoundException"></exception>
        public Assembly(string fileName)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("找不到文件", fileName);
            }

            var searchPath = Path.GetDirectoryName(fileName);
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(searchPath);

            var parameter = new ReaderParameters
            {
                InMemory = true,
                ReadSymbols = true,
                AssemblyResolver = resolver
            };

            this.FileName = fileName;
            this.module = ModuleDefinition.ReadModule(fileName, parameter);
        }

        /// <summary>
        /// 写入代理类型
        /// </summary>
        public void WirteProxyTypes()
        {
            var iHttpApiFullName = this.module.ImportReference(typeof(IHttpApi)).FullName;
            var interfaces = this.module.Types
                .Where(item => item.IsInterface && item.Interfaces.Any(i => i.InterfaceType.FullName == iHttpApiFullName))
                .ToArray();

            foreach (var item in interfaces)
            {
                var @interface = new Interface(item);
                var proxyType = new ProxyTypeBuilder(@interface).Build();
                if (proxyType != null)
                {
                    this.module.Types.Add(proxyType);
                }
            }
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
            this.module.Write(this.FileName, parameters);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.module.Dispose();
        }
    }
}
