namespace System.Net.Http
{
    /// <summary>
    /// 表示http进度
    /// </summary>
    public class HttpProgress
    {
        /// <summary>
        /// 获取文件总字节数
        /// </summary>
        public long? FileSize { get; }

        /// <summary>
        /// 获取当前接收到的字节数
        /// </summary>
        public long RecvSize { get; }

        /// <summary>
        /// 获取是否已完成
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// http进度
        /// </summary>
        /// <param name="fileSize">文件总字节数</param>
        /// <param name="recvSize">当前完成的字节数</param>
        /// <param name="isCompleted">是否已完成</param> 
        public HttpProgress(long? fileSize, long recvSize, bool isCompleted)
        {
            this.FileSize = fileSize;
            this.RecvSize = recvSize;
            this.IsCompleted = isCompleted;
        }
    }
}
