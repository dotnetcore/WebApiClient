# Q&A

## 1 声明的http接口为什么要继承IHttpApi接口？

一是为了方便WebApiClient库自动生成接口的代理类，相当用于标记作用；二是继承了`IHttpApi`接口，http接口代理类实例就有Dispose方法。

## 2 http接口可以继承其它http接口吗？

可以继承，父接口的相关方法也都当作Api方法，需要注意的是，父接口的方法的接口级特性将失效，而是应用了子接口的接口级特性，所以为了方便理解，最好不要这样继承。

## 3 使用`[ProxyAttribute(host,port)]`代理特性前，怎么验证代理的有效性？

可以使用ProxyValidator对象的Validate方法来验证代理的有效性。

## 4 为什么不支持将接口方法的返回类型声明为`Task`对象而必须为`Task<>`或`ITask<>`？

这个是设计的原则，因为不管开发者关不关注返回值，Http请求要么有响应要么抛出异常，如果你不关注结果的解析，可以声明为`Task<HttpResponseMessage>`而不去解析`HttpResponseMessage`就可以。

## 5 使用WebApiClient怎么下载文件？

你应该将接口返回类型声明为`ITask<HttpResponseFile>`。

## 6 接口返回类型除了声明为`ITask<HttpResponseMessage>`，还可以声明哪些抽象的返回类型？

还可以声明为`ITask<string>`、`ITask<Stream>`和`ITask<Byte[]>`，这些都是抽象的返回类型。

## 7 接口声明的参数可以为Object或某些类型的基类吗？

可以这样声明，数据还是子类的，但xml序列化会有问题，一般情况下，建议严格按照服务器的具体类型来声明参数。

## 8 WebApiClient怎么使用同步请求

WebApiClient是对HttpClient的封包，HttpClient没有提供相关的同步请求方法，所以WebApiClient也没有同步请求，不正确的阻塞ITask和Task返回值，在一些环境下很容易死锁。
