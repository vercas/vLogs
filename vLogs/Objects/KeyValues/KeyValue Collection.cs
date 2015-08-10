using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Objects.KeyValues
{
    /// <summary>
    /// Represents a collection of key/value pairs. This class cannot be inherited.
    /// </summary>
    public sealed class KeyValueCollection
        : IList<KeyValuePair>
    {
        private KeyValuePair[] _arr;
        int count = 0;
        bool locked = false;

        private void _Double()
        {
            var temp = _arr;
            _arr = new KeyValuePair[temp.Length * 2];
            Array.Copy(temp, _arr, temp.Length);
        }

        #region IList<KeyValuePair> Members

        /// <summary>
        /// Determines the index of a specific key/value pair in the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item is null.</exception>
        public int IndexOf(KeyValuePair item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            for (int i = 0; i < count; i++)
                if (_arr[i] == item)
                    return i;

            return -1;
        }

        /// <summary>
        /// Inserts a key/value pair to the collection at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given index is negative or greater than the number of items in the collection.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is locked (read-only).</exception>
        public void Insert(int index, KeyValuePair item)
        {
            if (locked)
                throw new InvalidOperationException("The collection is locked (read-only).");

            if (index < 0 || index > count)
                throw new ArgumentOutOfRangeException("index", "Given index must not be nagative or greater than the number of items in the collection.");

            if (item == null)
                throw new ArgumentNullException("item");

            for (int i = 0; i < count; i++)
                if (_arr[i].Key == item.Key)
                    throw new ArgumentException("A key/value pair with the key of the given item already exists in the collection.");

            if (count == _arr.Length)
                _Double();

            for (int i = count; i > index; i--)
                _arr[i] = _arr[i - 1];

            _arr[index] = item;

            count++;
        }

        /// <summary>
        /// Removes the key/value pair at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given index is negative or greater than or equal to the number of items in the collection.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is locked (read-only).</exception>
        public void RemoveAt(int index)
        {
            if (locked)
                throw new InvalidOperationException("The collection is locked (read-only).");

            if (index < 0 || index >= count)
                throw new ArgumentOutOfRangeException("index", "Given index must be positive and less than the number of items in the collection.");

            for (int i = index; i < count - 1; i++)
                _arr[i] = _arr[i + 1];

            _arr[count - 1] = null;

            count--;
        }

        /// <summary>
        /// Gets or sets the key/value pair at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given value is null.</exception>
        /// <exception cref="System.IndexOutOfRangeException">Thrown when the given index is negative or greater than or equal to the number of items in the collection.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when setting and the collection is locked (read-only).</exception>
        public KeyValuePair this[int index]
        {
            get
            {
                if (index < 0 || index > count)
                    throw new IndexOutOfRangeException("Given index must be positive and less than the number of items in the collection.");

                return _arr[index];
            }
            set
            {
                if (locked)
                    throw new InvalidOperationException("The collection is locked (read-only).");

                if (index < 0 || index > count)
                    throw new IndexOutOfRangeException("Given index must be positive and less than the number of items in the collection.");

                if (value == null)
                    throw new ArgumentNullException("value");

                for (int i = 0; i < count; i++)
                    if (i != index && _arr[i].Key == value.Key)
                        throw new ArgumentException("A key/value pair with the key of the given item already exists in the collection.");

                _arr[index] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair> Members

        /// <summary>
        /// Adds a key/value pair to the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is locked (read-only).</exception>
        public void Add(KeyValuePair item)
        {
            if (locked)
                throw new InvalidOperationException("The collection is locked (read-only).");

            if (item == null)
                throw new ArgumentNullException("item");

            for (int i = 0; i < count; i++)
                if (_arr[i].Key == item.Key)
                    throw new ArgumentException("A key/value pair with the key of the given item already exists in the collection.");

            if (count == _arr.Length)
                _Double();

            _arr[count] = item;

            count++;
        }

        /// <summary>
        /// Removes all key/value pairs from the collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is locked (read-only).</exception>
        public void Clear()
        {
            if (locked)
                throw new InvalidOperationException("The collection is locked (read-only).");

            for (int i = 0; i < count; i++)
                _arr[i] = null;

            count = 0;
        }

        /// <summary>
        /// Determines whether the collection contains a specific key/value pair.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item is null.</exception>
        public bool Contains(KeyValuePair item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            for (int i = 0; i < count; i++)
                if (_arr[i] == item)
                    return true;

            return false;
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given array is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given array index is negative or would not leave enough room to copy all the items in the collection to the destination array.</exception>
        public void CopyTo(KeyValuePair[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0 || arrayIndex >= array.Length - count)
                throw new ArgumentOutOfRangeException("arrayIndex", "Given index must be positive and must allow room for all pairs in the collection for copying.");

            Array.Copy(_arr, 0, array, arrayIndex, count);
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return locked; }
        }

        /// <summary>
        /// Removes the specific key/value pair from the collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is locked (read-only).</exception>
        public bool Remove(KeyValuePair item)
        {
            if (locked)
                throw new InvalidOperationException("The collection is locked (read-only).");

            for (int j = 0; j < count; j++)
                if (_arr[j] == item)
                {
                    for (int i = j; i < count - 1; i++)
                        _arr[i] = _arr[i + 1];

                    _arr[count - 1] = null;

                    count--;

                    return true;
                }

            return false;
        }

        #endregion

        #region IEnumerable<KeyValuePair> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return _arr[i];
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return _arr[i];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValueCollection"/> class.
        /// </summary>
        public KeyValueCollection()
        {
            this._arr = new KeyValuePair[2];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValueCollection"/> class from the given enumeration of <see cref="vLogs.Objects.KeyValues.KeyValuePair"/>s and an optional indication whether the collection should be locked or not.
        /// </summary>
        /// <param name="kvps"></param>
        /// <param name="locked">optional; True to lock the collection (making it read-only); otherwise false.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given enumeration contains two pairs with the same key.</exception>
        public KeyValueCollection(IEnumerable<KeyValuePair> kvps, bool locked = false)
        {
            if (kvps == null)
                throw new ArgumentNullException("kvps");

            this._arr = kvps.ToArray();
            this.count = this._arr.Length;

            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    if (i != j && this._arr[i].Key == this._arr[j].Key)
                        throw new ArgumentException("The given enumeration of key/value pairs contains two pairs with the same key.");

            if (count == 0)
                this._arr = new KeyValuePair[2];

            this.locked = locked;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValues.KeyValueCollection"/> class from the given enumeration of <see cref="vLogs.Objects.KeyValues.KeyValuePair"/>s and an optional indication whether the collection should be locked or not.
        /// </summary>
        /// <param name="locked">True to lock the collection (making it read-only); otherwise false.</param>
        /// <param name="kvps"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given array contains two pairs with the same key.</exception>
        public KeyValueCollection(bool locked, params KeyValuePair[] kvps)
        {
            if (kvps == null)
                throw new ArgumentNullException("kvps");

            for (int i = 0; i < kvps.Length; i++)
                for (int j = 0; j < kvps.Length; j++)
                    if (i != j && kvps[i].Key == kvps[j].Key)
                        throw new ArgumentException("The given enumeration of key/value pairs contains two pairs with the same key.");

            this.count = kvps.Length;
            this._arr = new KeyValuePair[this.count];

            if (this.count == 0)
                this._arr = new KeyValuePair[2];
            else
                Array.Copy(kvps, this._arr, this.count);

            this.locked = locked;
        }

        #endregion

        /// <summary>
        /// Locks the collection, making it read-only.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the collection is already locked (read-only).</exception>
        public void Lock()
        {
            if (this.locked)
                throw new InvalidOperationException("The collection is already locked.");

            this.locked = true;
        }
    }
}
