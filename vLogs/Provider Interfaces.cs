using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs
{
    using Objects;

    /// <summary>
    /// Represents an object which provides logging of messages and/or exceptions.
    /// </summary>
    public interface IObjectLogger
    {
        /// <summary>
        /// Logs the specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given object is null.</exception>
        bool Log(LogObject o);
    }
}
