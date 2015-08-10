using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vLogs.Loggers.Desktop
{
    using Objects;

    public delegate string BinaryPartitionerDelegate(LogObject o, byte[] b);

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

        /// <summary>
        /// Gets the partitioner used by this logger.
        /// </summary>
        public BinaryPartitionerDelegate Partitioner { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Loggers.Desktop.PartitionedBinaryLogger"/> class with the specified enumeration of <see cref="vLogs.Loggers.Desktop.Partitioners.IBinaryPartitioner"/>s.
        /// </summary>
        /// <param name="partitioner">The <see cref="vLogs.Loggers.Desktop.BinaryPartitionerDelegate"/> used to partition log objects.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given partitioner delegate is null.</exception>
        public PartitionedBinaryLogger(BinaryPartitionerDelegate partitioner)
        {
            if (partitioner == null)
                throw new ArgumentNullException("partitioner");

            this.Partitioner = partitioner;
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
            var pth = this.Partitioner(o, b);

            if (pth != null)
            {
                Log(pth, b);

                return true;
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
}
