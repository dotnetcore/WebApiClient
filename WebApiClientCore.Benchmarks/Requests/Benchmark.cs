using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    [InProcess]
    [MemoryDiagnoser]
    public abstract class Benchmark
    {
        protected IServiceProvider ServiceProvider { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();
            services
                .AddWebApiClient()
                .ConfigureHttpApi(o =>
                {
                    o.UseLogging = false;
                    o.UseParameterPropertyValidate = false;
                    o.UseReturnValuePropertyValidate = false;
                });

            services
                .AddHttpApi<IWebApiClientCoreJsonApi>()
                .AddHttpMessageHandler(() => new JsonResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddHttpApi<IWebApiClientCoreXmlApi>()
                .AddHttpMessageHandler(() => new XmlResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddRefitClient<IRefitJsonApi>(new RefitSettings
                {
                    ContentSerializer = new SystemTextJsonContentSerializer()
                })
                .AddHttpMessageHandler(() => new JsonResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddRefitClient<IRefitXmlApi>(new RefitSettings
                {
                    ContentSerializer = new XmlContentSerializer()
                })
                .AddHttpMessageHandler(() => new XmlResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            // 配置 RestSharp JSON 客户端
            services.AddHttpClient("RestSharpJsonClient")
                .AddHttpMessageHandler(() => new JsonResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            // 配置 RestSharp XML 客户端
            services.AddHttpClient("RestSharpXmlClient")
                .AddHttpMessageHandler(() => new XmlResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            // 注册 RestSharp JSON 客户端
            services.AddTransient<RestSharpJsonClient>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("RestSharpJsonClient");
                return new RestSharpJsonClient(httpClient);
            });

            // 注册 RestSharp XML 客户端
            services.AddTransient<RestSharpXmlClient>(sp =>
            {
                var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("RestSharpXmlClient");
                return new RestSharpXmlClient(httpClient);
            });

            this.ServiceProvider = services.BuildServiceProvider();

            // 服务显式加载预热
            this.ServiceProvider.GetService<IWebApiClientCoreJsonApi>();
            this.ServiceProvider.GetService<IWebApiClientCoreXmlApi>();
            this.ServiceProvider.GetService<IRefitJsonApi>();
            this.ServiceProvider.GetService<IRefitXmlApi>();
            this.ServiceProvider.GetService<RestSharpJsonClient>();
            this.ServiceProvider.GetService<RestSharpXmlClient>();
        }

        private class JsonResponseHandler : DelegatingHandler
        {
            private static readonly MediaTypeHeaderValue applicationJson = MediaTypeHeaderValue.Parse("application/json");
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var content = new StreamContent(new MemoryStream(User.Utf8Json, writable: false), User.Utf8Json.Length);
                content.Headers.ContentType = applicationJson;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };
                return Task.FromResult(response);
            }
        }

        private class XmlResponseHandler : DelegatingHandler
        {
            private static readonly MediaTypeHeaderValue applicationXml = MediaTypeHeaderValue.Parse("application/xml");
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var content = new StringContent(User.XmlString, Encoding.UTF8);
                content.Headers.ContentType = applicationXml;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };
                return Task.FromResult(response);
            }
        }

        // RestSharp JSON客户端包装类
        public class RestSharpJsonClient : RestClient
        {
            public RestSharpJsonClient(HttpClient httpClient) : base(httpClient)
            {
            }
        }

        // RestSharp XML客户端包装类
        public class RestSharpXmlClient : RestClient
        {
            public RestSharpXmlClient(HttpClient httpClient) : base(httpClient)
            {
            }
        }
    }
}
