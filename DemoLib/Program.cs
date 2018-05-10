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
            var client = WebApiClient.HttpApiClient.Create<IGithugApi>();          
            Console.WriteLine(client.GetType());
            var str = await client.GetApiListAsync("user");
        }
    }
}
