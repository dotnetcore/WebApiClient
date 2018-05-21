using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class PropertyGetterTest
    {
        [Fact]
        public void InvokeTest()
        {
            var model = new { name = "laojiu" };
            var getter = new PropertyGetter(model.GetType().GetProperty("name"));
            var name = getter.Invoke(model)?.ToString();

            Assert.True(name == model.name);

            var p = model.GetType().GetProperty("name");
            getter = new PropertyGetter(p.DeclaringType, p.Name);
            name = getter.Invoke(model)?.ToString();
            Assert.True(name == model.name);
        }
    }
}
