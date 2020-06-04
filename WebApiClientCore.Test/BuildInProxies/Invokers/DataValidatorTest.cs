using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebApiClientCore.Test.BuildInProxies.Invokers
{
    public class DataValidatorTest
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
            Assert.Throws<ValidationException>(() => DataValidator.ValidateParameter(parameter,null, true));

            parameter = TestParameter.Create();
            Assert.Throws<ValidationException>(() => DataValidator.ValidateParameter(parameter, new User { }, true));

            parameter = TestParameter.Create();
            DataValidator.ValidateParameter(parameter, new User { Account = "123" }, true);

            parameter = TestParameter.Create();
            Assert.Throws<ValidationException>(() => DataValidator.ValidateParameter(parameter, new User { Account = "123456" }, true));
        }

        [Fact]
        public void ValidateReturnValueTest()
        {
            var value = default(User);
            DataValidator.ValidateReturnValue(value);

            value = new User();
            Assert.Throws<ValidationException>(() => DataValidator.ValidateReturnValue(value));

            value = new User { Account = "123" };
            DataValidator.ValidateReturnValue(value);

            value = new User { Account = "123456" };
            Assert.Throws<ValidationException>(() => DataValidator.ValidateReturnValue(value));
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
