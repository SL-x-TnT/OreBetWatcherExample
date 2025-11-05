using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal sealed class Board : StateAccount
    {
        public ulong RoundId { get; set; }
        public ulong StartSlot { get; set; }
        public ulong EndSlot { get; set; }

        public bool WaitingForFirstBid => EndSlot == 0xFFFFFFFFFFFFFFFF;

        public static Board ReadFrom(ReadOnlySpan<byte> data)
        {
            int offset = 8; // skip first 8 bytes
            Board board = new Board();

            board.RoundId = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            board.StartSlot = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            board.EndSlot = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;


            return board;
        }
    }
}
