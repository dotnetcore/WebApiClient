using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WebApiClient
{
    /// <summary>
    /// Represents a storage and access container for custom data
    /// Thread-safe type
    /// </summary>
    [DebuggerDisplay("Count = {lazy.Value.Count}")]
    [DebuggerTypeProxy(typeof(DebugView))]
    public sealed class Tags
    {
        /// <summary>
        /// Sync lock
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// Data Dictionary
        /// </summary>
        private readonly Lazy<Dictionary<string, object>> lazy = new Lazy<Dictionary<string, object>>(() => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase), true);


        /// <summary>
        /// Gets or sets a unique identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TagItem this[string key]
        {
            get => this.Get(key);
        }

        /// <summary>
        /// Get value by key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TagItem Get(string key)
        {
            lock (this.syncRoot)
            {
                if (this.lazy.Value.TryGetValue(key, out object value) == true)
                {
                    return new TagItem(value);
                }
                return TagItem.NoValue;
            }
        }

        /// <summary>
        /// Delete the specified key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (this.lazy.IsValueCreated == false)
            {
                return false;
            }

            lock (this.syncRoot)
            {
                return this.lazy.Value.Remove(key);
            }
        }

        /// <summary>
        /// Remove by key
        /// Delete the corresponding key after removing
        /// Same as Get and then Remove
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public TagItem Take(string key)
        {
            lock (this.syncRoot)
            {
                if (this.lazy.Value.TryGetValue(key, out object value) == true)
                {
                    this.lazy.Value.Remove(key);
                    return new TagItem(value);
                }
                return TagItem.NoValue;
            }
        }

        /// <summary>
        /// Set value by key
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void Set(string key, object value)
        {
            lock (this.syncRoot)
            {
                this.lazy.Value[key] = value;
            }
        }

        /// <summary>
        /// Debug view
        /// </summary>
        private class DebugView
        {
            /// <summary>
            /// Values
            /// </summary>
            private readonly IEnumerable<KeyValuePair<string, object>> value;

            /// <summary>
            /// Debug view
            /// </summary>
            /// <param name="target">Viewed</param>
            public DebugView(Tags target)
            {
                this.value = target.lazy.IsValueCreated ? target.lazy.Value : null;
            }

            /// <summary>
            /// What to view
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<string, object>[] Values
            {
                get => this.value == null ? (new KeyValuePair<string, object>[0]) : this.value.ToArray();
            }
        }
    }
}
