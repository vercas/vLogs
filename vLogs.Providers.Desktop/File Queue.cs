using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vLogs.Loggers.Desktop
{
    /// <summary>
    /// Represents a queue of data for a specific file.
    /// </summary>
    internal class FileQueue<TData>
    {
        internal object _sync = new object();
        internal ConcurrentQueue<TData> queue = new ConcurrentQueue<TData>();
    }
}
