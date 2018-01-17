using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebApiClient
{
    /// <summary>
    /// 提供文件扩展名与文件类型/子类型映射
    /// </summary>
    static class MimeTable
    {
        /// <summary>
        /// 默认文件类型
        /// </summary>
        private const string defaultContentType = "application/octet-stream";

        /// <summary>
        /// mime数据表
        /// </summary>
        private static readonly Dictionary<string, string> mimeTable = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 文件扩展名与文件类型/子类型映射
        /// </summary>
        static MimeTable()
        {
            var datas = from line in MimeTable.LoadMimeLines()
                        let kv = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        let ext = kv.FirstOrDefault()
                        let contenType = kv.LastOrDefault()
                        select new { ext, contenType };

            foreach (var item in datas)
            {
                if (mimeTable.ContainsKey(item.ext) == false)
                {
                    mimeTable.Add(item.ext, item.contenType);
                }
            }
        }

        /// <summary>
        /// 加载mime资源所有行
        /// </summary>
        /// <returns></returns>
        private static string[] LoadMimeLines()
        {
            var name = typeof(MimeTable).Namespace + ".Internal.mime.day";
            using (var stream = typeof(MimeTable).Assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    return new string[0];
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        /// <summary>
        /// 获取ContentType
        /// </summary>
        /// <param name="fileNameOrExtension">文件扩展名</param>
        /// <returns></returns>
        public static string GetContentType(string fileNameOrExtension)
        {
            if (fileNameOrExtension == null)
            {
                return defaultContentType;
            }

            var ext = Path.GetExtension(fileNameOrExtension);
            if (mimeTable.ContainsKey(ext))
            {
                return mimeTable[ext];
            }
            return defaultContentType;
        }
    }
}
