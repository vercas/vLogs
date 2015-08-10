using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using vLogs;
using vLogs.Objects;
using vLogs.Objects.KeyValues;

using vLogs.Utilities;

namespace vLogs_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("(W)rite or (R)ead? ");

            ConsoleKey c;

            do
            {
                c = Console.ReadKey(true).Key;

                switch (c)
                {
                    case ConsoleKey.W:
                    case ConsoleKey.R:
                        break;

                    default: continue;
                }
            } while (false);

            Console.WriteLine();

            if (c == ConsoleKey.W)
            {
                Exception testEx = new Exception("bleh");

                try
                {
                    long rem, res;
                    res = Math.DivRem(616, 0, out rem);

                    Console.WriteLine("Division results: c = {0}, r = {1}", res, rem);
                }
                catch (Exception x)
                {
                    testEx = x;
                }

                var mgr = new LogManager();
                mgr.AddLogger(new TestLogger1());
                mgr.AddLogger(new TestLogger2());

                mgr.LogMessage("iiiiinfooooo", new string[] { "a", "b", "c" }, MessageType.Information, -10);
                mgr.LogMessage("waaarniiiing", new string[] { "a", "b", "c" }, MessageType.Warning, 10, 2);
                mgr.LogMessage("eeeerrooooor", new string[] { "a", "b", "c" }, MessageType.Error, 30, 4);

                mgr.LogException(testEx, new string[] { "exc", "ept", "ion" }, 127, 0, LogFlags.Terminal);

                mgr.WithObject(new string[] { "fluent", "object", "test" }, flags: LogFlags.Debug)
                    .WithMessage("Fluently created, modified and logged object test.", MessageType.Warning)
                    .WithException(testEx)
                    .WithExtraPayload(new KeyValuePayload(new KeyValueCollection(true,
                        new KeyValuePair("a", "b"),
                        new KeyValuePair("c", "d"),
                        new KeyValuePair("e", "f"),
                        new KeyValuePair("g", "h"),
                        new KeyValuePair("EXTRA",
                            new KeyValuePair("A", "B"),
                            new KeyValuePair("C", "D"),
                            new KeyValuePair("E", "F"),
                            new KeyValuePair("G", "H"),
                            new KeyValuePair("I", "J"),
                            new KeyValuePair("MOAAAR",
                                new KeyValuePair("1", "2"),
                                new KeyValuePair("3", "4"),
                                new KeyValuePair("5", "6"),
                                new KeyValuePair("7", "8"),
                                new KeyValuePair("9", "10"),
                                new KeyValuePair("11", "12"),
                                new KeyValuePair("13", "14"),
                                new KeyValuePair("15", "16"),
                                new KeyValuePair("17", "18")
                            ),
                            new KeyValuePair("blah", "nitzzle"),
                            new KeyValuePair("yada", "drizzle")
                        ),
                        new KeyValuePair("meh", "shizzle"),
                        new KeyValuePair("line split", string.Format("a{0}b{0}c{0}d", Environment.NewLine))
                    )))
                    .Log();
            }
            else
            {
                var tl1 = new TestLogger1();

                try
                {
                    using (var fs = new System.IO.FileStream("binary.log", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    using (var b = new System.IO.BinaryReader(fs))
                        foreach (var o in vLogs.Utilities.Binary.Read(b))
                            tl1.Log(o);
                }
                catch (Exception x)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;

                    Console.WriteLine(x.ToString());
                }
            }

            Console.ReadLine();
        }
    }

    class TestLogger1
        : vLogs.Loggers.TextLogger
    {
        protected override bool Log(vLogs.Objects.LogObject o, string s)
        {
            Console.Write(s);

            return true;
        }
    }

    class TestLogger2
        : vLogs.Loggers.BinaryLogger
    {
        protected override bool Log(LogObject o, byte[] b)
        {
            try
            {
                using (var fs = new System.IO.FileStream("binary.log", System.IO.FileMode.Append))
                    fs.Write(b, 0, b.Length);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
