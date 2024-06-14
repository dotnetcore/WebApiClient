## App
这是应用例子，为了方便，服务端和客户端都在同一个程序进程内

### 服务端
Controllers 为服务端，TokensController 模拟 token 发放的服务端，UsersController 模拟用户资源服务器

### 客户端
* ApiClients.IUserApi为WebApiClientCore的声明式接口
* ApiClients.UserService为包装的服务，注入了IUserApi接口
* ApiClients.UserHostedService为后台服务，启动时获取UserService实例并运行
