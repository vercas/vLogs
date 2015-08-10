using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Objects.KeyValues
{
    /// <summary>
    /// Represents a key and a value or a collection of key/value pairs.
    /// </summary>
    public class KeyValuePair
    {
        #region Properties and Fields

        private string _key;
        private KvpValueType _type;
        
        private string _string;
        private KeyValueCollection _col;
        private long _long;
        private double _double;

        /// <summary>
        /// Gets the key of the pair.
        /// </summary>
        public String Key { get { return _key; } }

        /// <summary>
        /// Gets the string value of the pair, if available.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the pair does not contain a string value.</exception>
        public String String
        {
            get
            {
                if (_type != KvpValueType.String)
                    throw new InvalidOperationException("The key/value pair does not contain a string value.");

                return _string;
            }
        }

        /// <summary>
        /// Gets the collection value of the pair, if available.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the pair does not contain a collection value.</exception>
        public KeyValueCollection Collection
        {
            get
            {
                if (_type != KvpValueType.Collection)
                    throw new InvalidOperationException("The key/value pair does not contain a collection value.");

                return _col;
            }
        }

        /// <summary>
        /// Gets the long value of the pair, if available.
        /// </summary>
        public Int64 Long
        {
            get
            {
                if (_type != KvpValueType.Long)
                    throw new InvalidOperationException("The key/value pair does not contain a long value.");

                return _long;
            }
        }

        /// <summary>
        /// Gets the double value of the pair, if available.
        /// </summary>
        public Double Double
        {
            get
            {
                if (_type != KvpValueType.Double)
                    throw new InvalidOperationException("The key/value pair does not contain a long value.");

                return _double;
            }
        }

        /// <summary>
        /// Gets the type of value contained by the pair.
        /// </summary>
        public KvpValueType ValueType { get { return _type; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValuePair"/> class with the specified key and string value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key or value are null.</exception>
        public KeyValuePair(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            this._key = key;
            this._type = KvpValueType.String;
            this._string = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValuePair"/> class with the specified key and collection value.
        /// </summary>
        /// <remarks>
        /// The given collection must be locked (read-only).
        /// </remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key or value are null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given collection is not locked (read-only).</exception>
        public KeyValuePair(string key, KeyValueCollection value)
        {
            if (key == null) 
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            if (!value.IsReadOnly)
                throw new ArgumentException("The given key/value collection must be locked (read-only) to assure the immutability of the object.", "value");

            this._key = key;
            this._type = KvpValueType.Collection;
            this._col = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValuePair"/> class with the specified key and key/value pairs to add to the collection.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="kvps"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public KeyValuePair(string key, params KeyValuePair[] kvps)
            : this(key, new KeyValueCollection(true, kvps))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValuePair"/> class with the specified key and long value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public KeyValuePair(string key, long value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            this._key = key;
            this._type = KvpValueType.Long;
            this._long = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValuePair"/> class with the specified key and double value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public KeyValuePair(string key, double value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            this._key = key;
            this._type = KvpValueType.Double;
            this._double = value;
        }

        #endregion
    }

    /// <summary>
    /// Possible value types for <see cref="vLogs.Objects.KeyValues.KeyValuePair"/>s.
    /// </summary>
    public enum KvpValueType
        : byte
    {
        /// <summary>
        /// Value is of type <see cref="System.String"/>.
        /// </summary>
        String = 0,
        /// <summary>
        /// Value is of type <see cref="System.Int64"/>.
        /// </summary>
        Long = 1,
        /// <summary>
        /// Value is of type <see cref="System.Double"/>.
        /// </summary>
        Double = 2,
        /// <summary>
        /// Value is of type <see cref="vLogs.Objects.KeyValues.KeyValueCollection"/>.
        /// </summary>
        Collection = 3,
    }
}
