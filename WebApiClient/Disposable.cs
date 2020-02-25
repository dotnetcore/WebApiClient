using System;

namespace WebApiClient
{
    /// <summary>
    /// Abstract base class representing Dispose
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        /// <summary>
        /// Gets whether the object is released
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Close and release all related resources
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed == false)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            this.IsDisposed = true;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Disposable()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">Whether to release managed resources</param>
        protected abstract void Dispose(bool disposing);
    }
}
