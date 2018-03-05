using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using Xunit;

namespace WebApiClientTest.Defaults.KeyValueFormats
{
    public class DynamicObjectConverterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var first = ConverterMiddleware.Link( new DynamicObjectConverter());
             dynamic  model = new dyObject();
            model.name = "laojiu";
            model.age = "18";
            var context = new ConvertContext("name", model, 0, null);
            var kvs = first.Invoke(context)
                .ToDictionary(item => item.Key, item => item.Value);

            Assert.True(kvs["name"] == "laojiu");
            Assert.True(kvs["age"] == "18");
        }

        class dyObject : DynamicObject
        {
            private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return this.dictionary.Keys;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                this.dictionary[binder.Name] = value;
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                return this.dictionary.TryGetValue(binder.Name, out result);
            }
        }
    }
}
