using System;
using System.Linq;

namespace WebApiClient.AOT.Task
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var fileName = args.FirstOrDefault();
                using (var assembly = new Assembly(fileName))
                {
                    assembly.WirteProxyTypes();
                    assembly.Save();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);               
            }
        }
    }
}
