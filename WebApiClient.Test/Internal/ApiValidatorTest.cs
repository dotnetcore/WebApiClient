using System.ComponentModel.DataAnnotations;
using WebApiClient.Contexts;
using Xunit;

namespace WebApiClient.Test.Internal
{
    public class ParameterValidatorTest
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
            var parameter = TestParameter.Create(null);
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter, true));

            parameter = TestParameter.Create(new User { });
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter, true));

            parameter = TestParameter.Create(new User { Account = "123" });
            ApiValidator.ValidateParameter(parameter, true);

            parameter = TestParameter.Create(new User { Account = "123456" });
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateParameter(parameter, true));
        }

        [Fact]
        public void ValidateReturnValueTest()
        {
            var value = default(User);
            ApiValidator.ValidateReturnValue(value, true);

            value = new User();
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateReturnValue(value, true));

            value = new User { Account = "123" };
            ApiValidator.ValidateReturnValue(value, true);

            value = new User { Account = "123456" };
            Assert.Throws<ValidationException>(() => ApiValidator.ValidateReturnValue(value, true));
        }
    }
}
