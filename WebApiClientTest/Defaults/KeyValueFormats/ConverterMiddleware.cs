using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    static class ConverterMiddleware
    {
        public static IConverter Link(params IConverter[] converters)
        {
            var first = converters.First();
            converters.Aggregate((cur, next) =>
            {
                cur.Next = next;
                cur.First = first;
                return next;
            }).First = first;
            return first;
        }
    }
}
