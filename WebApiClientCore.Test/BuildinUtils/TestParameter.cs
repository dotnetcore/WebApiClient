using System.ComponentModel.DataAnnotations;

namespace WebApiClientCore.Test.BuildinUtils
{
    class TestParameter
    {
        public void Test([RequiredAttribute]object p)
        {
        }

        public static ApiParameterDescriptor Create(   )
        {
            var p = typeof(TestParameter).GetMethod("Test").GetParameters()[0];
            return new ApiParameterDescriptor(p);
        }
    }
}
