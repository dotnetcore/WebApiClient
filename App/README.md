## App
这是应用例子，为了方便，服务端和客户端都在同一个程序进程内

### 服务端
Controllers为服务端，TokensController模拟token发放的服务端，UsersController模拟用户资源服务器

### 客户端
* Clients.IUserApi为WebApiClientCore的声明式接口
* Clients.IUserApi.ParameterStyle为Parameter式声明，两种效果相同
* Clients.UserService为包装的服务，注入了IUserApi接口
* Clients.UserHostedService为后台服务，启动时获取UserService实例并运行
