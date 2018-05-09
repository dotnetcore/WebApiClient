using System;

namespace DemoLib
{
    class Program
    {
        static void Main(string[] args)
        {
            TestAsync();
            Console.Read();
        }

        static async void TestAsync()
        {
            var client = WebApiClient.HttpApiClient.Create<IBaiduApi>();
            Console.WriteLine(client.GetType());
            var str = await client.GetAsync("https://www.baidu.com/");
        }
    }
}
