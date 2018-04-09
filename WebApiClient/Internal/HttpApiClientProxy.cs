using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApiClient代理类生成
    /// 不支持泛型方法的接口
    /// 不支持ref/out参数的接口
    /// </summary>
    static class HttpApiClientProxy
    {
        /// <summary>
        /// IApiInterceptor的Intercept方法
        /// </summary>
        private static readonly MethodInfo interceptMethod = typeof(IApiInterceptor).GetMethod("Intercept");

        /// <summary>
        /// HttpApiClient的构造器
        /// </summary>
        private static readonly ConstructorInfo baseConstructor = typeof(HttpApiClient).GetConstructor(new Type[] { typeof(IApiInterceptor) });

        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(MethodInfo[]) };

        /// <summary>
        /// 程序集HashCode^模块HashCode与模块创建器的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<int, ModuleBuilder> hashCodeModuleBuilderCache = new ConcurrentDictionary<int, ModuleBuilder>();

        /// <summary>
        /// 接口类型与代理类型的构造器缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, ConstructorInfo> proxyTypeCtorCache = new ConcurrentCache<Type, ConstructorInfo>();

        /// <summary>
        /// 创建HttpApiClient代理类
        /// 并实现指定的接口
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static object CreateProxyWithInterface(Type interfaceType, IApiInterceptor interceptor)
        {
            var apiMethods = interfaceType.GetAllApiMethods();
            var proxyTypeCtor = proxyTypeCtorCache.GetOrAdd(
                interfaceType,
                @interface => @interface.ImplementAsHttpApiClient(apiMethods));

            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods });
        }

        /// <summary>
        /// 继承HttpApiClient并实现接口
        /// 并返回代理类的构造器
        /// 对于相同的interfaceType，不允许并发执行
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="apiMethods">接口方法集合</param>
        /// <returns></returns>
        private static ConstructorInfo ImplementAsHttpApiClient(this Type interfaceType, MethodInfo[] apiMethods)
        {
            var moduleName = interfaceType.Module.Name;
            var hashCode = interfaceType.Assembly.GetHashCode() ^ interfaceType.Module.GetHashCode();

            // 每个动态集下面只会有一个模块
            var moduleBuilder = hashCodeModuleBuilderCache.GetOrAdd(hashCode, (hash) =>
            {
                return AssemblyBuilder
                .DefineDynamicAssembly(new AssemblyName(hash.ToString()), AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);
            });

            var builder = moduleBuilder.DefineType(interfaceType.FullName, TypeAttributes.Class, typeof(HttpApiClient));
            builder.AddInterfaceImplementation(interfaceType);
            return builder.BuildProxyType(apiMethods);
        }

        /// <summary>
        /// 生成代理类型并实现相关方法
        /// 并返回其构造器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="apiMethods">接口方法集合</param>
        /// <returns></returns>
        private static ConstructorInfo BuildProxyType(this TypeBuilder builder, MethodInfo[] apiMethods)
        {
            var fieldInterceptor = builder.BuildField("interceptor", typeof(IApiInterceptor));
            var fieldApiMethods = builder.BuildField("apiMethods", typeof(MethodInfo[]));

            builder.BuildCtor(fieldInterceptor, fieldApiMethods);
            builder.BuildMethods(apiMethods, fieldInterceptor, fieldApiMethods);

            var proxyType = builder.CreateTypeInfo();
            return proxyType.GetConstructor(proxyTypeCtorArgTypes);
        }

        /// <summary>
        /// 生成代理类型的字段
        /// </summary>
        /// <param name="typeBuilder">类型生成器</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="type">字段类型</param>
        /// <returns></returns>
        private static FieldBuilder BuildField(this TypeBuilder typeBuilder, string fieldName, Type type)
        {
            const FieldAttributes filedAttribute = FieldAttributes.Private | FieldAttributes.InitOnly;
            return typeBuilder.DefineField(fieldName, type, filedAttribute);
        }

        /// <summary>
        /// 生成代理类型的构造器
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="fieldInterceptor">拦截器字段</param>
        /// <param name="fieldApiMethods">接口方法集合字段</param>
        /// <returns></returns>
        private static void BuildCtor(this TypeBuilder builder, FieldBuilder fieldInterceptor, FieldBuilder fieldApiMethods)
        {
            // this(IApiInterceptor interceptor, MethodInfo[] methods):base(interceptor)          
            var ctor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, proxyTypeCtorArgTypes);

            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, baseConstructor);

            // this.interceptor = 第一个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldInterceptor);

            // this.apiMethods = 第二个参数
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldApiMethods);

            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 生成代理类型的接口实现方法
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="apiMethods">接口方法集合</param>
        /// <param name="fieldInterceptor">拦截器字段</param>
        /// <param name="fieldApiMethods">接口方法集合字段</param>
        private static void BuildMethods(this TypeBuilder builder, MethodInfo[] apiMethods, FieldBuilder fieldInterceptor, FieldBuilder fieldApiMethods)
        {
            const MethodAttributes implementAttribute = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.HideBySig;

            for (var i = 0; i < apiMethods.Length; i++)
            {
                var apiMethod = apiMethods[i];
                var apiParameters = apiMethod.GetParameters();
                var parameterTypes = apiParameters.Select(p => p.ParameterType).ToArray();

                var iL = builder
                    .DefineMethod(apiMethod.Name, implementAttribute, CallingConventions.Standard, apiMethod.ReturnType, parameterTypes)
                    .GetILGenerator();

                // this.interceptor
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldInterceptor);

                // 加载target参数
                iL.Emit(OpCodes.Ldarg_0);

                // var method = this.apiMethods[i]
                var method = iL.DeclareLocal(typeof(MethodInfo));
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldApiMethods);
                iL.Emit(OpCodes.Ldc_I4, i);
                iL.Emit(OpCodes.Ldelem_Ref);
                iL.Emit(OpCodes.Stloc, method);

                // 加载method参数
                iL.Emit(OpCodes.Ldloc, method);

                // var parameters = new object[parameters.Length]
                var parameters = iL.DeclareLocal(typeof(object[]));
                iL.Emit(OpCodes.Ldc_I4, apiParameters.Length);
                iL.Emit(OpCodes.Newarr, typeof(object));
                iL.Emit(OpCodes.Stloc, parameters);

                for (var j = 0; j < apiParameters.Length; j++)
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
                iL.Emit(OpCodes.Callvirt, interceptMethod);

                if (apiMethod.ReturnType == typeof(void))
                {
                    iL.Emit(OpCodes.Pop);
                }
                iL.Emit(OpCodes.Ret);
            }
        }
    }
}
