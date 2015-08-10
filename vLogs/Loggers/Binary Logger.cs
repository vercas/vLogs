using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Loggers
{
    using Objects;
    using Utilities;

    /// <summary>
    /// Prepares log objects as text before logging.
    /// </summary>
    public abstract class BinaryLogger
        : IObjectLogger
    {
        /// <summary>
        /// Gets the encoding used to store character strings.
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Initializes the <see cref="vLogs.Loggers.BinaryLogger"/>.
        /// </summary>
        /// <param name="encoding">optional; The encoding used to store character strings. The default value (null) menas UTF-8.</param>
        public BinaryLogger(Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            this.Encoding = encoding;
        }

        #region IObjectLogger Members

        /// <summary>
        /// Logs the specified object as string.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        public bool Log(LogObject o)
        {
            using (var ms = new MemoryStream())
            using (var b = new BinaryWriter(ms))
            {
                b.WriteStrings(o.Sources);

                b.Write(o.Timestamp.Ticks);
                b.Write(o.Priority);
                b.Write((byte)o.Flags);
                b.Write(o.Indent);

                b.Write((byte)o.PayloadTypes);

                if ((o.PayloadTypes & PayloadTypes.Message) != 0)
                {
                    var msg = (MessagePayload)o.Payloads.Where(p => p is MessagePayload).First();

                    b.Write((byte)msg.MessageType);
                    b.WriteSpaceEfficientString(msg.Message, Encoding);
                }

                if ((o.PayloadTypes & PayloadTypes.Exception) != 0)
                {
                    var exc = (ExceptionPayload)o.Payloads.Where(p => p is ExceptionPayload).First();

                    b.WriteStrings(exc.Exceptions);
                }

                if ((o.PayloadTypes & PayloadTypes.KeyValue) != 0)
                {
                    var kvp = (KeyValuePayload)o.Payloads.Where(p => p is KeyValuePayload).First();

                    b.WriteKeyValues(kvp.Collection);
                }

                b.Flush();
                return Log(o, ms.ToArray());
            }
        }

        #endregion

        /// <summary>
        /// Logs the specified byte array representing a <see cref="vLogs.Objects.LogObject"/>.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        protected abstract bool Log(LogObject o, byte[] b);
    }
}
