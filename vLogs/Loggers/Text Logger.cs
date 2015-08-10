using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Loggers
{
    using Objects;
    using Objects.KeyValues;

    using Utilities;

    /// <summary>
    /// Prepares log objects as text before logging.
    /// </summary>
    public abstract class TextLogger
        : IObjectLogger
    {
        /// <summary>
        /// Gets the string used to format the header (common information) of log objects.
        /// </summary>
        public String HeaderFormatString { get; private set; }

        /// <summary>
        /// Gets the helper container for indentation strings used by the text logger.
        /// </summary>
        public Indentation Indentation { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the source is excluded from the header or not, even if specified in the format string.
        /// </summary>
        public Boolean SourceExcluded { get; private set; }

        /// <summary>
        /// Initializes the <see cref="vLogs.Loggers.TextLogger"/> with the specified header format.
        /// </summary>
        /// <param name="headerFormat">optional; The string used to format the log object text representation header.</param>
        /// <param name="indent">optional; The container used as helper for indentation. Default value (null) means a tab character will be used per level (<see cref="vLogs.Utilities.Indentation.Tab"/>).</param>
        /// <param name="excludeSource">optional; True to exclude the source of an object from the header even if specified in the format; otherwise false.</param>
        public TextLogger(string headerFormat = "[{_TIMESTAMP_:yyyy.MM.dd HH:mm:ss.ff}|{_PRIORITY_,4}{_+FLAGS_}{_+SOURCES_}] ", Indentation indent = null, bool excludeSource = false)
        {
            if (headerFormat == null)
                throw new ArgumentNullException("headerFormat");

            if (indent == null)
                indent = Indentation.Tab;

            headerFormat = headerFormat.Replace("_TIMESTAMP_", "0").Replace("_PRIORITY_", "1").Replace("_+FLAGS_", "2").Replace("_+SOURCES_", "3");

            this.HeaderFormatString = headerFormat;
            this.Indentation = indent;
            this.SourceExcluded = excludeSource;
        }

        #region IObjectLogger Members

        /// <summary>
        /// Logs the specified object as string.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        public bool Log(LogObject o)
        {
            StringBuilder b = new StringBuilder(256);

            string flags = string.Empty;
            if (o.Flags != 0) flags = "|" + o.Flags.ToString();

            string sources = string.Empty;
            if (!this.SourceExcluded && o.Sources.Count > 0) sources = " @ " + string.Join("/", o.Sources);

            b.AppendFormat(this.HeaderFormatString, o.Timestamp, o.Priority, flags, sources);

            string indent = this.Indentation[o.Indent];
            string margin = Indentation.OneSpace[b.Length];

            for (int i = 0; i < o.Payloads.Count; i++)
            {
                if (i > 0)
                {
                    b.AppendLine();
                    b.Append(margin);
                }

                var pld = o.Payloads[i];
                
                switch (pld.GetType().GetPayloadType())
                {
                    case PayloadTypes.Message:
                        var pldm = (MessagePayload)pld;

                        b.AppendFormat("<{0,-11}> ", pldm.MessageType);

                        var extraMargin = Indentation.OneSpace[14]; //  Compensates for the text above.

                        var msplit = pldm.Message.Split('\n');

                        for (int j = 0; j < msplit.Length; j++)
                        {
                            if (j > 0)
                            {
                                b.Append(margin);
                                b.Append(extraMargin);
                            }

                            b.Append(indent);
                            b.AppendLine(msplit[j].Trim('\r'));
                        }

                        break;

                    case PayloadTypes.Exception:
                        var plde = (ExceptionPayload)pld;

                        for (int j = 0; j < plde.Exceptions.Count; j++)
                        {
                            if (j > 0)
                            {
                                b.AppendLine();
                                b.Append(margin);
                            }

                            var explit = plde.Exceptions[j].Split('\n');

                            for (int k = 0; k < explit.Length; k++)
                            {
                                if (k > 0)
                                    b.Append(margin);

                                b.Append(indent);
                                b.AppendLine(explit[k].Trim('\r'));
                            }
                        }

                        break;

                    case PayloadTypes.KeyValue:
                        var pldkv = (KeyValuePayload)pld;

                        _KVPs(b, pldkv.Collection, o.Indent, margin, string.Empty);

                        break;

                    default:
                        return false;
                }
            }

            return Log(o, b.ToString());
        }

        #endregion

        /// <summary>
        /// Logs the specified string representing a <see cref="vLogs.Objects.LogObject"/>.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        /// <returns>True if the object was logged; otherwise false.</returns>
        protected abstract bool Log(LogObject o, string s);

        #region Utilities

        private void _KVPs(StringBuilder b, IList<KeyValuePair> kvps, int ind, string margin, string extraMargin)
        {
            b.AppendFormat("({0} pairs)", kvps.Count);
            b.AppendLine();

            var inds = this.Indentation[ind];

            for (int i = 0; i < kvps.Count; i++)
            {
                var kvp = kvps[i];

                b.Append(margin);
                b.Append(inds);

                if (kvp.ValueType == KvpValueType.Collection)
                {
                    b.AppendFormat("\"{0}\": ", kvp.Key);

                    _KVPs(b, kvp.Collection, ind + 1, margin, Indentation.OneSpace[kvp.Key.Length + 4]);
                }
                else if (kvp.ValueType == KvpValueType.String)
                {
                    b.AppendFormat("\"{0}\" = \"", kvp.Key);

                    var extraExtraMargin = Indentation.OneSpace[kvp.Key.Length + 6];

                    var split = kvp.String.Split('\n');

                    for (int j = 0; j < split.Length; j++)
                    {
                        if (j > 0)
                        {
                            b.AppendLine();
                            b.Append(margin);
                            b.Append(inds);
                            b.Append(extraExtraMargin);
                        }

                        b.Append(split[j].Trim('\r'));
                    }

                    b.AppendLine("\"");
                }
                else
                {
                    string res;

                    if (kvp.ValueType == KvpValueType.Long)
                        res = kvp.Long.ToString();
                    else
                        res = kvp.Double.ToString();
                    
                    b.AppendFormat("\"{0}\" = {1}", kvp.Key, res);
                    b.AppendLine();
                }
            }
        }

        #endregion
    }
}
