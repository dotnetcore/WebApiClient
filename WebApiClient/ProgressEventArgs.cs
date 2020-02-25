using System;

namespace WebApiClient
{
    /// <summary>
    /// Indicates the upload or download progress
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Get the number of bytes currently completed
        /// </summary>
        public long CurrentBytes { get; }

        /// <summary>
        /// Get the total number of bytes
        /// </summary>
        public long? TotalBytes { get; }

        /// <summary>
        /// Whether the acquisition is completed
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// Get current progress
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
        /// Upload or download progress
        /// </summary>
        /// <param name="current">Number of bytes currently completed</param>
        /// <param name="total">Total number of bytes</param>
        /// <param name="isCompleted">Whether completed</param>
        public ProgressEventArgs(long current, long? total, bool isCompleted)
        {
            this.CurrentBytes = current;
            this.TotalBytes = total;
            this.IsCompleted = isCompleted;
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Math.Round(this.Progress * 100, 2)}%";
        }
    }
}
