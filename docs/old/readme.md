# 概览

## WebApiClient.JIT

在运行时使用Emit创建Http请求接口的代理类，HttpApiClient.Create()时，在新的程序集创建了TInterface的代理类，类名与TInterface相同，命名空间也相同，由于代理类和TInterface接口不在同一程序集，所以要求TInterface为public。

+ 可以在项目中直接引用WebApiClient.JIT.dll就能使用；
+ 不适用于不支持JIT技术的平台(IOS、UWP)；
+ 接口要求为public；

## WebApiClient.AOT

在编译过程中使用Mono.Cecil修改编译得到的程序集，向其插入Http请求接口的代理类IL指令，这一步是在AOT编译阶段之前完成。代理类型所在的程序集、模块、命名空间与接口类型的一样，其名称为$前缀的接口类型名称，使用反编译工具查看项目编译后的程序集可以看到这些代理类。

+ 项目必须使用nuget安装WebApiClient.AOT才能正常使用；
+ 没有JIT，支持的平台广泛；
+ 接口不要求为public，可以嵌套在类里面；
