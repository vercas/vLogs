using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs
{
    using Interfaces;
    using Fluent;
    using Objects;

    using Utilities;

    /// <summary>
    /// Represents an object that manages logging tasks.
    /// </summary>
    public class LogManager
        : /*ICollection<IObjectLogger>,*/ IObjectLogger, IFluentManager, IMessageLogger, IExceptionLogger
    {
        #region Properties and Fields

        private List<IObjectLogger> loggers;
        private volatile IObjectLogger[] _loggersDump;
        private object loggers_lock = new object();

        /// <summary>
        /// Gets the list of loggers used by the log manager.
        /// </summary>
        public ReadOnlyCollection<IObjectLogger> Loggers { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            this.loggers = new List<IObjectLogger>();
            this._loggersDump = new IObjectLogger[0];
            this.Loggers = new ReadOnlyCollection<IObjectLogger>(this.loggers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.LogManager"/> class with the specified loggers.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when the given loggers enumeration is null.</exception>
        public LogManager(IEnumerable<IObjectLogger> loggers)
        {
            if (loggers == null)
                throw new ArgumentNullException("loggers");

            this.loggers = new List<IObjectLogger>(loggers);
            this._loggersDump = this.loggers.ToArray();
            this.Loggers = new ReadOnlyCollection<IObjectLogger>(this.loggers);
        }

        #endregion

        #region Loggers

        private void DumpLoggers()
        {
            this._loggersDump = this.loggers.ToArray();
        }

        /// <summary>
        /// Adds the given logger to the manager.
        /// </summary>
        /// <typeparam name="TObjectLogger"></typeparam>
        /// <param name="logger"></param>
        /// <returns>The given logger.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given logger is null.</exception>
        public TObjectLogger AddLogger<TObjectLogger>(TObjectLogger logger)
            where TObjectLogger : IObjectLogger
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            lock (loggers_lock)
            {
                this.loggers.Add(logger);
                DumpLoggers();
            }

            return logger;
        }

        /// <summary>
        /// Determines whether the manager contains the specific logger.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns>True if the specific logger is contained in the manager; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given logger is null.</exception>
        public bool ContainsLogger(IObjectLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            var dump = this._loggersDump;

            for (int i = 0; i < dump.Length; i++)
                if (dump[i] == logger)
                    return true;

            return false;
        }

        /// <summary>
        /// Removes the specified logger from the manager, if found.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given logger is null.</exception>
        public bool RemoveLogger(IObjectLogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            lock (loggers_lock)
                if (this.loggers.Remove(logger))
                {
                    DumpLoggers();

                    return true;
                }
                else
                    return false;
        }

        #endregion

        /*
        #region ICollection<IObjectLogger> Members

        void ICollection<IObjectLogger>.Add(IObjectLogger item)
        {
            try
            {
                AddLogger(item);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("item");
            }
        }

        void ICollection<IObjectLogger>.Clear()
        {
            lock (loggers_lock)
            {
                this.loggers.Clear();
                this._loggersDump = new IObjectLogger[0];
            }
        }

        bool ICollection<IObjectLogger>.Contains(IObjectLogger item)
        {
            try
            {
                return ContainsLogger(item);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("item");
            }
        }

        void ICollection<IObjectLogger>.CopyTo(IObjectLogger[] array, int arrayIndex)
        {
            this._loggersDump.CopyTo(array, arrayIndex);
        }

        int ICollection<IObjectLogger>.Count
        {
            get { return this._loggersDump.Length; }
        }

        bool ICollection<IObjectLogger>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<IObjectLogger>.Remove(IObjectLogger item)
        {
            try
            {
                return RemoveLogger(item);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("item");
            }
        }

        #endregion

        #region IEnumerable<IObjectLogger> Members

        IEnumerator<IObjectLogger> IEnumerable<IObjectLogger>.GetEnumerator()
        {
            return ((IEnumerable<IObjectLogger>)this._loggersDump).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._loggersDump.GetEnumerator();
        }

        #endregion //*/

        #region IObjectLogger Members

        bool IObjectLogger.Log(LogObject o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            var res = false;

            var dump = this._loggersDump;
            //  This gets another reference to the array. "this._loggersDump" may change in the meantime without affecting the current operation.

            for (int i = 0; i < dump.Length; i++)
                res |= dump[i].Log(o);

            return res;
        }

        #endregion

        #region IFluentManager Members

        /// <summary>
        /// Initializes a new <see cref="vLogs.Interfaces.IFluentLogObject"/> for fluent modification and logging.
        /// </summary>
        /// <returns></returns>
        public IFluentLogObject WithObject()
        {
            return new FluentLogObject(this);
        }

        /// <summary>
        /// Initializes a new <see cref="vLogs.Interfaces.IFluentLogObject"/> for fluent modification and logging.
        /// </summary>
        /// <returns></returns>
        public IFluentLogObject WithObject(IEnumerable<string> sources, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags)
        {
            return new FluentLogObject(this, sources, flags, priority, indent);
        }

        /// <summary>
        /// Attempts to log the given <see cref="vLogs.Interfaces.IFluentLogObject"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given object is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when the given object does not belong to the current <see cref="vLogs.LogManager"/>.</exception>
        public bool Log(IFluentLogObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (obj.Manager != this)
                throw new InvalidOperationException("The given object does not belong to this manager.");

            return ((IObjectLogger)this).Log(new LogObject(obj.Sources, obj.Timestamp, obj.Flags, obj.Priority, obj.Indent, obj.Payloads));
        }

        #endregion

        #region Specialized Logging Methods

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
        public bool LogMessage(string message, IList<string> sources, MessageType type = Defaults._MessageType, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags)
        {
            return ((IObjectLogger)this).Log(new LogObject(sources, DateTime.Now, flags, priority, indent, new IPayload[] { new MessagePayload(type, message) }));
        }

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
        public bool LogException(Exception x, IList<string> sources, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, LogFlags flags = Defaults.LogFlags)
        {
            return ((IObjectLogger)this).Log(new LogObject(sources, DateTime.Now, flags, priority, indent, new IPayload[] { new ExceptionPayload(x) }));
        }

        #endregion
    }
}
