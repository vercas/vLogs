using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Loggers.Desktop.Partitioners
{
    using Objects;

    /// <summary>
    /// Represents an object which partitions binary log data.
    /// </summary>
    public interface IBinaryPartitioner
    {
        /// <summary>
        /// Partitions the given log object with its binary representation.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        string Partition(LogObject o, byte[] b);
    }

    /// <summary>
    /// Represents an object which partitions textual log data.
    /// </summary>
    public interface ITextPartitioner
    {
        /// <summary>
        /// Partitions the given log object with its textual representation.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        string Partition(LogObject o, string s);
    }

    /// <summary>
    /// Represents an object which partitions raw log data.
    /// </summary>
    public interface IObjectPartitioner
    {
        /// <summary>
        /// Partitions the given log object.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        string Partition(LogObject o);
    }
}
