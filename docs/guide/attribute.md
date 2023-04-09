
# 常用内置特性

内置特性指框架内提供的一些特性，拿来即用就能满足一般情况下的各种应用。当然，开发者也可以在实际应用中，编写满足特定场景需求的特性，然后将自定义特性修饰到接口、方法或参数即可。

> 执行前顺序

参数值验证 -> IApiActionAttribute -> IApiParameterAttribute -> IApiReturnAttribute -> IApiFilterAttribute

> 执行后顺序

IApiReturnAttribute -> 返回值验证 -> IApiFilterAttribute

## Return特性

特性名称 | 功能描述 | 备注
---|---|---|
RawReturnAttribute | 处理原始类型返回值 | 缺省也生效
JsonReturnAttribute | 处理Json模型返回值 | 缺省也生效
XmlReturnAttribute | 处理Xml模型返回值 | 缺省也生效
NoneReturnAttribute | 处理空返回值 | 缺省也生效

## 常用Action特性

特性名称 | 功能描述 | 备注
---|---|---|
HttpHostAttribute | 请求服务http绝对完整主机域名| 优先级比Options配置低
HttpGetAttribute | 声明Get请求方法与路径| 支持null、绝对或相对路径
HttpPostAttribute | 声明Post请求方法与路径| 支持null、绝对或相对路径
HttpPutAttribute | 声明Put请求方法与路径| 支持null、绝对或相对路径
HttpDeleteAttribute | 声明Delete请求方法与路径| 支持null、绝对或相对路径
*HeaderAttribute* | 声明请求头 | 常量值
*TimeoutAttribute* | 声明超时时间 | 常量值
*FormFieldAttribute* | 声明Form表单字段与值 | 常量键和值
*FormDataTextAttribute* | 声明FormData表单字段与值 | 常量键和值

## 常用Parameter特性

特性名称 | 功能描述 | 备注
---|---|---|
PathQueryAttribute | 参数值的键值对作为url路径参数或query参数的特性 | 缺省特性的参数默认为该特性
FormContentAttribute | 参数值的键值对作为x-www-form-urlencoded表单 |
FormDataContentAttribute | 参数值的键值对作为multipart/form-data表单 |
JsonContentAttribute | 参数值序列化为请求的json内容 |
XmlContentAttribute | 参数值序列化为请求的xml内容 |
UriAttribute | 参数值作为请求uri | 只能修饰第一个参数
ParameterAttribute | 聚合性的请求参数声明 | 不支持细颗粒配置
*HeaderAttribute* | 参数值作为请求头 |
*TimeoutAttribute* | 参数值作为超时时间 | 值不能大于HttpClient的Timeout属性
*FormFieldAttribute* | 参数值作为Form表单字段与值 | 只支持简单类型参数
*FormDataTextAttribute* | 参数值作为FormData表单字段与值 | 只支持简单类型参数

## Filter特性

特性名称 | 功能描述| 备注
---|---|---|
ApiFilterAttribute | Filter特性抽象类 |
LoggingFilterAttribute | 请求和响应内容的输出为日志的过滤器 |

## 自解释参数类型

类型名称 | 功能描述 | 备注
---|---|---|
FormDataFile | form-data的一个文件项 | 无需特性修饰，等效于FileInfo类型
JsonPatchDocument | 表示将JsonPatch请求文档 | 无需特性修饰
