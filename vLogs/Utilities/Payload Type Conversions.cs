using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Utilities
{
    using Objects;

    /// <summary>
    /// Contains methods for dealing with type of payloads.
    /// </summary>
    public static class PayloadTypeConversions
    {
        private static readonly Type MessagePayloadType = typeof(MessagePayload);
        private static readonly Type ExceptionPayloadType = typeof(ExceptionPayload);
        private static readonly Type KeyValuePayloadType = typeof(KeyValuePayload);

        /// <summary>
        /// Gets the <see cref="vLogs.Objects.PayloadTypes"/> associated with the given <see cref="System.Type"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given type is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given type is not a known payload type.</exception>
        public static PayloadTypes GetPayloadType(this Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            if (t == MessagePayloadType)
                return PayloadTypes.Message;
            if (t == ExceptionPayloadType)
                return PayloadTypes.Exception;
            if (t == KeyValuePayloadType)
                return PayloadTypes.KeyValue;

            throw new ArgumentException("The given type is not a payload type.", "t");
        }

        /// <summary>
        /// Gets the <see cref="System.Type"/> associated with the given specific <see cref="vLogs.Objects.PayloadTypes"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the given enumeration value does not correspond to a single known payload type.</exception>
        public static Type GetPayloadType(this PayloadTypes t)
        {
            switch (t)
            {
                case PayloadTypes.Message:
                    return MessagePayloadType;
                case PayloadTypes.Exception:
                    return ExceptionPayloadType;
                case PayloadTypes.KeyValue:
                    return KeyValuePayloadType;

                default:
                    throw new ArgumentOutOfRangeException("t", "The given enumeration value does not constitute a single known payload type.");
            }
        }
    }
}
