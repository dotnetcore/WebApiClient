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
            var files = SearchFiles(fileName, ".dll", ".exe");

            foreach (var item in files)
            {
                var proxyBuilder = new ProxyBuilder(item);
                 proxyBuilder.BuildAndSave();
            }
        }

        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="fileName">文件</param>
        /// <param name="extension">可能的扩展名</param>
        /// <returns></returns>
        private static IEnumerable<string> SearchFiles(string fileName, params string[] extension)
        {
            if (string.IsNullOrEmpty(fileName) == true)
            {
                yield break;
            }

            if (File.Exists(fileName) == true)
            {
                yield return fileName;
            }

            foreach (var item in extension)
            {
                var file = Path.ChangeExtension(fileName, item);
                if (File.Exists(file) == true)
                {
                    yield return file;
                }
            }
        }
    }
}
