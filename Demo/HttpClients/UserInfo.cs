using System;
using WebApiClient.DataAnnotations;

namespace Demo.HttpClients
{
    /// <summary>
    /// 表示用户模型
    /// </summary>
    public class UserInfo
    {
        public string Account { get; set; }

        [AliasAs("password")]
        public string Password { get; set; }

        [IgnoreWhenNull]
        [DateTimeFormat("yyyy-MM-dd")]
        public DateTime? BirthDay { get; set; }

        [IgnoreSerialized]
        public string Email { get; set; }

        public override string ToString()
        {
            return string.Format("{{Account:{0}, Password:{1}, BirthDay:{2}}}", this.Account, this.Password, this.BirthDay);
        }
    }
}
