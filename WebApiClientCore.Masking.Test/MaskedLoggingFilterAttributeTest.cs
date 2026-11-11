using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;
using WebApiClientCore.Implementations;
using WebApiClientCore.Masking.Strategies;
using Xunit;

namespace WebApiClientCore.Masking.Test
{
    public class MaskedLoggingFilterAttributeTest
    {
        public interface IMaskApi
        {
            [MaskField("phone", StrategyName = "Phone")]
            [MaskField("data.email", StrategyName = "Email")]
            Task<string> GetAsync();
        }

        public interface IReplaceApi
        {
            [MaskField("secret")]
            Task<string> GetAsync();
        }

        public interface IMissingStrategyApi
        {
            [MaskField("secret", "NotExists")]
            Task<string> GetAsync();
        }

        [Fact]
        public async Task 请求和响应内容都会被脱敏()
        {
            var context = CreateResponseContext(typeof(IMaskApi), new ServiceCollection().BuildServiceProvider());
            var logMessage = CreateLogMessage();

            await new ExposedFilter().InvokeAsync(context, logMessage);

            Assert.Contains("138****5678", logMessage.RequestContent);
            Assert.Contains("z***@example.com", logMessage.ResponseContent);
        }

        [Fact]
        public async Task 优先使用容器中注册的策略()
        {
            var services = new ServiceCollection();
            services.AddMasking(options => options.Strategies.Register(new ReplaceMaskStrategy("[已脱敏]")));

            var context = CreateResponseContext(typeof(IReplaceApi), services.BuildServiceProvider());
            var logMessage = CreateLogMessage();

            await new ExposedFilter().InvokeAsync(context, logMessage);

            Assert.Contains("[已脱敏]", logMessage.RequestContent);
            Assert.Contains("[已脱敏]", logMessage.ResponseContent);
        }

        [Fact]
        public async Task 策略不存在时抛出异常()
        {
            var context = CreateResponseContext(typeof(IMissingStrategyApi), new ServiceCollection().BuildServiceProvider());
            var logMessage = CreateLogMessage();

            await Assert.ThrowsAsync<MaskStrategyNotFoundException>(() =>
                new ExposedFilter().InvokeAsync(context, logMessage));
        }

        private static LogMessage CreateLogMessage()
        {
            return new LogMessage
            {
                HasRequest = true,
                HasResponse = true,
                RequestTime = DateTime.Now,
                RequestContent = """{"phone":"13812345678","secret":"abc"}""",
                ResponseTime = DateTime.Now,
                ResponseContent = """{"data":{"email":"zhangsan@example.com"},"secret":"abc"}"""
            };
        }

        private static ApiResponseContext CreateResponseContext(Type apiType, IServiceProvider serviceProvider)
        {
            var method = apiType.GetMethod("GetAsync")!;
            var options = new HttpApiOptions { HttpHost = new Uri("http://www.webapi.com/") };
            var httpClientContext = new HttpClientContext(new HttpClient(), serviceProvider, options, string.Empty);
            var httpContext = new HttpContext(httpClientContext, new FakeHttpApiRequestMessage());
            var descriptor = new DefaultApiActionDescriptor(method, apiType);
            var requestContext = new ApiRequestContext(httpContext, descriptor, Array.Empty<object?>(), new DefaultDataCollection());
            return new ApiResponseContext(requestContext);
        }

        private sealed class ExposedFilter : MaskedLoggingFilterAttribute
        {
            public Task InvokeAsync(ApiResponseContext context, LogMessage logMessage)
            {
                return this.WriteLogAsync(context, logMessage);
            }
        }

        private sealed class FakeHttpApiRequestMessage : HttpApiRequestMessage
        {
            public override Uri MakeRequestUri(Uri uri) => uri;

            public override void AddUrlQuery(string key, string? value)
            {
            }

            public override Task AddFormFieldAsync(IEnumerable<KeyValue> keyValues) => Task.CompletedTask;

            public override void AddFormDataText(IEnumerable<KeyValue> keyValues)
            {
            }

            public override void AddFormDataFile(Stream stream, string name, string? fileName, string? contentType)
            {
            }
        }
    }
}
