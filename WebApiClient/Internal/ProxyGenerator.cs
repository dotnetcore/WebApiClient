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
    /// 代理生成器
    /// </summary>
    static class ProxyGenerator
    {
        /// <summary>
        /// IApiInterceptor的Intercept方法
        /// </summary>
        private static readonly MethodInfo interceptMethod = typeof(IApiInterceptor).GetMethod("Intercept");

        /// <summary>
        /// 接口类型与代理类型的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> proxyCache = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 程序域的AssemblyBuilder缓存
        /// </summary>
        private static readonly ConcurrentDictionary<AppDomain, AssemblyBuilder> assemblyCache = new ConcurrentDictionary<AppDomain, AssemblyBuilder>();

        /// <summary>
        /// 创建接口的代理实例
        /// </summary>
        /// <typeparam name="T">接口殴类型</typeparam>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        public static T CreateInterfaceProxyWithoutTarget<T>(IApiInterceptor interceptor) where T : class
        {
            var interfaceType = typeof(T);
            var apiMethods = interfaceType.GetInterfaceAllMethods();

            var proxyType = proxyCache.GetOrAdd(interfaceType, t => GenerateProxyType(t, apiMethods));
            return Activator.CreateInstance(proxyType, new object[] { interceptor, apiMethods }) as T;
        }

        /// <summary>
        /// 生成接口的代理类
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="apiMethods">拦截的方法</param>
        /// <returns></returns>
        private static Type GenerateProxyType(Type interfaceType, MethodInfo[] apiMethods)
        {
            const string assemblyName = "ApiProxyAssembly";
            var moduleName = string.Format("{0}_{1}.dll", interfaceType.Name, Guid.NewGuid());
            var proxyTypeName = interfaceType.FullName;

            var assemblyBuilder = assemblyCache.GetOrAdd(AppDomain.CurrentDomain, domain => domain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run));
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            var typeBuilder = moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Class, typeof(MarshalByRefObject));
            typeBuilder.AddInterfaceImplementation(interfaceType);

            return ImplementApiMethods(typeBuilder, apiMethods);
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
            var fieldInterceptor = typeBuilder.DefineField("interceptor", typeof(IApiInterceptor), FieldAttributes.Private | FieldAttributes.InitOnly);
            var fieldApiMethods = typeBuilder.DefineField("apiMethods", typeof(MethodInfo[]), FieldAttributes.Private | FieldAttributes.InitOnly);

            // 构造器
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IApiInterceptor), typeof(MethodInfo[]) });
            var ctroIL = ctorBuilder.GetILGenerator();
            ctroIL.Emit(OpCodes.Ldarg_0);
            ctroIL.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));

            ctroIL.Emit(OpCodes.Ldarg_0);
            ctroIL.Emit(OpCodes.Ldarg_1);
            ctroIL.Emit(OpCodes.Stfld, fieldInterceptor);

            ctroIL.Emit(OpCodes.Ldarg_0);
            ctroIL.Emit(OpCodes.Ldarg_2);
            ctroIL.Emit(OpCodes.Stfld, fieldApiMethods);

            ctroIL.Emit(OpCodes.Ret);

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

                //var method = this.apiMethods[i]
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
