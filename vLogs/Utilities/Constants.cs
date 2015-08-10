using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Utilities
{
    using Objects;

    /// <summary>
    /// Holds default values for some parameters and properties.
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// Default log flags - none.
        /// </summary>
        public const LogFlags LogFlags = 0;

        /// <summary>
        /// Default priority - 0.
        /// </summary>
        public const sbyte Priority = 0;

        /// <summary>
        /// Default indent - 0.
        /// </summary>
        public const byte Indent = 0;

        /// <summary>
        /// Default message type - information.
        /// </summary>
        public const MessageType _MessageType = MessageType.Information;
    }
}
