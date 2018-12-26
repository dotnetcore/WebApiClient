using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// 表示程序集
    /// </summary>
    class CeAssembly : IDisposable
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// 程序集
        /// </summary>
        private readonly AssemblyDefinition assembly;

        /// <summary>
        /// 获取所有已知类型
        /// </summary>
        public TypeDefinition[] KnowTypes { get; private set; }

        /// <summary>
        /// 获取主模块
        /// </summary>
        public ModuleDefinition MainMdule { get; private set; }

        /// <summary>
        /// 程序集
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="searchDirectories">依赖项搜索目录</param>
        /// <param name="logger">日志</param>
        /// <exception cref="FileNotFoundException"></exception>
        public CeAssembly(string fileName, string[] searchDirectories, Logger logger)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("找不到文件编译输出的程序集");
            }

            var resolver = new DefaultAssemblyResolver();
            foreach (var dir in searchDirectories)
            {
                logger.Message("添加搜索目录", dir);
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

            this.MainMdule = this.assembly.MainModule;
            this.KnowTypes = this.MainMdule
                .AssemblyReferences
                .Select(asm => this.ResolveAssemblyNameReference(resolver, asm))
                .Where(item => item != null)
                .SelectMany(item => item.GetTypes())
                .ToArray();
        }

        /// <summary>
        /// 解析依赖项
        /// </summary>
        /// <param name="resolver">解析器</param>
        /// <param name="assembly">依赖的程序集</param>
        /// <returns></returns>
        private ModuleDefinition ResolveAssemblyNameReference(DefaultAssemblyResolver resolver, AssemblyNameReference assembly)
        {
            try
            {
                return resolver.Resolve(assembly).MainModule;
            }
            catch (Exception ex)
            {
                logger.Message(ex.Message);
                return null;
            }
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
                .Select(item => new CeInterface(this, item))
                .Where(item => item.IsHttpApiInterface())
                .ToArray();

            var willSave = false;
            foreach (var @interface in httpApiInterfaces)
            {
                var proxyType = new CeProxyType(this, @interface).Build();
                if (this.IsDefinded(proxyType) == true)
                {
                    continue;
                }

                this.logger.Message("正在写入IL", proxyType.FullName);
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
                this.logger.Message("正在保存修改", this.assembly.FullName);
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
