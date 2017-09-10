using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static async void Test()
        {
            try
            {
                var myWebApi = new WebApiClient.HttpApiClient().Implement<MyWebApi>();
                await myWebApi.TestAsync("myAction", new[] { 1, 2 }, null);
                await myWebApi.GetAboutAsync("typeValue");
                await myWebApi.UpdateUserAsync(new UserInfo { UserName = "abc", Password = "123456" });
                await myWebApi.DeleteUser2Async(id: "id001");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }
    }
}
