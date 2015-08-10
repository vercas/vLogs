using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace vLogs.Interfaces
{
    using Objects;

    using Utilities;

    /// <summary>
    /// Represents an object that can log messages.
    /// </summary>
    public interface IMessageLogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sources">A hierarchically-descending list of sources of the message.</param>
        /// <param name="type">optional; The type of message.</param>
        /// <param name="priority">optional; The relative priority of the message.</param>
        /// <param name="indent">optional; The logical indentation of the message.</param>
        /// <param name="flags">optional; Flags for logging the message.</param>
        /// <returns>True if the message was logged; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given message string or sources list is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given sources list contains a null string.</exception>
        bool LogMessage(string message, IList<string> sources, MessageType type = Defaults._MessageType, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags);
    }

    /// <summary>
    /// Represents an object that can log exceptions.
    /// </summary>
    public interface IExceptionLogger
    {
        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sources">A hierarchically-descending list of sources of the message.</param>
        /// <param name="priority">optional; The relative priority of the message.</param>
        /// <param name="indent">optional; The logical indentation of the message.</param>
        /// <param name="flags">optional; Flags for logging the message.</param>
        /// <returns>True if the message was logged; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given exception object or sources list is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given sources list contains a null string.</exception>
        bool LogException(Exception x, IList<string> sources, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags);
    }
}
