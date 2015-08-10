using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    using Utilities;

    /// <summary>
    /// Represents an immutable object which can be logged. This class cannot be inherited.
    /// </summary>
    public sealed class LogObject
    {
        #region Properties

        /// <summary>
        /// Gets the read-only hierachically-descending sequence of named sources of this log object.
        /// </summary>
        public ReadOnlyCollection<string> Sources { get; internal set; }

        /// <summary>
        /// Gets the instant in time at which the object was emitted.
        /// </summary>
        public DateTime Timestamp { get; internal set; }

        /// <summary>
        /// Gets a set of flags describing the object.
        /// </summary>
        public LogFlags Flags { get; internal set; }

        /// <summary>
        /// Gets the relative priority of the object.
        /// </summary>
        /// <example>
        /// 0 means neutral, 127 means top-priority and -128 means practically meaningless.
        /// </example>
        public SByte Priority { get; internal set; }

        /// <summary>
        /// Gets the logical indentation level of the object.
        /// </summary>
        public Byte Indent { get; internal set; }

        /// <summary>
        /// Gets the types of payload of the object.
        /// </summary>
        public PayloadTypes PayloadTypes { get; internal set; }

        /// <summary>
        /// Gets the payloads of the object.
        /// </summary>
        public ReadOnlyCollection<IPayload> Payloads { get; internal set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.LogObject"/> class with the specified sources, timestamp, flags, priority, indent and payload.
        /// </summary>
        /// <remarks>
        /// Individual sources may be null.
        /// </remarks>
        /// <param name="sources">Hierachically-descending sequence of named sources.</param>
        /// <param name="timestamp">Instant in time at which the object is emitted.</param>
        /// <param name="flags">Set of flags describing the object.</param>
        /// <param name="priority">Relative priority.</param>
        /// <param name="indent">Logical indentation level.</param>
        /// <param name="payloads">An enumeration of different payloads of the object.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given sources sequence or payloads enumeration are null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when a given payload is of unknown type -or- two payloads are of the same type -or- when a payload is null -or- a source is null.</exception>
        public LogObject(IList<string> sources, DateTime timestamp, LogFlags flags, sbyte priority, byte indent, IEnumerable<IPayload> payloads)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");
            if (payloads == null)
                throw new ArgumentNullException("payloads");

            this.Timestamp = timestamp;
            this.Flags = flags;
            this.Priority = priority;
            this.Indent = indent;

            for (int i = 0; i < sources.Count; i++)
                if (sources[i] == null)
                    throw new ArgumentException(string.Format("The list of sources contains a null element at index {0}.", i), "sources");

            this.Sources = new ReadOnlyCollection<string>(sources);

            var plds = payloads.ToArray();

            this.Payloads = new ReadOnlyCollection<IPayload>(plds);

            for (int i = 0; i < plds.Length; i++)
            {
                PayloadTypes pldt;

                try
                {
                    pldt = plds[i].GetType().GetPayloadType();
                }
                catch (ArgumentException x)
                {
                    throw new ArgumentException("Payloads enumeration contains a payload of unkown type.", x);
                }
                catch (NullReferenceException x)
                {
                    throw new ArgumentException("Payloads enumeration contains a null payload.", x);
                }

                if ((this.PayloadTypes & pldt) == 0)
                    this.PayloadTypes |= pldt;
                else
                    throw new ArgumentException("Payloads enumerations may not contain two payloads of the same type." + Environment.NewLine + "Duplicated type: " + pldt);
            }
        }
    }
}
