using NetworkSocket.Http;
using System.Text;
using WebApiClient;
using WebApiClient.Defaults;

namespace Demo.HttpServices
{
    class XmlResult : ActionResult
    {
        private readonly object data;

        private readonly Encoding encoding;

        public XmlResult(object data)
            : this(data, null)
        {
        }

        public XmlResult(object data, Encoding encoding)
        {
            this.data = data;
            this.encoding = encoding ?? Encoding.UTF8;
        }

        public override void ExecuteResult(RequestContext context)
        {
            var xml = XmlFormatter.Default.Serialize(this.data, this.encoding);
            context.Response.Charset = encoding;
            context.Response.ContentType = "application/xml";
            context.Response.WriteResponse(xml);
        }
    }
}
