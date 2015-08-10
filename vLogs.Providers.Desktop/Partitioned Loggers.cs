using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vLogs.Loggers.Desktop
{
    using Partitioners;

    /// <summary>
    /// A logger which uses multiple partitioners to log objects in binary representation.
    /// </summary>
    public class PartitionedBinaryLogger
        : BinaryLogger
    {
        #region Constants

        /// <summary>
        /// The delay (in milliseconds) to wait before starting to flush data to the file - letting multiple objects accumulate.
        /// </summary>
        public const int FlushDelay = 25;

        /// <summary>
        /// The <see cref="System.IO.FileMode"/> used for opening the file.
        /// </summary>
        public const FileMode FileMode = System.IO.FileMode.Append;
        /// <summary>
        /// The <see cref="System.IO.FileAccess"/> used for opening the file.
        /// </summary>
        public const FileAccess FileAccess = System.IO.FileAccess.Write;
        /// <summary>
        /// The <see cref="System.IO.FileShare"/> used for opening the file.
        /// </summary>
        public const FileShare FileShare = System.IO.FileShare.Read;

        #endregion

        private IBinaryPartitioner[] _parts;

        /// <summary>
        /// Gets the partitioner used by this logger.
        /// </summary>
        public ReadOnlyCollection<IBinaryPartitioner> Partitioners { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Loggers.Desktop.PartitionedBinaryLogger"/> class with the specified enumeration of <see cref="vLogs.Loggers.Desktop.Partitioners.IBinaryPartitioner"/>s.
        /// </summary>
        /// <param name="parts"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given partitioners enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given partitioners enumeration contains a null item.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given partitioners enumeration contains no items.</exception>
        public PartitionedBinaryLogger(IEnumerable<IBinaryPartitioner> parts)
        {
            if (parts == null)
                throw new ArgumentNullException("part");

            _parts = parts.ToArray();

            if (_parts.Length == 0)
                throw new ArgumentOutOfRangeException("parts", "The enumeration should contain at least one partitioner.");

            for (int i = 0; i < _parts.Length; i++)
                if (_parts[i] == null)
                    throw new ArgumentException(string.Format("Partition at index {0} in the enumeration is null.", i), "parts");

            this.Partitioners = new ReadOnlyCollection<IBinaryPartitioner>(_parts);
            this.FlushDelegate = new WaitCallback(Flush);
        }

        /// <summary>
        /// Logs the given log object with the partitioners.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected override bool Log(Objects.LogObject o, byte[] b)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                var pth = _parts[i].Partition(o, b);
                
                if (pth != null)
                {
                    Log(pth, b);

                    return true;
                }
            }

            return false;
        }

        #region Flushing

        private WaitCallback FlushDelegate;
        private int _dumping = 0;
        //static ConcurrentQueue<string>[] datas = new ConcurrentQueue<string>[files.Length];
        ConcurrentDictionary<string, FileQueue<byte[]>> datas = new ConcurrentDictionary<string, FileQueue<byte[]>>();

        protected void Log(string fn, byte[] data)
        {
            datas.GetOrAdd(fn, _ => new FileQueue<byte[]>()).queue.Enqueue(data);

            Dump();
        }

        private void Dump()
        {
            /*bool go = false;

            lock (_sync)
                if (!_dumping)
                    go = _dumping = true;*/

            if (Interlocked.CompareExchange(ref _dumping, -1, 0) == 0)
                ThreadPool.QueueUserWorkItem(FlushDelegate);

            /* Here's the deal. The oddity above basically means this:
             * 
             * if (_dumping == 0)
             * {
             *     _dumping = -1;
             *     return true;
             * }
             * else
             *     return false;
             *     
             * Except it is atomic.
             */
        }

        void Flush(object state)
        {
            Thread.Sleep(FlushDelay);

            Interlocked.Exchange(ref _dumping, 0);

            Parallel.ForEach(datas, kv =>
            {
                var data = kv.Value;

                if (data.queue.Count > 0)
                    using (var queue = new MemoryStream(64 * 1024))
                    {
                        bool ok = false;
                        do
                        {
                            byte[] bytes = null;
                            ok = data.queue.TryDequeue(out bytes);

                            if (ok)
                                queue.Write(bytes, 0, bytes.Length);
                        } while (ok);

                        lock (data._sync)
                        {

                        }
                    }
            });
        }

        #endregion
    }

    /// <summary>
    /// A logger which uses multiple partitioners to log objects in textual representation.
    /// </summary>
    public class PartitionedTextLogger
        : TextLogger
    {
        private ITextPartitioner[] _parts;

        /// <summary>
        /// Gets the partitioner used by this logger.
        /// </summary>
        public ReadOnlyCollection<ITextPartitioner> Partitioners { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Loggers.Desktop.PartitionedTextLogger"/> class with the specified enumeration of <see cref="vLogs.Loggers.Desktop.Partitioners.ITextPartitioner"/>s.
        /// </summary>
        /// <param name="parts"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given partitioners enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given partitioners enumeration contains a null item.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given partitioners enumeration contains no items.</exception>
        public PartitionedTextLogger(IEnumerable<ITextPartitioner> parts)
        {
            if (parts == null)
                throw new ArgumentNullException("part");

            _parts = parts.ToArray();

            if (_parts.Length == 0)
                throw new ArgumentOutOfRangeException("parts", "The enumeration should contain at least one partitioner.");

            for (int i = 0; i < _parts.Length; i++)
                if (_parts[i] == null)
                    throw new ArgumentException(string.Format("Partition at index {0} in the enumeration is null.", i), "parts");

            this.Partitioners = new ReadOnlyCollection<ITextPartitioner>(_parts);
        }

        /// <summary>
        /// Logs the given log object with the partitioners.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        protected override bool Log(Objects.LogObject o, string s)
        {
            bool res = false;

            for (int i = 0; i < _parts.Length; i++)
                res |= _parts[i].Partition(o, s);

            return res;
        }
    }

    /// <summary>
    /// A logger which uses multiple partitioners to log raw objects.
    /// </summary>
    public class PartitionedObjectLogger
        : IObjectLogger
    {
        private IObjectPartitioner[] _parts;

        /// <summary>
        /// Gets the partitioner used by this logger.
        /// </summary>
        public ReadOnlyCollection<IObjectPartitioner> Partitioners { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Loggers.Desktop.PartitionedTextLogger"/> class with the specified enumeration of <see cref="vLogs.Loggers.Desktop.Partitioners.IObjectPartitioner"/>s.
        /// </summary>
        /// <param name="parts"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given partitioners enumeration is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given partitioners enumeration contains a null item.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given partitioners enumeration contains no items.</exception>
        public PartitionedObjectLogger(IEnumerable<IObjectPartitioner> parts)
        {
            if (parts == null)
                throw new ArgumentNullException("part");

            _parts = parts.ToArray();

            if (_parts.Length == 0)
                throw new ArgumentOutOfRangeException("parts", "The enumeration should contain at least one partitioner.");

            for (int i = 0; i < _parts.Length; i++)
                if (_parts[i] == null)
                    throw new ArgumentException(string.Format("Partition at index {0} in the enumeration is null.", i), "parts");

            this.Partitioners = new ReadOnlyCollection<IObjectPartitioner>(_parts);
        }

        /// <summary>
        /// Logs the given log object with the partitioners.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        protected override bool Log(Objects.LogObject o)
        {
            bool res = false;

            for (int i = 0; i < _parts.Length; i++)
                res |= _parts[i].Partition(o);

            return res;
        }
    }
}
