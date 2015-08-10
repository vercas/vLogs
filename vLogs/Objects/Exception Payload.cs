using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    using Utilities;

    /// <summary>
    /// A payload containing a series of exceptions and inner exceptions.
    /// </summary>
    public class ExceptionPayload
        : IPayload
    {
        #region IPayload Members

        /// <summary>
        /// Gets the type of this payload.
        /// </summary>
        public PayloadTypes Type
        {
            get { return PayloadTypes.Exception; }
        }

        #endregion
        
        /// <summary>
        /// Gets a sequence of strings representing exceptions (and inner exceptions).
        /// </summary>
        public ReadOnlyCollection<string> Exceptions { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.ExceptionPayload"/> class with the specified exception, and, optionally, its inner exceptions.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="includeAllInnerExceptions">optional; True to include all inner exceptions in the payload; otherwise false for just the given exception.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given exception is null.</exception>
        public ExceptionPayload(Exception x, bool includeAllInnerExceptions = false)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (includeAllInnerExceptions)
            {
                List<string> exceptions = new List<string>(2);

                while (x != null)
                {
                    exceptions.Add(x.ToString());
                    x = x.InnerException;
                }

                this.Exceptions = new ReadOnlyCollection<string>(exceptions);
            }
            else
            {
                this.Exceptions = new ReadOnlyCollection<string>(new string[1] { x.ToString() });
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.ExceptionPayload"/> class with the specified sequence of exceptions and inner exceptions.
        /// </summary>
        /// <param name="x"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given list of strings is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given list of strings contains a null string.</exception>
        public ExceptionPayload(IList<string> x)
        {
            if (x == null)
                throw new ArgumentNullException("x");

            if (x.Where(s => s == null).Any())
                throw new ArgumentException("List of strings must not contain null strings.", "x");

            this.Exceptions = new ReadOnlyCollection<string>(x);
        }
    }
}
