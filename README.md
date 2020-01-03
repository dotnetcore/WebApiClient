## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
本项目为一个实验室项目，计划只支持.netcore平台，并紧密与.netcore新特性紧密结合。
 
### 为什么有此项目
 
1. WebApiClient很优秀，它将不同框架不同平台都实现了统一的api
2. WebApiClient不够优秀，它在.netcore下完全可以更好，但它不得不兼容.net45开始所有框架而有所牺牲


### 此项目有何变化
1. 将System.Text.Json替换Json.net
2. 移除HttpApi、HttpApiFactoryHttApiConfig功能，使用DI、HttpClientFactory和Options
3. 移除AOT功能
4. 更多的HttpContent不再于string拼接，尽量使用Utf8Stream
5. 更规范的特性命名

### 个人想法
WebApiClient紧密集合Json.net，其与json.net的命运一样，在.netcore里不再非常重要。多年前，我造出了WebApiClient，现在，我想它应该有新的变化与使命了。
