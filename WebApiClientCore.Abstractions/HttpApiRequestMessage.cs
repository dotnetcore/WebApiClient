using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示httpApi的请求消息
    /// </summary>
    public abstract class HttpApiRequestMessage : HttpRequestMessage
    {
        /// <summary>
        /// 返回使用uri值合成的请求URL
        /// </summary> 
        /// <param name="uri">uri值</param> 
        /// <returns></returns>
        public abstract Uri MakeRequestUri(Uri uri);

        /// <summary>
        /// 追加Query参数到请求路径
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param> 
        public abstract void AddUrlQuery(string key, string? value);


        /// <summary>
        /// 添加字段到已有的Content
        /// 要求content-type为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param> 
        /// <returns></returns>
        public async Task AddFormFieldAsync(string name, string? value)
        {
            var keyValue = new KeyValue(name, value);
            await this.AddFormFieldAsync(new[] { keyValue }).ConfigureAwait(false);
        }

        /// <summary>
        /// 添加字段到已有的Content
        /// 要求content-type为application/x-www-form-urlencoded
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public abstract Task AddFormFieldAsync(IEnumerable<KeyValue> keyValues);


        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="name">名称</param>
        /// <param name="value">文本</param> 
        public void AddFormDataText(string name, string? value)
        {
            var keyValue = new KeyValue(name, value);
            this.AddFormDataText(new[] { keyValue });
        }

        /// <summary>
        /// 添加文本内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>     
        /// <param name="keyValues">键值对</param> 
        public abstract void AddFormDataText(IEnumerable<KeyValue> keyValues);

        /// <summary>
        /// 添加文件内容到已有的Content
        /// 要求content-type为multipart/form-data
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <param name="name">名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">文件Mime</param> 
        public abstract void AddFormDataFile(Stream stream, string name, string? fileName, string? contentType);
    }
}
