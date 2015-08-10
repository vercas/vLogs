using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vLogs.Utilities
{
    /// <summary>
    /// Defines a method to compare enumerations.
    /// </summary>
    /// <typeparam name="T">The type of the elements to compare.</typeparam>
    public class EnumerationComparer<T>
        : Comparer<IEnumerable<T>>
    {
        /// <summary>
        /// Returns a default sort order comparer for enumerations of the type specified by the generic argument.
        /// </summary>
        public static new EnumerationComparer<T> Default { get; private set; }

        static EnumerationComparer()
        {
            Default = new EnumerationComparer<T>(Comparer<T>.Default);
        }

        /// <summary>
        /// Gets the comparer used to compare elements of enumerations.
        /// </summary>
        public IComparer<T> ElementComparer { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether shorter enumerations are "less than" longer ones, when their common elements are identical.
        /// </summary>
        public Boolean ShorterGoesFirst { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether element order is reversed (relative to the result of the provided comparer).
        /// </summary>
        public Boolean ReverseElementsOrder { get; internal set; }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greated than the other.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            using (var enx = x.GetEnumerator())
            using (var eny = y.GetEnumerator())
            {
                int compRes = 0;

                do
                {
                    if (!enx.MoveNext())
                    {
                        if (eny.MoveNext())
                            return ShorterGoesFirst ? -1 : 1;
                        else
                            return 0;
                    }
                    else if (!eny.MoveNext())
                        return ShorterGoesFirst ? 1 : -1;

                    compRes = ElementComparer.Compare(enx.Current, eny.Current);

                    if (compRes != 0)
                        return ReverseElementsOrder ? -compRes : compRes;
                } while (true);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vLogs.Utilities.EnumerationComparer{T}"/> class with the specified individual element comparer.
        /// </summary>
        /// <param name="elementComparer">Comparer used to compare individual elements.</param>
        /// <param name="shorterGoesFirst"></param>
        /// <param name="reverseElementsOrder"></param>
        public EnumerationComparer(IComparer<T> elementComparer, bool shorterGoesFirst = true, bool reverseElementsOrder = false)
        {
            if (elementComparer == null)
                throw new ArgumentNullException("elementComparer");

            this.ElementComparer = elementComparer;
            this.ShorterGoesFirst = shorterGoesFirst;
            this.ReverseElementsOrder = reverseElementsOrder;
        }
    }
}
