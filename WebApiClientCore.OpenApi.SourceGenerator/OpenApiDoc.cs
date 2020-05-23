using NJsonSchema.CodeGeneration;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebApiClientCore.OpenApi.SourceGenerator
{
    /// <summary>
    /// 表示OpenApi描述
    /// </summary>
    public class OpenApiDoc
    {
        private readonly CSharpTypeResolver resolver;

        /// <summary>
        /// 获取Swagger文档
        /// </summary>
        public OpenApiDocument Document { get; private set; }

        /// <summary>
        /// 获取Swagger设置项
        /// </summary>
        public HttpApiSettings Settings { get; private set; }

        /// <summary>
        /// OpenApi描述
        /// </summary>
        /// <param name="options">选项</param>
        public OpenApiDoc(OpenApiDocOptions options)
            : this(GetDocument(options.OpenApi))
        {
            if (string.IsNullOrEmpty(options.Namespace) == false)
            {
                this.Settings.NameSpace = options.Namespace;             
                this.Settings.CSharpGeneratorSettings.Namespace = options.Namespace;
            }
        }

        /// <summary>
        /// OpenApi描述
        /// </summary>
        /// <param name="document">Swagger文档</param>
        public OpenApiDoc(OpenApiDocument document)
        {
            this.Document = document;
            this.Settings = new HttpApiSettings();

            this.resolver = CSharpGeneratorBase
                .CreateResolverWithExceptionSchema(this.Settings.CSharpGeneratorSettings, document);
        }

        /// <summary>
        /// 获取OpenApi描述文档
        /// </summary>
        /// <param name="openApi"></param>
        /// <returns></returns>
        private static OpenApiDocument GetDocument(string openApi)
        {
            Console.WriteLine($"正在分析OpenApi：{openApi}");
            if (Uri.TryCreate(openApi, UriKind.Absolute, out var _) == true)
            {
                return OpenApiDocument.FromUrlAsync(openApi).Result;
            }
            else
            {
                return OpenApiDocument.FromFileAsync(openApi).Result;
            }
        }

        /// <summary>
        /// 生成代码并保存到文件
        /// </summary>
        public void GenerateFiles()
        {
            var dir = Path.Combine("output", this.Settings.NameSpace);
            var apisPath = Path.Combine(dir, "HttpApis");
            var modelsPath = Path.Combine(dir, "HttpModels");

            Directory.CreateDirectory(apisPath);
            Directory.CreateDirectory(modelsPath);

            var apis = new HttpApiProvider(this).GetHttpApis();
            foreach (var api in apis)
            {
                var file = Path.Combine(apisPath, $"{api.TypeName}.cs");
                File.WriteAllText(file, api.ToString(), Encoding.UTF8);
                Console.WriteLine($"输出接口文件：{file}");
            }

            var models = new HttpModelProvider(this).GetHttpModels();
            foreach (var model in models)
            {
                var file = Path.Combine(modelsPath, $"{model.TypeName}.cs");
                File.WriteAllText(file, model.ToString(), Encoding.UTF8);
                Console.WriteLine($"输出模型文件：{file}");
            }

            Console.WriteLine($"共输出{apis.Length + models.Length}个文件..");
        }


        /// <summary>
        /// 表示HttpApi提供者
        /// </summary>
        private class HttpApiProvider : CSharpControllerGenerator
        {
            /// <summary>
            /// openApi
            /// </summary>
            private readonly OpenApiDoc openApi;

            /// <summary>
            /// api列表
            /// </summary>
            private readonly List<HttpApi> httpApiList = new List<HttpApi>();

            /// <summary>
            /// HttpApi提供者
            /// </summary>
            /// <param name="openApi"></param>
            public HttpApiProvider(OpenApiDoc openApi)
                : base(openApi.Document, openApi.Settings, openApi.resolver)
            {
                this.openApi = openApi;
            }

            /// <summary>
            /// 获取所有HttpApi描述模型
            /// </summary>
            /// <returns></returns>
            public HttpApi[] GetHttpApis()
            {
                this.httpApiList.Clear();
                this.GenerateFile();
                return this.httpApiList.ToArray();
            }

            /// <summary>
            /// 生成客户端调用代码
            /// 但实际只为了获得HttpApi实例
            /// </summary>
            /// <param name="controllerName"></param>
            /// <param name="controllerClassName"></param>
            /// <param name="operations"></param>
            /// <returns></returns>
            protected override IEnumerable<CodeArtifact> GenerateClientTypes(string controllerName, string controllerClassName, IEnumerable<CSharpOperationModel> operations)
            {
                var model = new HttpApi(controllerClassName, operations, this.openApi.Document, this.openApi.Settings);
                this.httpApiList.Add(model);
                return new CodeArtifact[0];
            }


            /// <summary>
            /// 生成文件
            /// 这里不生成
            /// </summary>
            /// <param name="clientTypes"></param>
            /// <param name="dtoTypes"></param>
            /// <param name="outputType"></param>
            /// <returns></returns>
            protected override string GenerateFile(IEnumerable<CodeArtifact> clientTypes, IEnumerable<CodeArtifact> dtoTypes, ClientGeneratorOutputType outputType)
            {
                return string.Empty;
            }

            /// <summary>
            /// 创建操作描述
            /// 这里创建HttpApiOperation
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="settings"></param>
            /// <returns></returns>
            protected override CSharpOperationModel CreateOperationModel(OpenApiOperation operation, ClientGeneratorBaseSettings settings)
            {
                return new HttpApiMethod(operation, (CSharpGeneratorBaseSettings)settings, this, (CSharpTypeResolver)Resolver);
            }
        }

        /// <summary>
        /// 表示HttpModel提供者
        /// </summary>
        private class HttpModelProvider : CSharpGenerator
        {
            /// <summary>
            /// swagger
            /// </summary>
            private readonly OpenApiDoc swagger;

            /// <summary>
            /// HttpModel提供者
            /// </summary>
            /// <param name="swagger"></param>
            public HttpModelProvider(OpenApiDoc swagger)
                : base(swagger.Document, swagger.Settings.CSharpGeneratorSettings, swagger.resolver)
            {
                this.swagger = swagger;
            }

            /// <summary>
            /// 获取所有HttpModels
            /// </summary>
            /// <returns></returns>
            public HttpModel[] GetHttpModels()
            {
                return this.GenerateTypes()
                    .Select(item => new HttpModel(item, this.swagger.Settings.NameSpace))
                    .ToArray();
            }
        }
    }
}