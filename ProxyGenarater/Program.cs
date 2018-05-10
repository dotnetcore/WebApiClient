using System;
using System.Linq;

namespace ProxyGenarater
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

               // new ProxyBuilder(fileName).BuildAndSave();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.ExitCode = -1;
            }
        }
    }
}
