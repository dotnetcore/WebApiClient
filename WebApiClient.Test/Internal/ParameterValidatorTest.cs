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
            var parameter = new ApiParameterDescriptor
            {
                ValidationAttributes = new[] { new RequiredAttribute() },
                Name = "user"
            };
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));

            parameter.Value = new User { };
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));

            parameter.Value = new User { Account = "123" };
            ParameterValidator.Validate(parameter, true);

            parameter.Value = new User { Account = "123456" };
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter, true));
        }
    }
}
