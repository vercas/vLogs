using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Utilities
{
    using Objects;
    using Objects.KeyValues;

    /// <summary>
    /// Contains utilitary methods for writing and reading binary values.
    /// </summary>
    public static class Binary
    {
        const int k2to30 = 1 << 30;
        const int k2to22 = 1 << 22;
        const int k2to14 = 1 << 14;
        const int k2to6 = 1 << 6;

        /// <summary>
        /// True if this computer architecture stores bytes in little endian order; otherwise false.
        /// </summary>
        public static readonly bool IsLittleEndian;

        static Binary()
        {
            IsLittleEndian = BitConverter.IsLittleEndian && BitConverter.IsLittleEndian;
        }

        #region Economy

        /// <summary>
        /// Writes the given <see cref="System.UInt32"/> to the binary writer, using the fewest bytes possible, in network byte order.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the target binary writer is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given value is greater than or equal to the 30th power of 2.</exception>
        public static void WriteSpaceEfficientUInt32(this BinaryWriter target, uint value)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (value >= k2to30)
                throw new ArgumentOutOfRangeException("value", "Must be less than the 30th power of 2.");

            var bytes = BitConverter.GetBytes(value);

            if (IsLittleEndian)
                Array.Reverse(bytes);

            if (value < k2to6)
            {
                target.Write(bytes[3]);
            }
            else if (value < k2to14)
            {
                bytes[2] |= 0x40;

                target.Write(bytes, 2, 2);
            }
            else if (value < k2to22)
            {
                bytes[1] |= 0x80;

                target.Write(bytes, 1, 3);
            }
            else
            {
                bytes[0] |= 0xC0;

                target.Write(bytes, 0, 4);
            }
        }

        /// <summary>
        /// Reads a <see cref="System.UInt32"/> from the binary reader, stored with the fewest bytes possible, in network byte order.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Null if end of stream; otherwise a valid value.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the source binary reader is null.</exception>
        public static uint? ReadSpaceEfficientUInt32(this BinaryReader source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var bytes = new byte[4] { 0, 0, 0, 0 };

            try
            {
                bytes[0] = source.ReadByte();
            }
            catch
            {
                return null;
            }

            int extra = (bytes[0] & 0xC0) >> 6;

            bytes[0] &= 0x3F;

            switch (extra)
            {
                case 3:
                    bytes[1] = source.ReadByte();
                    bytes[2] = source.ReadByte();
                    bytes[3] = source.ReadByte();

                    break;

                case 2:
                    bytes[2] = source.ReadByte();
                    bytes[3] = source.ReadByte();

                    bytes[1] = bytes[0];
                    bytes[0] = 0;

                    break;

                case 1:
                    bytes[3] = source.ReadByte();

                    bytes[2] = bytes[0];
                    bytes[0] = 0;
                    
                    break;

                default:
                    bytes[3] = bytes[0];
                    bytes[0] = 0;

                    break;
            }   //  Why did I choose big endian? -_-

            if (IsLittleEndian)    //  Curse you, CPU architecture designers.
                Array.Reverse(bytes);

            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Writes the given string to the biniary writer in a space-efficient way.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="str"></param>
        /// <param name="encoding">optional; The encoding to use for writing text. Default (null) means UTF-8.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the target binary writer or given string is null.</exception>
        public static void WriteSpaceEfficientString(this BinaryWriter target, string str, Encoding encoding = null)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (str == null)
                throw new ArgumentNullException("str");

            if (encoding == null)
                encoding = Encoding.UTF8;

            var strBytes = encoding.GetBytes(str);

            WriteSpaceEfficientUInt32(target, (uint)strBytes.Length);
            target.Write(strBytes);
        }

        /// <summary>
        /// Reads a string from the binary reader which was written in a space-efficient way.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding">optional; The encoding to use for writing text. Default (null) means UTF-8.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the source binary reader is null.</exception>
        public static string ReadSpaceEfficientString(this BinaryReader source, Encoding encoding = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (encoding == null)
                encoding = Encoding.UTF8;

            uint? len = ReadSpaceEfficientUInt32(source);

            if (len.HasValue)
            {
                var strBytes = source.ReadBytes((int)len.Value);

                return encoding.GetString(strBytes, 0, strBytes.Length);
            }
            else
                return null;
        }

        #endregion

        /*
        /// <summary>
        /// Writes the given <see cref="System.Exception"/> to the binary writer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="x"></param>
        /// <param name="encoding">optional; The encoding to use for serializing text.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the target binary writer or given exception is null.</exception>
        public static void WriteException(this BinaryWriter target, Exception x, Encoding encoding = null)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (x == null)
                throw new ArgumentNullException("x");

            if (encoding == null)
                encoding = Encoding.UTF8;

            var strBytes = encoding.GetBytes(x.ToString());

            target.WriteSpaceEfficientUInt32((uint)strBytes.Length);

            if (x.InnerException == null)
                target.Write((byte)0x00);
            else
                target.Write((byte)0xFF);

            target.Write(strBytes);

            if (x.InnerException != null)
                WriteException(target, x.InnerException, encoding);
        }//*/

        /// <summary>
        /// Writes the given list of strings to the binary writer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="str"></param>
        /// <param name="encoding">optional; The encoding to use for writing text. Default (null) means UTF-8.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the target binary writer or given list of strings is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given list of strings contains a null item.</exception>
        public static void WriteStrings(this BinaryWriter target, IList<string> str, Encoding encoding = null)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (str == null)
                throw new ArgumentNullException("str");

            if (encoding == null)
                encoding = Encoding.UTF8;

            WriteSpaceEfficientUInt32(target, (uint)str.Count);

            for (int i = 0; i < str.Count; i++)
            {
                if (str[i] == null)
                    throw new ArgumentException(string.Format("The given list of strings contains a null string at position {0}.", i), "str");

                WriteSpaceEfficientString(target, str[i], encoding);
            }
        }

        /// <summary>
        /// Reads a sequence of strings.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding">optional; The encoding to use for writing text. Default (null) means UTF-8.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the source binary reader is null.</exception>
        public static string[] ReadStrings(this BinaryReader source, Encoding encoding = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (encoding == null)
                encoding = Encoding.UTF8;

            uint? cnt = ReadSpaceEfficientUInt32(source);

            if (cnt.HasValue)
            {
                var res = new string[cnt.Value];

                for (int i = 0; i < cnt; i++)
                    res[i] = ReadSpaceEfficientString(source, encoding);

                return res;
            }
            else
                return null;
        }

        /// <summary>
        /// Writes the given list of key/value pairs to the binary writer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="kvps"></param>
        /// <param name="encoding">optional; The encoding to use for writing text. Default (null) means UTF-8.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the target binary writer or given key/value pair list is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the given key/value pair list contains a null item -or- an unknown/unsupported value type is encountered on a pair.</exception>
        public static void WriteKeyValues(this BinaryWriter target, IList<KeyValuePair> kvps, Encoding encoding = null)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (kvps == null)
                throw new ArgumentNullException("kvps");

            if (encoding == null)
                encoding = Encoding.UTF8;

            WriteSpaceEfficientUInt32(target, (uint)kvps.Count);

            for (int i = 0; i < kvps.Count; i++)
            {
                var kvp = kvps[i];

                if (kvp == null)
                    throw new ArgumentException(string.Format("Key/value pair at index {0} in the list is null.", i), "kvps");

                WriteSpaceEfficientString(target, kvp.Key, encoding);

                target.Write((byte)kvp.ValueType);

                switch (kvp.ValueType)
                {
                    case KvpValueType.String:
                        WriteSpaceEfficientString(target, kvp.String, encoding);
                        break;

                    case KvpValueType.Long:
                        target.Write(kvp.Long);
                        break;

                    case KvpValueType.Double:
                        target.Write(kvp.Double);
                        break;

                    case KvpValueType.Collection:
                        WriteKeyValues(target, kvp.Collection, encoding);
                        break;

                    default:
                        throw new ArgumentException("A key/value pair has an unknown value type.", "kvps");
                }
            }
        }

        /// <summary>
        /// Reads a collection of key/value pairs from the binary reader.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding">optional; The encoding to use for reading text. Default (null) means UTF-8.</param>
        /// <param name="lockIt">optional; True to lock the collection before returning; otherwise false. Also affects sub-collections.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the source binary reader is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an unknown/unsupported value type is encountered on a pair.</exception>
        public static KeyValueCollection ReadKeyValues(this BinaryReader source, Encoding encoding = null, bool lockIt = true)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (encoding == null)
                encoding = Encoding.UTF8;

            uint? cnt = ReadSpaceEfficientUInt32(source);

            if (cnt.HasValue)
            {
                var res = new KeyValuePair[cnt.Value];

                for (int i = 0; i < cnt; i++)
                {
                    var key = ReadSpaceEfficientString(source, encoding);

                    var type = (KvpValueType)source.ReadByte();

                    switch (type)
                    {
                        case KvpValueType.String:
                            res[i] = new KeyValuePair(key, ReadSpaceEfficientString(source, encoding));
                            break;

                        case KvpValueType.Long:
                            res[i] = new KeyValuePair(key, source.ReadInt64());
                            break;

                        case KvpValueType.Double:
                            res[i] = new KeyValuePair(key, source.ReadDouble());
                            break;

                        case KvpValueType.Collection:
                            res[i] = new KeyValuePair(key, ReadKeyValues(source, encoding, lockIt));
                            break;

                        default:
                            throw new ArgumentException(string.Format("A key/value pair has an unknown value type: {0} ({1}).", type, (byte)type));
                    }
                }

                return new KeyValueCollection(lockIt, res);
            }
            else
                return null;
        }

        /// <summary>
        /// Reads a sequence of binary log objects from a source stream.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="encoding">optional; The encoding to use for reading text. Default (null) means UTF-8.</param>
        /// <returns>An enumeration of <see cref="vLogs.Objects.LogObject"/>s.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the source binary reader is null.</exception>
        public static IEnumerable<LogObject> Read(this BinaryReader source, Encoding encoding = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (encoding == null)
                encoding = Encoding.UTF8;

            while (true)
            {
                var sources = source.ReadStrings(encoding);

                if (sources == null)
                    yield break;

                var timestamp = new DateTime(source.ReadInt64());
                var priority = source.ReadSByte();
                var flags = (LogFlags)source.ReadByte();
                var indent = source.ReadByte();

                var payloadTypes = (PayloadTypes)source.ReadByte();

                var payloads = new List<IPayload>(3);

                if ((payloadTypes & PayloadTypes.Message) != 0)
                {
                    var msgType = (MessageType)source.ReadByte();
                    var msgString = ReadSpaceEfficientString(source, encoding);

                    payloads.Add(new MessagePayload(msgType, msgString));
                }

                if ((payloadTypes & PayloadTypes.Exception) != 0)
                {
                    payloads.Add(new ExceptionPayload(ReadStrings(source, encoding)));
                }

                if ((payloadTypes & PayloadTypes.KeyValue) != 0)
                {
                    payloads.Add(new KeyValuePayload(ReadKeyValues(source, encoding, true)));
                }

                yield return new LogObject(sources, timestamp, flags, priority, indent, payloads);
            }
        }
    }
}
