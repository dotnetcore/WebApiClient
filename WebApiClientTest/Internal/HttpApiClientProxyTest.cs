#if JIT
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient;
using Xunit;

namespace WebApiClientTest.Internal
{
    public class HttpApiClientProxyTest
    {
        class MyInterceptor : IApiInterceptor,IDisposable
        {
            public HttpApiConfig ApiConfig { get; private set; }

            public MyInterceptor(HttpApiConfig apiConfig)
            {
                this.ApiConfig = apiConfig;
            }

            public object Intercept(object target, MethodInfo method, object[] parameters)
            {
                var result = new InterceptResult
                {
                    Target = target,
                    Method = method,
                    Parameters = parameters
                };
                return Task.FromResult(result);
            }

            public void Dispose()
            {
                this.ApiConfig.Dispose();
            }
        }

        public class InterceptResult
        {
            public object Target { get; set; }
            public MethodInfo Method { get; set; }
            public object[] Parameters { get; set; }
        }

        public interface IMyApi
        {
            Task<InterceptResult> M1(int x, int y);
        }


        [Fact]
        public async Task CreatePorxyTest()
        {
            var config = new HttpApiConfig();
            var interceptor = new MyInterceptor(config);
            var myApi = HttpApiClientProxy.CreateInstance(typeof(IMyApi), interceptor) as IMyApi;

            var result = await myApi.M1(0, 1);
            Assert.Equal(result.Method, typeof(IMyApi).GetMethod("M1"));
            Assert.True(result.Parameters.Length == 2);
            Assert.True((int)result.Parameters.First() == 0);
            Assert.True((int)result.Parameters.Last() == 1);
        }

    }
}
#endif