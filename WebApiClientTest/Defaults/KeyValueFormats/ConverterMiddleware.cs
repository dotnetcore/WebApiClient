using System;
using Xunit;
using WebApiClient;
using WebApiClient.Defaults;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using System.Linq;

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
