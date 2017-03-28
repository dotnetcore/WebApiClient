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
            var myWebApi = new WebApiClient.HttpApiClient().GetHttpApi<MyWebApi>();

            await myWebApi.GetAboutAsync("typeValue");
            await myWebApi.UpdateUserAsync(new UserInfo { UserName = "abc", Password = "123456" });
            await myWebApi.DeleteUser2Async(id: "id001");
        }

        static void Main(string[] args)
        {
            Test();
            Console.ReadLine();
        }
    }
}
