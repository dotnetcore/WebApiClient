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
        public void ValidateTest()
        {
            var parameter = TestParameter.Create(null);
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));

            parameter = TestParameter.Create(new User { });
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));

            parameter = TestParameter.Create(new User { Account = "123" });
            ParameterValidator.Validate(parameter, true);

            parameter = TestParameter.Create(new User { Account = "123456" });
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));
        }
    }
}
