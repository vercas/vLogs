using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vLogs.Loggers.Desktop.Partitioners
{
    using Objects;

    /// <summary>
    /// Partitions all log objects to the same file.
    /// </summary>
    public class PlainFilePartitioner
        : IBinaryPartitioner, ITextPartitioner, IObjectPartitioner
    {
        /// <summary>
        /// Gets the relative path of the file in which log objects are written.
        /// </summary>
        public String File { get; private set; }

        /// <summary>
        /// Gets the encoding used for writing textual log objects.
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Providers.Desktop.Partitioners.PlainFilePartitioner"/> class with the specified relative path and optional text encoding.
        /// </summary>
        /// <param name="file">The relative path in which all log objects are partitioned.</param>
        /// <param name="encoding">optional; The encoding to use for writing textual log objects. Default (null) means UTF-8.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given relative file path is null.</exception>
        public PlainFilePartitioner(string file, Encoding encoding = null)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            this.File = file;
            this.Encoding = encoding ?? Encoding.UTF8;
        }

        #region Implementations

        /// <summary>
        /// Partitions the given log object with its binary representation.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        public virtual string Partition(LogObject o, byte[] b)
        {
            return this.File;
        }

        /// <summary>
        /// Partitions the given log object with its textual representation.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        public virtual string Partition(LogObject o, string s)
        {
            return this.File;
        }

        /// <summary>
        /// Partitions the given log object.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns>A <see cref="System.String"/> representing a relative file path where the object will be stored; null if not stored.</returns>
        public virtual string Partition(LogObject o)
        {
            return this.File;
        }

        #endregion
    }
}
