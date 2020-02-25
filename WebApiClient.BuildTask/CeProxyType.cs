using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System.Linq;
using System.Reflection;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Indicates the proxy type of the interface
    /// </summary>
    class CeProxyType : CeMetadata
    {
        /// <summary>
        /// interface
        /// </summary>
        private readonly CeInterface @interface;

        /// <summary>
        /// Proxy type generator
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="interface">interface</param>
        public CeProxyType(CeAssembly assembly, CeInterface @interface)
           : base(assembly)
        {
            this.@interface = @interface;
        }

        /// <summary>
        /// Conversion to TypeDefinition
        /// </summary>
        /// <returns></returns>
        public TypeDefinition Build()
        {
            var proxyType = this.@interface.MakeProxyType();
            var interceptorType = this.ImportType<IApiInterceptor>();
            var apiMethodsType = this.ImportType<MethodInfo>().MakeArrayType();

            var fieldInterceptor = this.BuildField(proxyType, "interceptor", interceptorType);
            var fieldApiMethods = this.BuildField(proxyType, "apiMethods", apiMethodsType);
            this.BuildCtor(proxyType, fieldInterceptor, fieldApiMethods);

            var apiMethos = this.@interface.GetAllApis();
            this.BuildMethods(proxyType, apiMethos, fieldInterceptor, fieldApiMethods);

            return proxyType;
        }


        /// <summary>
        /// Generate fields for proxy type
        /// </summary>
        /// <param name="proxyType">Proxy type</param>
        /// <param name="fieldName">Field Name</param>
        /// <param name="type">Field Type</param>
        /// <returns></returns>
        private FieldDefinition BuildField(TypeDefinition proxyType, string fieldName, TypeReference type)
        {
            const Mono.Cecil.FieldAttributes filedAttribute = Mono.Cecil.FieldAttributes.Private | Mono.Cecil.FieldAttributes.InitOnly; ;
            var filed = new FieldDefinition(fieldName, filedAttribute, type);
            proxyType.Fields.Add(filed);
            return filed;
        }

        /// <summary>
        /// Constructor for proxy type
        /// </summary>
        /// <param name="proxyType">Proxy type</param>
        /// <param name="fieldInterceptor">Interceptor field</param>
        /// <param name="fieldApiMethods">Interface method collection fields</param>
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

            var baseConstructor = this.ImportMethod<HttpApi>(item => item.IsConstructor);
            il.Emit(OpCodes.Call, baseConstructor);

            // this.interceptor = First parameter
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldInterceptor);

            // this.apiMethods = Second parameter
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Stfld, fieldApiMethods);

            il.Emit(OpCodes.Ret);

            proxyType.Methods.Add(ctor);
        }

        /// <summary>
        /// Method for generating interface of proxy type
        /// </summary>
        /// <param name="proxyType">Proxy type</param>
        /// <param name="apiMethods">Interface method collection</param>
        /// <param name="fieldInterceptor">Interceptor field</param>
        /// <param name="fieldApiMethods">Interface method collection fields</param>
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
                    var parameter = new ParameterDefinition(item.ParameterType);
                    implMethod.Parameters.Add(parameter);
                }
                proxyType.Methods.Add(implMethod);

                var iL = implMethod.Body.GetILProcessor();

                // this.interceptor
                iL.Emit(OpCodes.Ldarg_0);
                iL.Emit(OpCodes.Ldfld, fieldInterceptor);

                // Load target parameter
                iL.Emit(OpCodes.Ldarg_0);

                // Load method parameters this.apiMethods[i]
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

                // Load parameters
                iL.Emit(OpCodes.Ldloc, parameters);

                // Intercep(this, method, parameters)
                var interceptMethod = this.ImportMethod<IApiInterceptor>(nameof(IApiInterceptor.Intercept));
                iL.Emit(OpCodes.Callvirt, interceptMethod);

                iL.Emit(OpCodes.Castclass, apiMethod.ReturnType);
                iL.Emit(OpCodes.Ret);
            }
        }
    }
}
