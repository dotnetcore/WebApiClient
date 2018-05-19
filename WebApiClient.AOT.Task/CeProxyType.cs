using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApiClient.AOT.Task
{
    /// <summary>
    /// 表示接口的代理类型
    /// </summary>
    class CeProxyType : CeMetadata
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly CeInterface @interface;

        /// <summary>
        /// 代理类型生成器
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="interface">接口</param>
        public CeProxyType(CeAssembly assembly, CeInterface @interface)
           : base(assembly)
        {
            this.@interface = @interface;
        }

        /// <summary>
        /// 转换为TypeDefinition
        /// </summary>
        /// <returns></returns>
        public TypeDefinition Build()
        {
            var proxyType = this.@interface.MakeProxyType();
            var interceptorType = this.ImportTypeReference<IApiInterceptor>();
            var apiMethodsType = this.ImportTypeReference<MemberInfo>().MakeArrayType();

            var fieldInterceptor = this.BuildField(proxyType, "interceptor", interceptorType);
            var fieldApiMethods = this.BuildField(proxyType, "apiMethods", apiMethodsType);
            this.BuildCtor(proxyType, fieldInterceptor, fieldApiMethods);

            var apiMethos = this.@interface.GetAllApis();
            this.BuildMethods(proxyType, apiMethos, fieldInterceptor, fieldApiMethods);

            return proxyType;
        }


        /// <summary>
        /// 生成代理类型的字段
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="type">字段类型</param>
        /// <returns></returns>
        private FieldDefinition BuildField(TypeDefinition proxyType, string fieldName, TypeReference type)
        {
            const Mono.Cecil.FieldAttributes filedAttribute = Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.InitOnly; ;
            var filed = new FieldDefinition(fieldName, filedAttribute, type);
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
            // void .ctor(IApiInterceptor interceptor, MethodInfo[] methods):base(interceptor)
            var ctor = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.Public, this.TypeSystem.Void)
            {
                Attributes = Mono.Cecil.MethodAttributes.Public
                | Mono.Cecil.MethodAttributes.HideBySig
                | Mono.Cecil.MethodAttributes.SpecialName
                | Mono.Cecil.MethodAttributes.RTSpecialName
            };

            var parameterInterceptor = new ParameterDefinition(fieldInterceptor.FieldType);
            var parameterApiMethods = new ParameterDefinition(fieldApiMethods.FieldType);
            ctor.Parameters.Add(parameterInterceptor);
            ctor.Parameters.Add(parameterApiMethods);

            var il = ctor.Body.GetILProcessor();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);

            var baseConstructor = this.ImportMethodReference<HttpApiClient>(item => item.IsConstructor);
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
            const Mono.Cecil.MethodAttributes implementAttribute = Mono.Cecil.MethodAttributes.Public
                | Mono.Cecil.MethodAttributes.Virtual
                | Mono.Cecil.MethodAttributes.Final
                | Mono.Cecil.MethodAttributes.NewSlot
                | Mono.Cecil.MethodAttributes.HideBySig;

            for (var i = 0; i < apiMethods.Length; i++)
            {
                var apiMethod = apiMethods[i];
                var apiParameters = apiMethod.Parameters;
                var parameterTypes = apiParameters.Select(p => p.ParameterType).ToArray();

                var implMethod = new MethodDefinition(apiMethod.Name, implementAttribute, apiMethod.ReturnType);
                foreach (var item in apiParameters)
                {
                    implMethod.Parameters.Add(item);
                }
                proxyType.Methods.Add(implMethod);

                var iL = implMethod.Body.GetILProcessor();

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
                var parameters = new VariableDefinition(new ArrayType(this.TypeSystem.Object));
                implMethod.Body.Variables.Add(parameters);

                iL.Emit(OpCodes.Ldc_I4, apiParameters.Count);
                iL.Emit(OpCodes.Newarr, this.TypeSystem.Object);
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
                var interceptMethod = this.ImportMethodReference<IApiInterceptor>(nameof(IApiInterceptor.Intercept));
                iL.Emit(OpCodes.Callvirt, interceptMethod);

                iL.Emit(OpCodes.Castclass, apiMethod.ReturnType);
                iL.Emit(OpCodes.Ret);
            }
        }
    }
}
