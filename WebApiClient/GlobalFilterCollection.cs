using System;
using System.Collections;
using System.Collections.Generic;

namespace WebApiClient
{
    /// <summary>
    /// Represents a collection of global filters
    /// Global filters have the highest execution priority, and the execution order is the order of addition
    /// Non-thread-safe type
    /// </summary>
    public class GlobalFilterCollection : ICollection<IApiActionFilter>
    {
        /// <summary>
        /// List of saved data
        /// </summary>
        private readonly List<IApiActionFilter> filters;

        /// <summary>
        /// Collection of global filters
        /// </summary>
        public GlobalFilterCollection()
        {
            this.filters = new List<IApiActionFilter>();
        }

        /// <summary>
        /// Get the number of filters
        /// </summary>
        public int Count
        {
            get => this.filters.Count;
        }

        /// <summary>
        /// Adding global filters
        /// </summary>
        /// <param name="item">Global filter</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(IApiActionFilter item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            this.filters.Add(item);
        }

        /// <summary>
        /// Clear all global filters
        /// </summary>
        public void Clear()
        {
            this.filters.Clear();
        }

        /// <summary>
        /// Returns whether the specified global filter is included
        /// </summary>
        /// <param name="item">Global filter</param>
        /// <returns></returns>
        public bool Contains(IApiActionFilter item)
        {
            return item == null ? false : this.filters.Contains(item);
        }

        /// <summary>
        /// Delete the specified global filter
        /// </summary>
        /// <param name="item">Defined global filters</param>
        /// <returns></returns>
        public bool Remove(IApiActionFilter item)
        {
            return item == null ? false : this.filters.Remove(item);
        }

        /// <summary>
        /// Iterator that returns the filter
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IApiActionFilter> GetEnumerator()
        {
            return this.filters.GetEnumerator();
        }

        #region Explicit implementation
        /// <summary>
        /// Get whether it is read-only
        /// </summary>
        bool ICollection<IApiActionFilter>.IsReadOnly
        {
            get => false;
        }

        /// <summary>
        /// Copy to current array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<IApiActionFilter>.CopyTo(IApiActionFilter[] array, int arrayIndex)
        {
            this.filters.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get iterator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.filters.GetEnumerator();
        }
        #endregion
    }
}
