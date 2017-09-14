using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace Demo
{
    class Program
    {
        static async Task TestAsync()
        {
            var webApiClient = new HttpApiClient();
            var myWebApi = webApiClient.Implement<MyWebApi>();
            var user = new UserInfo { Account = "laojiu", Password = "123456" };
            var file = new MulitpartFile("head.jpg");

            var user1 = await myWebApi.GetUserByIdAsync("id001");
            var user2 = await myWebApi.GetUserByAccountAsync("laojiu");

            await myWebApi.UpdateUserWithFormAsync(user);
            await myWebApi.UpdateUserWithJsonAsync(user);
            await myWebApi.UpdateUserWithXmlAsync(user);
            await myWebApi.UpdateUserWithMulitpartAsync(user, file);
        }

        static void Main(string[] args)
        {
            try
            {
                TestAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
