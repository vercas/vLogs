using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Utilities
{
    /// <summary>
    /// Represents a container for indentation strings.
    /// </summary>
    public sealed class Indentation
    {
        static Indentation _tab = new Indentation("\t", 256);
        static Indentation _1space = new Indentation(" ", 256);
        static Indentation _2spaces = new Indentation("  ", 256);
        static Indentation _4spaces = new Indentation("    ", 256);

        /// <summary>
        /// Gets an indentation helper which uses a tabulator per level.
        /// </summary>
        public static Indentation Tab { get { return _tab; } }

        /// <summary>
        /// Gets an indentation helper which uses one space per level.
        /// </summary>
        public static Indentation OneSpace { get { return _1space; } }

        /// <summary>
        /// Gets an indentation helper which uses two spaces per level.
        /// </summary>
        public static Indentation TwoSpaces { get { return _2spaces; } }

        /// <summary>
        /// Gets an indentation helper which uses four spaces per level.
        /// </summary>
        public static Indentation FourSpaces { get { return _4spaces; } }

        /// <summary>
        /// Gets the sequence used for indentation.
        /// </summary>
        public String Indenter { get; private set; }

        private string[] inds;

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Utilities.Indentation"/> class.
        /// </summary>
        /// <param name="indenter">The sequence used to load indentation strings.</param>
        /// <param name="capacity">The number of indentation strings to preload.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given indenter sequence is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given capacity is negative.</exception>
        public Indentation(string indenter, int capacity)
        {
            if (indenter == null)
                throw new ArgumentNullException("indenter");

            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity", "Given capacity must be positive.");

            inds = new string[capacity];

            inds[0] = string.Empty;

            for (int i = 1; i < capacity; i++)
                inds[i] = inds[i - 1] + indenter;

            this.Indenter = indenter;
        }

        /// <summary>
        /// Gets a string representing the given amount of indentation levels.
        /// </summary>
        /// <remarks>
        /// If the level requested is not preloaded, every level up to and including the requested one is loaded.
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given amount is negative.</exception>
        public string this[int level]
        {
            get
            {
                return GetString(level);
            }
        }

        /// <summary>
        /// Gets a string representing the given amount of indentation levels.
        /// </summary>
        /// <remarks>
        /// If the level requested is not preloaded, every level up to and including the requested one is loaded.
        /// </remarks>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given level is negative.</exception>
        public string GetString(int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException("level", "Given level must be positive.");

            if (level >= this.inds.Length)
            {
                var oldInds = this.inds;
                this.inds = new string[level + 1];

                Array.Copy(oldInds, this.inds, oldInds.Length);

                for (int i = oldInds.Length; i <= level; i++)
                    this.inds[i] = this.inds[i - 1] + Indenter;
            }

            return this.inds[level];
        }
    }
}
