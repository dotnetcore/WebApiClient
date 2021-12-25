using System;

namespace App.Services
{
    public interface IDBProvider
    {
        string SelectServiceUri(string serviceName);
    }
    public class DBProvider : IDBProvider
    {
        public string SelectServiceUri(string serviceName)
        {
            if (serviceName == "baiduService") return "https://www.baidu.com";
            if (serviceName == "microsoftService") return "https://www.microsoft.com";
            return string.Empty;
        }
    }

    public class HostProvider
    {
        private readonly IDBProvider dbProvider;

        public HostProvider(IDBProvider dbProvider)
        {
            this.dbProvider = dbProvider;
            //将HostProvider放到依赖注入容器中，即可从容器获取其它服务来实现动态的服务地址查询
        }

        internal string ResolveService(string name)
        {
            //如何获取动态的服务地址由你自己决定，此处仅以简单的接口定义简要说明
            return dbProvider.SelectServiceUri(name);
        }
    }
}
