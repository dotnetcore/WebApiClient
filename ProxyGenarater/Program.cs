using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ProxyGenarater
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = args.FirstOrDefault();
            var proxyBuilder = new ProxyBuilder(fileName);
            proxyBuilder.BuildAndSave();
        }        
    }
}
