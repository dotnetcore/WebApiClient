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
        private readonly byte[] json;

        public MockResponseHandler()
        {
            var model = new Model { A = "A", B = 2, C = 3d };
            this.json = JsonSerializer.SerializeToUtf8Bytes(model);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayJsonContent(this.json) };
            return Task.FromResult(response);
        }
    }
}
