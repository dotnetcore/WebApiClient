using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示滞后写入数据的HttpContent抽象类
    /// </summary>
    public abstract class HystereticContent :HttpContent
    {
        /// <summary>
        /// 获取当前是否支持写入数据
        /// </summary>
        public bool CanWrite { get; private set; } = true;

        /// <summary>
        /// 检测写入操作
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void CheckForWrite()
        {
            if (this.CanWrite == false)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// 序列化到目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected sealed override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            this.CanWrite = false;
            return this.SerializeToStreamAsync(stream);
        }

        /// <summary>
        /// 序列化到目标流中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected abstract Task SerializeToStreamAsync(Stream stream);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.CanWrite = false;
            base.Dispose(disposing);
        }
    }
}
