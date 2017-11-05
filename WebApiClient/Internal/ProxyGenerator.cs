using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 接口的代理类型生成器
    /// 不支持泛型方法
    /// 不支持ref/out参数
    /// </summary>
    static class ProxyGenerator
    {
        /// <summary>
        /// IApiInterceptor的Intercept方法
        /// </summary>
        private static readonly MethodInfo interceptMethod = typeof(IApiInterceptor).GetMethod("Intercept");

        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(MethodInfo[]) };

        /// <summary>
        /// 应用程序池下的程序集创建器
        /// </summary>
        private static readonly AssemblyBuilder domainAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ApiProxyAssembly"), AssemblyBuilderAccess.Run);

        /// <summary>
        /// 接口类型与代理类型的构造器缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> proxyTypeCtorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        /// <summary>
        /// 模块与模块创建器的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Module, ModuleBuilder> moduleModuleBuilderCache = new ConcurrentDictionary<Module, ModuleBuilder>();

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <typeparam name="T">接口殴类型</typeparam>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static T CreateInterfaceProxyWithoutTarget<T>(IApiInterceptor interceptor) where T : class
        {
            var interfaceType = typeof(T);
            var apiMethods = interfaceType.GetApiAllMethods();
            var proxyTypeCtor = proxyTypeCtorCache.GetOrAdd(interfaceType, type => GenerateProxyTypeCtor(type, apiMethods));
            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods }) as T;
        }

        /// <summary>
        /// 生成接口的代理类
        /// 返回其构造器
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="apiMethods">拦截的方法</param>
        /// <returns></returns>
        private static ConstructorInfo GenerateProxyTypeCtor(Type interfaceType, MethodInfo[] apiMethods)
        {
            var moduleBuilder = moduleModuleBuilderCache.GetOrAdd(interfaceType.Module, module => domainAssemblyBuilder.DefineDynamicModule(module.Name));
            var typeBuilder = moduleBuilder.DefineType(interfaceType.FullName, TypeAttributes.Class);
            typeBuilder.AddInterfaceImplementation(interfaceType);

            var proxyType = ImplementApiMethods(typeBuilder, apiMethods);
            return proxyType.GetConstructor(proxyTypeCtorArgTypes);
        }

        /// <summary>
        /// 实现接口方法
        /// 返回代理类型
        /// </summary>
        /// <param name="typeBuilder">类型生成器</param>
        /// <param name="apiMethods">接口的所有方法</param>
        /// <returns></returns>
        private static Type ImplementApiMethods(TypeBuilder typeBuilder, MethodInfo[] apiMethods)
        {
            // 字段
            var filedAttribute = FieldAttributes.Private | FieldAttributes.InitOnly;
            var fieldInterceptor = typeBuilder.DefineField("interceptor", typeof(IApiInterceptor), filedAttribute);
            var fieldApiMethods = typeBuilder.DefineField("apiMethods", typeof(MethodInfo[]), filedAttribute);

            // 构造器
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, proxyTypeCtorArgTypes);
            var ctorIL = ctorBuilder.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));

            // this.interceptor = 第一个参数
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, fieldInterceptor);

            // this.apiMethods = 第二个参数
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_2);
            ctorIL.Emit(OpCodes.Stfld, fieldApiMethods);

            ctorIL.Emit(OpCodes.Ret);

            // 接口实现
            for (var i = 0; i < apiMethods.Length; i++)
            {
                var apiMethod = apiMethods[i];
                var apiParameters = apiMethod.GetParameters();
                var parameterTypes = apiParameters.Select(p => p.ParameterType).ToArray();
                var implementAttribute = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.HideBySig;

                var methodBuilder = typeBuilder.DefineMethod(apiMethod.Name, implementAttribute, CallingConventions.Standard, apiMethod.ReturnType, parameterTypes);
                var apiMethodIL = methodBuilder.GetILGenerator();

                // this.interceptor
                apiMethodIL.Emit(OpCodes.Ldarg_0);
                apiMethodIL.Emit(OpCodes.Ldfld, fieldInterceptor);

                // 加载target参数
                apiMethodIL.Emit(OpCodes.Ldarg_0);

                // var method = this.apiMethods[i]
                var method = apiMethodIL.DeclareLocal(typeof(MethodInfo));
                apiMethodIL.Emit(OpCodes.Ldarg_0);
                apiMethodIL.Emit(OpCodes.Ldfld, fieldApiMethods);
                apiMethodIL.Emit(OpCodes.Ldc_I4, i);
                apiMethodIL.Emit(OpCodes.Ldelem_Ref);
                apiMethodIL.Emit(OpCodes.Stloc, method);

                // 加载method参数
                apiMethodIL.Emit(OpCodes.Ldloc, method);

                // var parameters = new object[parameters.Length]
                var parameters = apiMethodIL.DeclareLocal(typeof(object[]));
                apiMethodIL.Emit(OpCodes.Ldc_I4, apiParameters.Length);
                apiMethodIL.Emit(OpCodes.Newarr, typeof(object));
                apiMethodIL.Emit(OpCodes.Stloc, parameters);

                for (var j = 0; j < apiParameters.Length; j++)
                {
                    apiMethodIL.Emit(OpCodes.Ldloc, parameters);
                    apiMethodIL.Emit(OpCodes.Ldc_I4, j);
                    apiMethodIL.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameterTypes[j];
                    if (parameterType.IsValueType || parameterType.IsGenericParameter)
                    {
                        apiMethodIL.Emit(OpCodes.Box, parameterType);
                    }
                    apiMethodIL.Emit(OpCodes.Stelem_Ref);
                }

                // 加载parameters参数
                apiMethodIL.Emit(OpCodes.Ldloc, parameters);

                // Intercep(this, method, parameters)
                apiMethodIL.Emit(OpCodes.Callvirt, interceptMethod);

                if (apiMethod.ReturnType == typeof(void))
                {
                    apiMethodIL.Emit(OpCodes.Pop);
                }
                apiMethodIL.Emit(OpCodes.Ret);
            }
            return typeBuilder.CreateType();
        }
    }
}
