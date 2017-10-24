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
        /// 接口类型与代理类型的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> cache = new ConcurrentDictionary<Type, Type>();

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

            var proxyType = cache.GetOrAdd(interfaceType, t => GenerateProxyType(t, apiMethods));
            return Activator.CreateInstance(proxyType, interceptor, apiMethods) as T;
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
            var moduleName = Guid.NewGuid().ToString();
            var proxyTypeName = interfaceType.Namespace + "." + interfaceType.Name;

            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
            var typeBuilder = moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Class, typeof(MarshalByRefObject));
            typeBuilder.AddInterfaceImplementation(interfaceType);

            return ImplementInterface(typeBuilder, apiMethods);
        }

        /// <summary>
        /// 实现接口
        /// </summary>
        /// <param name="typeBuilder">类型生成器</param>
        /// <param name="apiMethods">接口的方法</param>
        /// <returns></returns>
        private static Type ImplementInterface(TypeBuilder typeBuilder, MethodInfo[] apiMethods)
        {
            // 字段
            var fieldInterceptor = typeBuilder.DefineField("interceptor", typeof(IApiInterceptor), FieldAttributes.Private | FieldAttributes.InitOnly);
            var fieldApiMethods = typeBuilder.DefineField("apiMethods", typeof(MethodInfo[]), FieldAttributes.Private | FieldAttributes.InitOnly);

            // IApiInterceptor构造器
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
                var methodIL = methodBuilder.GetILGenerator();

                // this.interceptor
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, fieldInterceptor);

                // this
                methodIL.Emit(OpCodes.Ldarg_0);

                //var method = this.apiMethods[i]
                var method = methodIL.DeclareLocal(typeof(MethodInfo));
                methodIL.Emit(OpCodes.Ldarg_0);
                methodIL.Emit(OpCodes.Ldfld, fieldApiMethods);
                methodIL.Emit(OpCodes.Ldc_I4, i);
                methodIL.Emit(OpCodes.Ldelem_Ref);
                methodIL.Emit(OpCodes.Stloc, method);
                methodIL.Emit(OpCodes.Ldloc, method);

                // var parameters = new object[parameters.Length]
                var parameters = methodIL.DeclareLocal(typeof(object[]));
                methodIL.Emit(OpCodes.Ldc_I4, apiParameters.Length);
                methodIL.Emit(OpCodes.Newarr, typeof(object));
                methodIL.Emit(OpCodes.Stloc, parameters);

                for (var j = 0; j < apiParameters.Length; j++)
                {
                    methodIL.Emit(OpCodes.Ldloc, parameters);
                    methodIL.Emit(OpCodes.Ldc_I4, j);
                    methodIL.Emit(OpCodes.Ldarg, j + 1);

                    var parameterType = parameterTypes[j];
                    if (parameterType.IsValueType || parameterType.IsGenericParameter)
                    {
                        methodIL.Emit(OpCodes.Box, parameterType);
                    }
                    methodIL.Emit(OpCodes.Stelem_Ref);
                }
                methodIL.Emit(OpCodes.Ldloc, parameters);

                // call Intercept 
                methodIL.Emit(OpCodes.Callvirt, typeof(IApiInterceptor).GetMethod("Intercept"));

                if (apiMethod.ReturnType == typeof(void))
                {
                    methodIL.Emit(OpCodes.Pop);
                }
                methodIL.Emit(OpCodes.Ret);
            }
            return typeBuilder.CreateType();
        }
    }
}
