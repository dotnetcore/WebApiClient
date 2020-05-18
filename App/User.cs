using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace App
{
    /// <summary>
    /// 表示用户模型
    /// </summary>
    public class User
    {
        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Account { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Password { get; set; }

        public string NickName { get; set; }

        public DateTime? BirthDay { get; set; }

        public Gender Gender { get; set; }

        [JsonIgnore]
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
