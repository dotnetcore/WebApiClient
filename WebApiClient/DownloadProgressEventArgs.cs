using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示下载进度
    /// </summary>
    public class DownloadProgressEventArgs : EventArgs
    {
        /// <summary>
        /// 获取当前收到的字节数
        /// </summary>
        public long CurrentBytes { get; }

        /// <summary>
        /// 获取总字节数
        /// </summary>
        public long? TotalBytes { get; }

        /// <summary>
        /// 获取是否已完成
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// 获取当前进度
        /// </summary>
        public double Progress
        {
            get
            {
                if (this.TotalBytes == null)
                {
                    return 0.00d;
                }
                return (double)this.CurrentBytes / (double)this.TotalBytes.Value;
            }
        }

        /// <summary>
        /// 下载进度
        /// </summary>
        /// <param name="current">当前收到的字节数</param>
        /// <param name="total">总字节数</param>
        /// <param name="isCompleted">是否已完成</param>
        public DownloadProgressEventArgs(long current, long? total, bool isCompleted)
        {
            this.CurrentBytes = current;
            this.TotalBytes = total;
            this.IsCompleted = isCompleted;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Math.Round(this.Progress * 100, 2)}%";
        }
    }
}
