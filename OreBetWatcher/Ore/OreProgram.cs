using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal class OreProgram
    {
        public static readonly PublicKey Id = new PublicKey("oreV3EG1i9BEgiAJ8b177Z2S2rMarzak4NMv1kULvWv");

        public static readonly PublicKey Board = DeriveBoard();

        public static PublicKey DeriveBoard()
        {
            PublicKey.TryFindProgramAddress(new List<byte[]> { Encoding.UTF8.GetBytes("board")}, Id, out PublicKey address, out byte _);

            return address;
        }

        public static PublicKey DeriveTreasury()
        {
            PublicKey.TryFindProgramAddress(new List<byte[]> { Encoding.UTF8.GetBytes("treasury") }, Id, out PublicKey address, out byte _);

            return address;
        }

        public static PublicKey DeriveAutomation(PublicKey authority)
        {
            PublicKey.TryFindProgramAddress(new List<byte[]> { Encoding.UTF8.GetBytes("automation"), authority.KeyBytes }, Id, out PublicKey address, out byte _);

            return address;
        }

        public static PublicKey DeriveMiner(PublicKey authority)
        {
            PublicKey.TryFindProgramAddress(new List<byte[]> { Encoding.UTF8.GetBytes("miner"), authority.KeyBytes }, Id, out PublicKey address, out byte _);

            return address;
        }

        public static PublicKey DeriveRound(ulong roundId)
        {
            Span<byte> round = stackalloc byte[8];

            BinaryPrimitives.WriteUInt64LittleEndian(round, roundId);

            PublicKey.TryFindProgramAddress(new List<byte[]> { Encoding.UTF8.GetBytes("round"), round.ToArray() }, Id, out PublicKey address, out byte _);

            return address;
        }
    }
}
