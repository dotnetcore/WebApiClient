using CommandLine;
using System;

namespace WebApiClientCore.Extensions.OpenApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new OpenApiDocOptions();
            if (Parser.Default.ParseArguments(args, options))
            {
                var doc = new OpenApiDoc(options);
                doc.GenerateFiles();
            }
            else
            {
                Console.WriteLine(options.GetUsage());
                Console.Read();
            }
        }
    }
}
