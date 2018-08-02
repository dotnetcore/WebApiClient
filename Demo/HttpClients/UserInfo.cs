using System;
using System.Text;
using System.Linq;
using WebApiClient.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Demo.HttpClients
{
    /// <summary>
    /// 表示用户模型
    /// </summary>
    public class UserInfo
    {
        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Account { get; set; }

        [Required]
        [AliasAs("password")]
        [StringLength(10, MinimumLength = 1)]
        public string Password { get; set; }

        [IgnoreWhenNull]
        [DateTimeFormat("yyyy-MM-dd")]
        public DateTime? BirthDay { get; set; }

        public Gender Gender { get; set; }

        [IgnoreSerialized]
        public string Email { get; set; }
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Gender
    {
        Female = 0,
        Male = 1
    }
}
