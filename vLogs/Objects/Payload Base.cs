using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Objects
{
    /// <summary>
    /// Represents log object payloads.
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Gets the type of this payload.
        /// </summary>
        PayloadTypes Type { get; }
    }
}
