using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solid.Extensions.System.Web
{
    /// <summary>
    /// The Async call context
    /// </summary>
    public class AsyncCallContext
    {
        private ConcurrentDictionary<string, AsyncLocal<object>> _context = new ConcurrentDictionary<string, AsyncLocal<object>>();

        private AsyncCallContext() { }

        internal static AsyncCallContext Current { get; } = new AsyncCallContext();

        /// <summary>
        /// Adds a key value pair to the current async flow
        /// </summary>
        /// <param name="key">The key for the value</param>
        /// <param name="value">The value to store</param>
        public void Add(string key, object value)
        {
            GetAsyncLocal(key).Value = value;
        }

        /// <summary>
        /// Gets a value from the current async flow
        /// </summary>
        /// <param name="key">The key for the value</param>
        /// <returns>The stored value or null</returns>
        public object Get(string key)
        {
            return GetAsyncLocal(key).Value;
        }

        internal void Clear()
        {
            foreach(var key in _context.Keys)
            {
                GetAsyncLocal(key).Value = null;
            }
        }

        private AsyncLocal<object> GetAsyncLocal(string key)
        {
            return _context.GetOrAdd(key, k => new AsyncLocal<object>());
        }
    }
}
