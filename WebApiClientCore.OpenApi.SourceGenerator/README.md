## WebApiClientCore.OpenApi.SourceGenerator
 
> 将OpenApi的本地或远程文档解析生成WebApiClientCore的接口定义代码文件

### 1.1 命令介绍
```
  -o OpenApi, --openapi=OpenApi          Required. openApi的json本地文件路径或远程Uri地址
  -n Namespace, --namespace=Namespace    代码的命名空间，如WebApiClientCore
  --help                                 Display this help screen.
```
### 1.2 工作流程
1. 使用NSwag解析OpenApi的json得到OpenApiDocument对象
2. 使用RazorEngine将OpenApiDocument传入cshtml模板编译得到html
3. 使用XDocument将html的文本代码提取，得到WebApiClientCore的声明式代码
4. 代码美化，输出到本地文件
