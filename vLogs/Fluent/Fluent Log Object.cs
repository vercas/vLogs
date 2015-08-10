using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Fluent
{
    using Interfaces;
    using Objects;

    using Utilities;

    /// <summary>
    /// Represents a mutable log object which can be modified and logged fluently.
    /// </summary>
    public class FluentLogObject
        : IFluentLogObject
    {
        #region Properties

        private List<string> sources;
        private List<IPayload> payloads;

        private IFluentManager Manager;
        private ReadOnlyCollection<string> Sources;
        private DateTime Timestamp;
        private LogFlags Flags;
        private SByte Priority;
        private Byte Indent;
        private PayloadTypes PayloadTypes;
        private ReadOnlyCollection<IPayload> Payloads;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the object with the given property values.
        /// </summary>
        /// <param name="manager">The manager which owns the object; must be non-null.</param>
        /// <param name="timestamp">The creation timestamp.</param>
        /// <param name="flags">Flags for logging the object.</param>
        /// <param name="priority">Relative priority of the object.</param>
        /// <param name="indent">Logical indentation of the object.</param>
        /// <param name="sources">An enumeration of sources of the object.</param>
        /// <param name="payloads">An enumeration of payloads of the object.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manager object is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated source is null -or- an enumerated payload is null -or- a given payload is of unknown type -or- two payloads are of the same type.</exception>
        protected FluentLogObject(IFluentManager manager, DateTime timestamp, LogFlags flags = Defaults.LogFlags, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent, IEnumerable<string> sources = null, IEnumerable<IPayload> payloads = null)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            this.Manager = manager;
            this.Timestamp = timestamp;
            this.Flags = flags;
            this.Priority = priority;
            this.Indent = indent;

            if (sources == null)
            {
                this.sources = new List<string>();
            }
            else
            {
                this.sources = new List<string>(sources);

                for (int i = 0; i < this.sources.Count; i++)
                    if (this.sources[i] == null)
                        throw new ArgumentException("The enumeration of sources contains a null element.", "sources");
            }

            this.Sources = new ReadOnlyCollection<string>(this.sources);

            if (payloads == null)
            {
                this.payloads = new List<IPayload>();
            }
            else
            {
                var plds = payloads.ToArray();

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

                this.payloads = new List<IPayload>(plds);
            }

            this.Payloads = new ReadOnlyCollection<IPayload>(this.payloads);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Fluent.FluentLogObject"/> class with the specified fluent logging manager.
        /// </summary>
        /// <param name="manager">The fluent logging manager which owns the current object.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manager object is null.</exception>
        public FluentLogObject(IFluentManager manager)
            : this(manager, DateTime.Now)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Fluent.FluentLogObject"/> class with the specified fluent logging manager, sources enumeration, logging flags, priority and indent.
        /// </summary>
        /// <param name="manager">The fluent logging manager which owns the current object.</param>
        /// <param name="sources">An enumeration of sources for the log object.</param>
        /// <param name="flags">optional; Flags for logging the object.</param>
        /// <param name="priority">optional; Relative priority of the object.</param>
        /// <param name="indent">optional; Logical indentation of the object.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given manager object is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an enumerated source is null.</exception>
        public FluentLogObject(IFluentManager manager, IEnumerable<string> sources, LogFlags flags = Defaults.LogFlags, sbyte priority = Defaults.Priority, byte indent = Defaults.Indent)
            : this(manager, DateTime.Now, flags, priority, indent, sources)
        {

        }

        #endregion

        #region IFluentLogObject Members

        IFluentManager IFluentLogObject.Manager
        {
            get { return this.Manager; }
        }

        ReadOnlyCollection<string> IFluentLogObject.Sources
        {
            get { return this.Sources; }
        }

        DateTime IFluentLogObject.Timestamp
        {
            get { return this.Timestamp; }
        }

        LogFlags IFluentLogObject.Flags
        {
            get { return this.Flags; }
        }

        sbyte IFluentLogObject.Priority
        {
            get { return this.Priority; }
        }

        byte IFluentLogObject.Indent
        {
            get { return this.Indent; }
        }

        PayloadTypes IFluentLogObject.PayloadTypes
        {
            get { return this.PayloadTypes; }
        }

        ReadOnlyCollection<IPayload> IFluentLogObject.Payloads
        {
            get { return this.Payloads; }
        }

        IFluentLogObject IFluentLogObject.WithSources(IEnumerable<string> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");

            this.sources.Clear();
            this.sources.AddRange(sources);

            for (int i = 0; i < this.sources.Count; i++)
                if (this.sources[i] == null)
                    throw new ArgumentException("The enumeration of sources contains a null element.", "sources");

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraSources(IEnumerable<string> sources)
        {
            if (sources == null)
                throw new ArgumentNullException("sources");

            var start = this.sources.Count;

            this.sources.AddRange(sources);

            for (int i = start; i < this.sources.Count; i++)
                if (this.sources[i] == null)
                    throw new ArgumentException("The enumeration of extra sources contains a null element.", "sources");

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraSource(string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.sources.Add(source);

            return this;
        }

        IFluentLogObject IFluentLogObject.WithTimestamp(DateTime timestamp)
        {
            this.Timestamp = timestamp;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithFlags(LogFlags flags)
        {
            this.Flags = flags;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraFlags(LogFlags flags)
        {
            this.Flags |= flags;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithoutFlags(LogFlags flags)
        {
            this.Flags &= ~flags;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithPriority(sbyte priority)
        {
            this.Priority = priority;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraPriority(int priorityDifference)
        {
            int res = this.Priority + priorityDifference;

            if (res < sbyte.MinValue)
                throw new ArgumentOutOfRangeException("priorityDifference", string.Format("Addition results in a number below the minimum value of a signed byte ({0}).", sbyte.MinValue));
            if (res > sbyte.MaxValue)
                throw new ArgumentOutOfRangeException("priorityDifference", string.Format("Addition results in a number above the maximum value of a signed byte ({0}).", sbyte.MaxValue));

            this.Priority = (sbyte)res;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithIndent(byte indent)
        {
            this.Indent = indent;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraIndent(int indentDifference)
        {
            int res = this.Indent + indentDifference;

            if (res < byte.MinValue)
                throw new ArgumentOutOfRangeException("priorityDifference", string.Format("Addition results in a number below the minimum value of an unsigned byte ({0}).", byte.MinValue));
            if (res > byte.MaxValue)
                throw new ArgumentOutOfRangeException("priorityDifference", string.Format("Addition results in a number above the maximum value of an unsigned byte ({0}).", byte.MaxValue));

            this.Indent = (byte)res;

            return this;
        }

        IFluentLogObject IFluentLogObject.WithPayloads(IEnumerable<IPayload> payloads)
        {
            if (payloads == null)
                throw new ArgumentNullException("payloads");

            this.payloads.Clear();
            this.PayloadTypes = 0;

            this.payloads.AddRange(payloads);

            for (int i = 0; i < this.payloads.Count; i++)
            {
                PayloadTypes pldt;

                try
                {
                    pldt = this.payloads[i].GetType().GetPayloadType();
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

            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraPayloads(IEnumerable<IPayload> payloads)
        {
            if (payloads == null)
                throw new ArgumentNullException("payloads");

            var start = this.payloads.Count;

            this.payloads.AddRange(payloads);

            for (int i = start; i < this.payloads.Count; i++)
            {
                PayloadTypes pldt;

                try
                {
                    pldt = this.payloads[i].GetType().GetPayloadType();
                }
                catch (ArgumentException x)
                {
                    throw new ArgumentException("Extra payloads enumeration contains a payload of unkown type.", x);
                }
                catch (NullReferenceException x)
                {
                    throw new ArgumentException("Extra payloads enumeration contains a null payload.", x);
                }

                if ((this.PayloadTypes & pldt) == 0)
                    this.PayloadTypes |= pldt;
                else
                    throw new InvalidOperationException("An attempt was made to add a payload of a type that was already added to the object." + Environment.NewLine + "Duplicated type: " + pldt);
            }
            
            return this;
        }

        IFluentLogObject IFluentLogObject.WithExtraPayload(IPayload payload)
        {
            if (payload == null)
                throw new ArgumentNullException("payload");

            PayloadTypes pldt;

            try
            {
                pldt = payload.GetType().GetPayloadType();
            }
            catch (ArgumentException x)
            {
                throw new ArgumentException("Given payload is of unkown type.", x);
            }

            if ((this.PayloadTypes & pldt) == 0)
                this.PayloadTypes |= pldt;
            else
                throw new InvalidOperationException("The object already contains a payload of the same type." + Environment.NewLine + "Duplicated type: " + pldt);

            this.payloads.Add(payload);

            return this;
        }

        IFluentLogObject IFluentLogObject.WithMessage(string message, MessageType type)
        {
            return ((IFluentLogObject)this).WithExtraPayload(new MessagePayload(type, message));
        }

        IFluentLogObject IFluentLogObject.WithException(Exception x)
        {
            return ((IFluentLogObject)this).WithExtraPayload(new ExceptionPayload(x));
        }

        IFluentLogObject IFluentLogObject.Log()
        {
            Manager.Log(this);

            return this;
        }

        IFluentLogObject IFluentLogObject.Log(out bool res)
        {
            res = Manager.Log(this);

            return this;
        }

        #endregion
    }
}
