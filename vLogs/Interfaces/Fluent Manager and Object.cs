using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Interfaces
{
    using Fluent;
    using Objects;

    using Utilities;

    /// <summary>
    /// Represents a logging manager which can initialize, modify and log objects fluently.
    /// </summary>
    public interface IFluentManager
    {
        /// <summary>
        /// Initializes a new <see cref="vLogs.Interfaces.IFluentLogObject"/> for fluent modification and logging.
        /// </summary>
        /// <returns></returns>
        IFluentLogObject WithObject();

        /// <summary>
        /// Initializes a new <see cref="vLogs.Interfaces.IFluentLogObject"/> for fluent modification and logging.
        /// </summary>
        /// <returns></returns>
        IFluentLogObject WithObject(IEnumerable<string> sources, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags);

        /// <summary>
        /// Attempts to log the given <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given object is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the given object does not belong to the current <see cref="vLogs.Interfaces.IFluentManager"/>.</exception>
        bool Log(IFluentLogObject obj);
    }

    /// <summary>
    /// Represents a log object which can be modified and logged fluently.
    /// </summary>
    public interface IFluentLogObject
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="vLogs.Interfaces.IFluentManager"/> which created the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        IFluentManager Manager { get; }

        /// <summary>
        /// Gets the read-only hierachically-descending sequence of named sources of this object.
        /// </summary>
        ReadOnlyCollection<string> Sources { get; }

        /// <summary>
        /// Gets the instant in time at which the object was emitted.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets a set of flags describing the object.
        /// </summary>
        LogFlags Flags { get; }

        /// <summary>
        /// Gets the relative priority of the object.
        /// </summary>
        /// <example>
        /// 0 means neutral, 127 means top-priority and -128 means practically meaningless.
        /// </example>
        SByte Priority { get; }

        /// <summary>
        /// Gets the logical indentation level of the object.
        /// </summary>
        Byte Indent { get; }

        /// <summary>
        /// Gets the types of payload of the object.
        /// </summary>
        PayloadTypes PayloadTypes { get; }

        /// <summary>
        /// Gets the payloads of the object.
        /// </summary>
        ReadOnlyCollection<IPayload> Payloads { get; }

        #endregion

        #region Fluent Methods

        /// <summary>
        /// Sets the given sources to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given sources enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated source is null.</exception>
        IFluentLogObject WithSources(IEnumerable<string> sources);

        /// <summary>
        /// Appends the given sources to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="sources"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given sources enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated source is null.</exception>
        IFluentLogObject WithExtraSources(IEnumerable<string> sources);

        /// <summary>
        /// Appends the given source to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given source is null.</exception>
        IFluentLogObject WithExtraSource(string source);

        /// <summary>
        /// Applies the given timestamp to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithTimestamp(DateTime timestamp);

        /// <summary>
        /// Applies the given log flags to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithFlags(LogFlags flags);

        /// <summary>
        /// Adds the given log flags to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithExtraFlags(LogFlags flags);

        /// <summary>
        /// Removes the given log flags to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithoutFlags(LogFlags flags);

        /// <summary>
        /// Applies the given priority to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="priority"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithPriority(sbyte priority);

        /// <summary>
        /// Adds the given priority to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="priorityDifference"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the resulted addition is outside of the range of <see cref="System.SByte"/>.</exception>
        IFluentLogObject WithExtraPriority(int priorityDifference);

        /// <summary>
        /// Applies the given indentation to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="indent"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject WithIndent(byte indent);

        /// <summary>
        /// Adds the given indentation to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="indentDifference"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the resulted addition is outside of the range of <see cref="System.Byte"/>.</exception>
        IFluentLogObject WithExtraIndent(int indentDifference);

        /// <summary>
        /// Sets the given payloads to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="payloads"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given payloads enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated payload is null -or- a given payload is of unknown type -or- two payloads are of the same type.</exception>
        IFluentLogObject WithPayloads(IEnumerable<IPayload> payloads);

        /// <summary>
        /// Appends the given payloads to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="payloads"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given payloads enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated payload is null -or- a given payload is of unknown type -or- two payloads are of the same type.</exception>
        IFluentLogObject WithExtraPayloads(IEnumerable<IPayload> payloads);

        /// <summary>
        /// Appends the given payloads to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given payload is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given payload is of unknown type.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the object already contains a payload of the same type.</exception>
        IFluentLogObject WithExtraPayload(IPayload payload);

        #endregion

        #region Specialized Fluent Methods

        /// <summary>
        /// Appends a message payload with the given data to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type">optional; The type of the message.</param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given message string is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the object already contains a message payload.</exception>
        IFluentLogObject WithMessage(string message, MessageType type = Defaults._MessageType);

        /// <summary>
        /// Appends an exception payload with the given data to the <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given exception object is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the object already contains an exception payload.</exception>
        IFluentLogObject WithException(Exception x);

        #endregion

        #region Logging

        /// <summary>
        /// Logs the <see cref="vLogs.Interfaces.IFluentLogObject"/> with its owning <see cref="vLogs.Interfaces.IFluentManager"/>.
        /// </summary>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject Log();

        /// <summary>
        /// Logs the <see cref="vLogs.Interfaces.IFluentLogObject"/> with its owning <see cref="vLogs.Interfaces.IFluentManager"/> and gives back the result.
        /// </summary>
        /// <param name="res">True if the object was logged by the manager; otherwise false.</param>
        /// <returns>The current <see cref="vLogs.Interfaces.IFluentLogObject"/>.</returns>
        IFluentLogObject Log(out bool res);

        #endregion
    }
}
