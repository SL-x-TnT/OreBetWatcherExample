using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    /// <summary>
    /// LINQ‑style extensions for <c>ulong</c> sequences.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the sum of all values in the sequence.
        /// The operation is performed in a <c>checked</c> context – 
        /// an <see cref="OverflowException"/> is thrown if the total exceeds <c>ulong.MaxValue</c>.
        /// </summary>
        /// <param name="source">A sequence of <c>ulong</c> values.</param>
        /// <returns>The arithmetic sum of the elements.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="OverflowException">If the sum exceeds <c>ulong.MaxValue</c>.</exception>
        public static ulong Sum(this IEnumerable<ulong> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            ulong sum = 0;
            checked   // overflow will raise OverflowException
            {
                foreach (ulong v in source)
                {
                    sum += v;
                }
            }
            return sum;
        }

        /// <summary>
        /// Returns the sum of all non‑null values in the sequence.
        /// Mirrors the behaviour of the built‑in <c>Sum(this IEnumerable&lt;Nullable&lt;T&gt;&gt;)</c> overloads.
        /// </summary>
        /// <param name="source">A sequence of nullable <c>ulong</c> values.</param>
        /// <returns>The arithmetic sum of the non‑null elements.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="source"/> is <c>null</c>.</exception>
        /// <exception cref="OverflowException">If the sum exceeds <c>ulong.MaxValue</c>.</exception>
        public static ulong Sum(this IEnumerable<ulong?> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            ulong sum = 0;
            checked
            {
                foreach (ulong? v in source)
                {
                    if (v.HasValue)
                        sum += v.Value;
                }
            }
            return sum;
        }

        // -----------------------------------------------------------------
        // OPTIONAL: a “big‑integer” version that never overflows.
        // -----------------------------------------------------------------
        /// <summary>
        /// Returns the sum as a <see cref="BigInteger"/> – useful when you expect
        /// the total to be larger than <c>ulong.MaxValue</c>.
        /// </summary>
        public static BigInteger SumBig(this IEnumerable<ulong> source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            BigInteger sum = BigInteger.Zero;
            foreach (ulong v in source)
                sum += v;

            return sum;
        }
    }
}
