using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;

namespace WebApiClientTest.Internal
{
    public class PropertySetterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var model = new Model { name = "laojiu" };
            var setter = new PropertySetter(model.GetType().GetProperty("name"));
            setter.Invoke(model, "ee");
            Assert.True("ee" == model.name);
        }

        class Model
        {
            public string name { get; set; }
        }
    }
}
