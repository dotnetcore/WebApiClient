using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOTTask
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = args.FirstOrDefault();
            var path = Path.GetDirectoryName(target);
            var task = new WebApiClient.AOT.Task.Assembly(target, new[] { path, @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" });
            task.WirteProxyTypes();
            task.Save();
        }
    }
}
