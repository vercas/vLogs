using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    using KeyValues;

    using Utilities;

    /// <summary>
    /// A payload containing a collection of key/value pairs.
    /// </summary>
    public class KeyValuePayload
        : IPayload
    {
        #region IPayload Members

        /// <summary>
        /// Gets the type of this payload.
        /// </summary>
        public PayloadTypes Type
        {
            get { return PayloadTypes.KeyValue; }
        }

        #endregion
        
        /// <summary>
        /// Gets the key/value collection contained in the payload.
        /// </summary>
        public KeyValueCollection Collection {get; private set;}

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.KeyValuePayload"/> class with the specified key/value collection.
        /// </summary>
        /// <param name="col"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given collection is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given collection is not locked (read-only).</exception>
        public KeyValuePayload(KeyValueCollection col)
        {
            if (col == null)
                throw new ArgumentNullException("col");
            if (!col.IsReadOnly)
                throw new ArgumentException("The given key/value collection must be locked (read-only) to assure the immutability of the payload.", "col");

            this.Collection = col;
        }
    }
}
