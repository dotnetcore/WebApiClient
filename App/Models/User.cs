using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApiClientCore.Serialization.JsonConverters;

namespace App.Models
{
    /// <summary>
    /// 表示用户模型
    /// </summary>
    public class User
    {
        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Account { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Password { get; set; } = string.Empty;

        public string? NickName { get; set; }

        [JsonDateTime("yyyy年MM月dd日")]
        public DateTime? BirthDay { get; set; }

        public Gender Gender { get; set; }

        [JsonIgnore]
        public string? Email { get; set; }
    }
}
