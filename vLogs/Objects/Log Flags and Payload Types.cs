using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    /// <summary>
    /// Flags for logging objects.
    /// </summary>
    [Flags]
    public enum LogFlags
        : byte
    {
        /// <summary>
        /// Object is for debugging purposes.
        /// </summary>
        Debug = 0x80,
        /// <summary>
        /// Object is related to the termination of the application.
        /// </summary>
        Terminal = 0x40,
    }

    /// <summary>
    /// Possible types of payloads.
    /// </summary>
    [Flags]
    public enum PayloadTypes
        : byte
    {
        /// <summary>
        /// Text message of specific type.
        /// <para>Corresponds to <see cref="vLogs.Objects.MessagePayload"/>.</para>
        /// </summary>
        Message = 0x01,
        /// <summary>
        /// Exception and inner exception(s).
        /// <para>Corresponds to <see cref="vLogs.Objects.ExceptionPayload"/>.</para>
        /// </summary>
        Exception = 0x02,
        /// <summary>
        /// A collection of string keys with values, which are either strings or other key/value collections.
        /// <para>Corresponds to <see cref="vLogs.Objects.KeyValuePayload"/>.</para>
        /// </summary>
        KeyValue = 0x04,
    }
}
