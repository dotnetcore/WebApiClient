using System;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Parameters;

namespace App.Clients
{
    public class UserService
    {
        private readonly IUserApi userApi;

        public UserService(IUserApi userApi)
        {
            this.userApi = userApi;
        }

        public async Task RunRequestAsync()
        {
            var user = new User
            {
                Account = "lao九",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com",
                Gender = Gender.Male
            };

            // 上传的文件
            var file = new FormDataFile("TextFile.txt");

            var response = await userApi.GetAsync(account: "get1");

            var @string = await userApi.GetAsStringAsync(account: "get2");
            var jsonText = await userApi.GetExpectJsonAsync(account: "json");
            var xmlText = await this.userApi.GetExpectXmlAsync(account: "xml");

            var byteArray = await userApi.GetAsByteArrayAsync(account: "get3");
            var stream = await userApi.GetAsStreamAsync(account: "get4");
            var model = await userApi.GetAsModelAsync(account: "get5");

            var post1 = await userApi.PostByJsonAsync(user);
            var post2 = await userApi.PostByXmlAsync(user);
            var post3 = await userApi.PostByFormAsync(user);
            var post4 = await userApi.PostByFormDataAsync(user, file);

            var retry = await userApi.GetAsync(account: "retry")
                .Retry(maxCount: 3)
                .WhenCatch<Exception>();

            await userApi.DeleteAsync(account: "account");
        }
    }
}
