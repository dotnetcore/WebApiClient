using System.Net.Http;
using App.Attributes;
using App.Filters;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace App.Clients
{
    [LoggingFilter]
    public interface IDynamicHostDemo
    {
        [HttpGet]
        ITask<HttpResponseMessage> ByUrlString([Uri] string urlString);

        [UriFilter]
        [HttpGet]
        ITask<HttpResponseMessage> ByFilter();


        [HttpGet]
        [ServiceName("baiduService")]
        ITask<HttpResponseMessage> ByAttribute();
    }
}
