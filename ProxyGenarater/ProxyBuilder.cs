using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient;

namespace ProxyGenarater
{
    /// <summary>
    /// 表示HttpApiClient代理类
    /// </summary>
    class ProxyBuilder : IDisposable
    {
        /// <summary>
        /// IApiInterceptor的Intercept方法
        /// </summary>
        private static readonly System.Reflection.MethodInfo interceptMethod = typeof(IApiInterceptor).GetMethod("Intercept");

        /// <summary>
        /// HttpApiClient的构造器
        /// </summary>
        private static readonly System.Reflection.ConstructorInfo baseConstructor = typeof(HttpApiClient).GetConstructor(new Type[] { typeof(IApiInterceptor) });

        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(System.Reflection.MethodInfo[]) };


        /// <summary>
        /// 获取文件名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 获取模块
        /// </summary>
        public ModuleDefinition Module { get; private set; }

        /// <summary>
        /// HttpApiClient代理类
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public ProxyBuilder(string fileName)
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
            this.Module = ModuleDefinition.ReadModule(fileName, parameter);
        }


        /// <summary>
        /// 返回TypeReference
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private TypeReference GetType(Type type)
        {
            return this.Module.ImportReference(type);
        }

        /// <summary>
        /// 返回MethodReference
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private MethodReference GetMethod(System.Reflection.MethodBase method)
        {
            return this.Module.ImportReference(method);
        }

        /// <summary>
        /// 插入代理并保存
        /// </summary>
        /// <returns></returns>
        public int BuildAndSave()
        {
            var interfaces = this.Module.Types.Where(item => item.IsPublic && item.IsInterface).ToArray();
            foreach (TypeDefinition type in interfaces)
            {
                var proxyType = this.GenarateProxyType(type);
                this.Module.Types.Add(proxyType);
            }

            var parameters = new WriterParameters
            {
                WriteSymbols = true
            };
            this.Module.Write(this.FileName, parameters);
            return interfaces.Length;
        }

        /// <summary>
        /// 生成代理类
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private TypeDefinition GenarateProxyType(TypeDefinition interfaceType)
        {
            var @namespace = $"System.StaticProxy.{interfaceType.Namespace}";
            var proxyTypeName = interfaceType.Name.Length > 1 && interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name;

            var proxyType = new TypeDefinition(@namespace, proxyTypeName, TypeAttributes.Public | TypeAttributes.Class);
            proxyType.Interfaces.Add(new InterfaceImplementation(interfaceType));
            proxyType.BaseType = this.GetType(typeof(HttpApiClient));

            var exceptHashSet = new HashSet<TypeDefinition>(new[]
            {
                this.GetType(typeof(IDisposable)).Resolve(),
                this.GetType(typeof(IHttpApiClient)).Resolve()
            }, TypeDefinitionComparer.Instance);

            var methodHashSet = new HashSet<MethodDefinition>();
            this.GetInterfaceMethods(interfaceType, ref exceptHashSet, ref methodHashSet);

            var fieldInterceptor = this.BuildField(proxyType, "interceptor", typeof(IApiInterceptor));
            var fieldApiMethods = this.BuildField(proxyType, "apiMethods", typeof(System.Reflection.MethodInfo[]));

            this.BuildCtor(proxyType, fieldInterceptor, fieldApiMethods);
            this.BuildMethods(proxyType, methodHashSet.ToArray(), fieldInterceptor, fieldApiMethods);

            return proxyType;
        }

        /// <summary>
        /// 递归查找接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="exceptHashSet">排除的接口类型</param>
        /// <param name="methodHashSet">收集到的方法</param>
        /// <exception cref="NotSupportedException"></exception>
        private void GetInterfaceMethods(TypeDefinition interfaceType, ref HashSet<TypeDefinition> exceptHashSet, ref HashSet<MethodDefinition> methodHashSet)
        {
            if (exceptHashSet.Add(interfaceType) == false)
            {
                return;
            }

            foreach (var item in interfaceType.Methods)
            {
                this.EnsureApiMethod(item);
                methodHashSet.Add(item);
            }

            foreach (var item in interfaceType.Interfaces)
            {
                var @interface = item.InterfaceType.Resolve();
                this.GetInterfaceMethods(@interface, ref exceptHashSet, ref methodHashSet);
            }
        }

        /// <summary>
        /// 确保方法是支持的Api接口
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private void EnsureApiMethod(MethodDefinition method)
        {
            if (method.IsGenericInstance == true)
            {
                throw new NotSupportedException("不支持泛型方法：" + method);
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException("不支持属性访问器：" + method);
            }

            var genericType = method.ReturnType;
            if (genericType.IsGenericInstance == true)
            {
                genericType = genericType.GetElementType();
            }

            var isTaskType = this.IsTypeEquals(genericType, typeof(Task<>)) || this.IsTypeEquals(genericType, typeof(ITask<>));
            if (isTaskType == false)
            {
                var message = $"返回类型必须为Task<>或ITask<>：{method}";
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.Parameters)
            {
                if (parameter.ParameterType.IsByReference == true)
                {
                    var message = $"接口参数不支持ref/out修饰：{parameter}";
                    throw new NotSupportedException(message);
                }
            }
        }

        /// <summary>
        /// 比较类型是否相等
        /// </summary>
        /// <param name="type1"></param>
        /// <param name="type2"></param>
        /// <returns></returns>
        private bool IsTypeEquals(TypeReference type1, Type type2)
        {
            return type1.FullName.GetHashCode() == this.GetType(type2).FullName.GetHashCode();
        }

        /// <summary>
        /// 生成代理类型的字段
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="type">字段类型</param>
        /// <returns></returns>
        private FieldDefinition BuildField(TypeDefinition proxyType, string fieldName, Type type)
        {
            const FieldAttributes filedAttribute = FieldAttributes.Private | FieldAttributes.InitOnly; ;
            var filed = new FieldDefinition(fieldName, filedAttribute, this.GetType(type));
            proxyType.Fields.Add(filed);
            return filed;
        }

        /// <summary>
        /// 生成代理类型的构造器
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="fieldInterceptor">拦截器字段</param>
        /// <param name="fieldApiMethods">接口方法集合字段</param>
        /// <returns></returns>
        private void BuildCtor(TypeDefinition proxyType, FieldDefinition fieldInterceptor, FieldDefinition fieldApiMethods)
        {
            // this(IApiInterceptor interceptor, MethodInfo[] methods):base(interceptor)          
            var ctor = new MethodDefinition(".ctor", MethodAttributes.Public, this.GetType(typeof(void)));
            ctor.Attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

            foreach (var item in proxyTypeCtorArgTypes)
            {
                var parameter = new ParameterDefinition(this.GetType(item));
                ctor.Parameters.Add(parameter);
            }

            var il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, this.GetMethod(baseConstructor));

            // this.interceptor = 第一个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldInterceptor);

            // this.apiMethods = 第二个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldApiMethods);

            il.Emit(OpCodes.Ret);

            proxyType.Methods.Add(ctor);
        }

        /// <summary>
        /// 生成代理类型的接口实现方法
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="apiMethods">接口方法集合</param>
        /// <param name="fieldInterceptor">拦截器字段</param>
        /// <param name="fieldApiMethods">接口方法集合字段</param>
        private void BuildMethods(TypeDefinition proxyType, MethodDefinition[] apiMethods, FieldDefinition fieldInterceptor, FieldDefinition fieldApiMethods)
        {
            const MethodAttributes implementAttribute = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.HideBySig;

            for (var i = 0; i < apiMethods.Length; i++)
            {
                var apiMethod = apiMethods[i];
                var apiParameters = apiMethod.Parameters;
                var parameterTypes = apiParameters.Select(p => p.ParameterType).ToArray();

                var impMethod = new MethodDefinition(apiMethod.Name, implementAttribute, apiMethod.ReturnType);
                foreach (var item in apiParameters)
                {
                    impMethod.Parameters.Add(item);
                }
                proxyType.Methods.Add(impMethod);

                var iL = impMethod.Body.GetILProcessor();

                // this.interceptor
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldInterceptor);

                // 加载target参数
                iL.Emit(OpCodes.Ldarg_0);

                // var method = this.apiMethods[i]
                iL.Create(OpCodes.Localloc);

                var method = new VariableDefinition(this.GetType(typeof(System.Reflection.MethodInfo)));
                impMethod.Body.Variables.Add(method);

                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldApiMethods);
                iL.Emit(OpCodes.Ldc_I4, i);
                iL.Emit(OpCodes.Ldelem_Ref);
                iL.Emit(OpCodes.Stloc, method);

                // 加载method参数
                iL.Emit(OpCodes.Ldloc, method);

                // var parameters = new object[parameters.Length]
                var parameters = new VariableDefinition(this.GetType(typeof(object[])));
                impMethod.Body.Variables.Add(parameters);

                iL.Emit(OpCodes.Ldc_I4, apiParameters.Count);
                iL.Emit(OpCodes.Newarr, this.GetType(typeof(object)));
                iL.Emit(OpCodes.Stloc, parameters);

                for (var j = 0; j < apiParameters.Count; j++)
                {
                    iL.Emit(OpCodes.Ldloc, parameters);
                    iL.Emit(OpCodes.Ldc_I4, j);
                    iL.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameterTypes[j];
                    if (parameterType.IsValueType || parameterType.IsGenericParameter)
                    {
                        iL.Emit(OpCodes.Box, parameterType);
                    }
                    iL.Emit(OpCodes.Stelem_Ref);
                }

                // 加载parameters参数
                iL.Emit(OpCodes.Ldloc, parameters);

                // Intercep(this, method, parameters)
                iL.Emit(OpCodes.Callvirt, this.GetMethod(interceptMethod));

                iL.Emit(OpCodes.Ret);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.Module.Dispose();
        }
    }
}
