using System;
using System.Collections.Generic;
using System.Text;
using WebApiClient;
using Xunit;
using System.Linq;
using WebApiClient.Contexts;
using System.ComponentModel.DataAnnotations;

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
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter));

            parameter.Value = new User { };
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter));

            parameter.Value = new User { Account = "123" };
            ParameterValidator.Validate(parameter);

            parameter.Value = new User { Account = "123456" };
            Assert.Throws<ValidationException>(() => ParameterValidator.Validate(parameter));
        }
    }
}
