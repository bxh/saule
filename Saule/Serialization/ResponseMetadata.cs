using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Saule.Serialization
{
    /// <summary>
    /// Base class for metadata. Subclasses of this class are treated differently
    /// during the serialization process.
    /// </summary>
    public abstract class ResponseMetadata : IReadOnlyDictionary<string, object>
    {
        private readonly PropertyInfo[] _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseMetadata"/> class.
        /// </summary>
        protected ResponseMetadata()
        {
            _properties = GetType().GetProperties();
        }

        /// <inheritdoc/>
        public int Count => _properties.Length;

        /// <inheritdoc/>
        public IEnumerable<string> Keys => _properties.Select(p => p.Name);

        /// <inheritdoc/>
        public IEnumerable<object> Values => _properties.Select(p => p.GetValue(this));

        /// <inheritdoc/>
        public object this[string key]
        {
            get
            {
                object value;
                var success = TryGetValue(key, out value);
                if (success)
                {
                    return value;
                }

                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.Select(p => new KeyValuePair<string, object>(
                p.Name, p.GetValue(this))).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return _properties.Any(p => p.Name == key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, out object value)
        {
            var result = _properties.FirstOrDefault(p => p.Name == key);
            if (result != null)
            {
                value = result.GetValue(this);
                return true;
            }

            value = null;
            return false;
        }
    }
}