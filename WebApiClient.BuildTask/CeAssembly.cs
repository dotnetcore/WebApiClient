using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Representation assembly
    /// </summary>
    class CeAssembly : IDisposable
    {
        /// <summary>
        /// Log
        /// </summary>
        private readonly Logger logger;

        /// <summary>
        /// Assembly
        /// </summary>
        private readonly AssemblyDefinition assembly;

        /// <summary>
        /// Get all known types
        /// </summary>
        public TypeDefinition[] KnowTypes { get; private set; }

        /// <summary>
        /// Get the main module
        /// </summary>
        public ModuleDefinition MainMdule { get; private set; }

        /// <summary>
        /// Assembly
        /// </summary>
        /// <param name="fileName">file path</param>
        /// <param name="searchDirectories">Dependency search directory</param>
        /// <param name="logger">Log</param>
        /// <exception cref="FileNotFoundException"></exception>
        public CeAssembly(string fileName, string[] searchDirectories, Logger logger)
        {
            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("Cannot find assembly for file compilation output");
            }

            var resolver = new DefaultAssemblyResolver();
            foreach (var dir in searchDirectories)
            {
                logger.Message("Add search directory", dir);
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
        /// Resolve dependencies
        /// </summary>
        /// <param name="resolver">Parser</param>
        /// <param name="assembly">Dependent assembly</param>
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
        /// Write proxy type
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

                this.logger.Message("Writing to IL", proxyType.FullName);
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
                this.logger.Message("Saving changes", this.assembly.FullName);
                var parameters = new WriterParameters
                {
                    WriteSymbols = true
                };
                this.assembly.Write(parameters);
            }
            return willSave;
        }

        /// <summary>
        /// Whether the return type is declared in the module
        /// </summary>
        /// <param name="typeDefinition">Types of</param>
        /// <returns></returns>
        public bool IsDefinded(TypeDefinition typeDefinition)
        {
            return this.assembly
                .MainModule
                .GetTypes()
                .Any(item => item.FullName == typeDefinition.FullName);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
            this.assembly.Dispose();
        }
    }
}
