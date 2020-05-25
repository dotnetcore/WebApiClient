using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    /// <summary>
    /// 无真实http请求的Handler
    /// </summary>
    class MockResponseHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage benchmarkModelResponseMessage;

        public MockResponseHandler()
        {
            var model = new Model { A = "A", B = 2, C = 3d };
            var json = JsonSerializer.SerializeToUtf8Bytes(model);
            this.benchmarkModelResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new JsonContent(json) };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(this.benchmarkModelResponseMessage);
        }
    }
}
