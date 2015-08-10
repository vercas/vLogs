using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    using Utilities;

    /// <summary>
    /// A payload containing a described exception.
    /// </summary>
    public class MessagePayload
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
        /// Gets the type of message of the payload.
        /// </summary>
        public MessageType MessageType { get; internal set; }

        /// <summary>
        /// Gets the message of the payload.
        /// </summary>
        public String Message { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Objects.MessagePayload"/> class with the specified message and type.
        /// </summary>
        /// <param name="type">The type of message.</param>
        /// <param name="message"></param>
        public MessagePayload(MessageType type, string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.MessageType = type;
            this.Message = message;
        }
    }

    /// <summary>
    /// Possible logging message types.
    /// </summary>
    public enum MessageType
        : byte
    {
        /// <summary>
        /// Message is for informative purposes.
        /// </summary>
        Information = 0,
        /// <summary>
        /// Message is for warning purposes.
        /// </summary>
        Warning = 1,
        /// <summary>
        /// Message represents an error.
        /// </summary>
        Error = 2,
    }
}
