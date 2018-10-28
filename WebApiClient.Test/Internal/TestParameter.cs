using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Test.Internal
{
    class TestParameter
    {
        public void Test([RequiredAttribute]object p)
        {
        }

        public static ApiParameterDescriptor Create(object value)
        {
            var p = typeof(TestParameter).GetMethod("Test").GetParameters()[0];
            return ApiParameterDescriptor.Create(p).Clone(value);
        }
    }
}
