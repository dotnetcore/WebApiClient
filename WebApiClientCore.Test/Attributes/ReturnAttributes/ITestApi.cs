using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.Test.Attributes.ReturnAttributes
{
    public interface ITestApi : IDisposable
    {
        Task<HttpResponseMessage> HttpResponseMessageAsync();

        Task<string> StringAsync();

        Task<byte[]> ByteArrayAsync();

        Task<TestModel> JsonXmlAsync();
    }
}
