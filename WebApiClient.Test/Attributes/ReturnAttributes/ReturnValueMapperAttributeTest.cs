using System;
using WebApiClient.Attributes.ReturnAttributes;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Attributes.ReturnAttributes
{
    public class ReturnValueMapperAttributeTest
    {
        [Fact]
        public void ArgumentCheck()
        {
            Assert.Throws<ArgumentNullException>(() => new ReturnValueMapperAttribute(null, null));
            Assert.Throws<ArgumentException>(() => new ReturnValueMapperAttribute(null, typeof(string)));
            Assert.Throws<ArgumentException>(() => new ReturnValueMapperAttribute(null, typeof(TestClass1)));
            Assert.Throws<ArgumentException>(() => new ReturnValueMapperAttribute(null, typeof(TestClass2)));
            Assert.Throws<ArgumentNullException>(() => new ReturnValueMapperAttribute(null, typeof(TestClass3)));
        }

        private abstract class TestClass1 : IReturnValueMapper
        {
            public abstract object Map(object returnValue, ApiActionContext context);
        }

        private class TestClass2 : TestClass3
        {
            public TestClass2(object _) { }
        }

        private class TestClass3 : IReturnValueMapper
        {
            public object Map(object returnValue, ApiActionContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
