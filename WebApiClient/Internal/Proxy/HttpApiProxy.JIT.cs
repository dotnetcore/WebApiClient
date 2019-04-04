#if JIT
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi代理描述
    /// 提供HttpApi代理类的实例化
    /// </summary>
    partial class HttpApiProxy
    {
        /// <summary>
        /// IApiInterceptor的Intercept方法
        /// </summary>
        private static readonly MethodInfo interceptMethod = typeof(IApiInterceptor).GetMethod(nameof(IApiInterceptor.Intercept));

        /// <summary>
        /// HttpApi的构造器
        /// </summary>
        private static readonly ConstructorInfo baseConstructor = typeof(HttpApi).GetConstructor(new Type[] { typeof(IApiInterceptor) });

        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(MethodInfo[]) };

        /// <summary>
        /// 接口类型与代理描述缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, HttpApiProxy> interfaceProxyCache = new ConcurrentCache<Type, HttpApiProxy>();

        /// <summary>
        /// 返回HttpApi代理类的实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static HttpApi CreateInstance(Type interfaceType, IApiInterceptor interceptor)
        {
            // 接口的实现在动态程序集里，所以接口必须为public修饰才可以创建代理类并实现此接口            
            if (interfaceType.GetTypeInfo().IsVisible == false)
            {
                var message = $"WebApiClient.JIT不支持非public接口定义：{interfaceType}";
                throw new NotSupportedException(message);
            }

            var proxy = interfaceProxyCache.GetOrAdd(interfaceType, @interface =>
            {
                var apiMethods = @interface.GetAllApiMethods();
                var proxyType = BuildProxyType(@interface, apiMethods);
                return new HttpApiProxy(proxyType, apiMethods);
            });

            return proxy.CreateInstance(interceptor);
        }

        /// <summary>
        /// 返回创建的接口的代理代理类型
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="apiMethods">接口方法集合</param>
        /// <returns></returns>
        private static Type BuildProxyType(Type interfaceType, MethodInfo[] apiMethods)
        {
            var moduleName = interfaceType.GetTypeInfo().Module.Name;
            var assemblyName = new AssemblyName(Guid16.NewGuid16().ToString());

            var module = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(moduleName);

            var builder = module.DefineType(interfaceType.FullName, TypeAttributes.Class, typeof(HttpApi));
            builder.AddInterfaceImplementation(interfaceType);

            var fieldInterceptor = BuildField(builder, "interceptor", typeof(IApiInterceptor));
            var fieldApiMethods = BuildField(builder, "apiMethods", typeof(MethodInfo[]));

            BuildCtor(builder, fieldInterceptor, fieldApiMethods);
            BuildMethods(builder, apiMethods, fieldInterceptor, fieldApiMethods);

            return builder.CreateTypeInfo().AsType();
        }

        /// <summary>
        /// 生成代理类型的字段
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        private static FieldBuilder BuildField(TypeBuilder builder, string fieldName, Type fieldType)
        {
            const FieldAttributes filedAttribute = FieldAttributes.Private | FieldAttributes.InitOnly;
            return builder.DefineField(fieldName, fieldType, filedAttribute);
        }

        /// <summary>
        /// 生成代理类型的构造器
        /// </summary>
        /// <param name="builder">类型生成器</param>
        /// <param name="fieldInterceptor">拦截器字段</param>
        /// <param name="fieldApiMethods">接口方法集合字段</param>
        /// <returns></returns>
        private static void BuildCtor(TypeBuilder builder, FieldBuilder fieldInterceptor, FieldBuilder fieldApiMethods)
        {
            // .ctor(IApiInterceptor interceptor, MethodInfo[] methods):base(interceptor)          
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
        private static void BuildMethods(TypeBuilder builder, MethodInfo[] apiMethods, FieldBuilder fieldInterceptor, FieldBuilder fieldApiMethods)
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

                // 加载method参数 this.apiMethods[i]
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldApiMethods);
                iL.Emit(OpCodes.Ldc_I4, i);
                iL.Emit(OpCodes.Ldelem_Ref);

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
                    if (parameterType.GetTypeInfo().IsValueType || parameterType.IsGenericParameter)
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

                iL.Emit(OpCodes.Castclass, apiMethod.ReturnType);
                iL.Emit(OpCodes.Ret);
            }
        }
    }
}
#endif