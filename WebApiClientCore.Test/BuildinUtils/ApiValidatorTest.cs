using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebApiClientCore.Test.BuildinUtils
{
    public class ApiValidatorTest
    {
        class User
        {
            [Required]
            [StringLength(4)]
            public string Account { get; set; }
        }

        [Fact]
        public void ValidateParameterTest()
        {
            var parameter = TestParameter.Create();
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter,null, true));

            parameter = TestParameter.Create();
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter, new User { }, true));

            parameter = TestParameter.Create();
            ApiValidator.ValidateParameter(parameter, new User { Account = "123" }, true);

            parameter = TestParameter.Create();
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter, new User { Account = "123456" }, true));
        }

        [Fact]
        public void ValidateReturnValueTest()
        {
            var value = default(User);
            ApiValidator.ValidateReturnValue(value);

            value = new User();
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateReturnValue(value));

            value = new User { Account = "123" };
            ApiValidator.ValidateReturnValue(value);

            value = new User { Account = "123456" };
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateReturnValue(value));
        }

        class TestParameter
        {
            public void Test([RequiredAttribute]object p)
            {
            }

            public static ApiParameterDescriptor Create()
            {
                var p = typeof(TestParameter).GetMethod("Test").GetParameters()[0];
                return new ApiParameterDescriptor(p);
            }
        }
    }
}
