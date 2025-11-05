using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a little‑endian <c>ulong</c> from <paramref name="span"/>
        /// starting at <paramref name="offset"/> and returns the value.
        /// The offset is advanced by 8 bytes.
        /// </summary>
        public static ulong ReadU64(this ReadOnlySpan<byte> span, ref int offset)
        {
            if (offset + 8 > span.Length)
                throw new ArgumentException("Not enough bytes left in span to read UInt64.");

            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(offset, 8));
            offset += 8;
            return value;
        }

        /// <summary>
        /// Reads a fixed‑size byte array from <paramref name="span"/>.
        /// Returns a *new* array that contains the copied data.
        /// </summary>
        public static byte[] ReadByteArray(this ReadOnlySpan<byte> span, ref int offset, int length)
        {
            if (offset + length > span.Length)
                throw new ArgumentException($"Not enough bytes left in span to read {length} bytes.");

            byte[] result = span.Slice(offset, length).ToArray(); // copy
            offset += length;
            return result;
        }

        /// <summary>
        /// Reads an array of <c>ulong</c> values of length <paramref name="count"/>.
        /// </summary>
        public static ulong[] ReadU64Array(this ReadOnlySpan<byte> span, ref int offset, int count)
        {
            var arr = new ulong[count];
            for (int i = 0; i < count; i++)
                arr[i] = span.ReadU64(ref offset);
            return arr;
        }
    }
}
