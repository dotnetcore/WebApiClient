using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.DataAnnotations;

namespace Demo
{
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
