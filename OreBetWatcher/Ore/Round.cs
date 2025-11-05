
using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal sealed class Round : StateAccount
    {

        public ulong Id { get; init; }

        public ulong[] Deployed { get; init; } = new ulong[25];

        public byte[] SlotHash { get; init; } = new byte[32];

        public ulong[] Count { get; init; } = new ulong[25];

        public ulong ExpiresAt { get; init; }

        public ulong Motherlode { get; init; }

        public PublicKey RentPayer { get; init; }

        public PublicKey TopMiner { get; init; }

        public ulong TopMinerReward { get; init; }

        public ulong TotalDeployed { get; init; }

        public ulong TotalVaulted { get; init; }

        public ulong TotalWinnings { get; init; }

        public ulong CurrentMotherload { get; set; }

        public ulong CalculateWinningsForSquare(int square)
        {
            ulong totalDeployed = Deployed.Sum();
            ulong fullWinnings = (totalDeployed - Deployed[square]);
            ulong fees = fullWinnings * 109 / 1000;

            return fullWinnings - fees;
        }

        public static Round ReadFrom(ReadOnlySpan<byte> data)
        {
            const int ExpectedSize = 560;           // 8‑byte discriminator + 552‑byte struct
            if (data.Length < ExpectedSize)
                throw new ArgumentException(
                    $"Data slice is too short. Expected at least {ExpectedSize} bytes, got {data.Length}.",
                    nameof(data));

            // -------------------------------------------------------------
            // 1️⃣  Skip the first 8 bytes (Solana discriminator)
            // -------------------------------------------------------------
            ReadOnlySpan<byte> span = data.Slice(8);
            int offset = 0;                         // moving cursor inside `span`

            // -------------------------------------------------------------
            // 2️⃣  Read fields in the order they appear in the Rust struct
            // -------------------------------------------------------------
            ulong id = span.ReadU64(ref offset);

            ulong[] deployed = span.ReadU64Array(ref offset, 25);

            byte[] slotHash = span.ReadByteArray(ref offset, 32);

            ulong[] count = span.ReadU64Array(ref offset, 25);

            ulong expiresAt = span.ReadU64(ref offset);

            ulong motherlode = span.ReadU64(ref offset);

            // PublicKey values are just 32‑byte raw ed25519 public keys.
            // Most Solana SDKs expose a constructor that takes a byte[]/ReadOnlySpan.
            PublicKey rentPayer = new PublicKey(span.ReadByteArray(ref offset, 32));
            PublicKey topMiner = new PublicKey(span.ReadByteArray(ref offset, 32));

            ulong topMinerReward = span.ReadU64(ref offset);
            ulong totalDeployed = span.ReadU64(ref offset);
            ulong totalVaulted = span.ReadU64(ref offset);
            ulong totalWinnings = span.ReadU64(ref offset);

            // -------------------------------------------------------------
            // 3️⃣  Build the immutable model
            // -------------------------------------------------------------
            return new Round
            {
                Id = id,
                Deployed = deployed,
                SlotHash = slotHash,
                Count = count,
                ExpiresAt = expiresAt,
                Motherlode = motherlode,
                RentPayer = rentPayer,
                TopMiner = topMiner,
                TopMinerReward = topMinerReward,
                TotalDeployed = totalDeployed,
                TotalVaulted = totalVaulted,
                TotalWinnings = totalWinnings
            };
        }

        private ulong RNG()
        {
            var s = SlotHash.AsSpan();

            var r1 = BinaryPrimitives.ReadUInt64LittleEndian(s);
            var r2 = BinaryPrimitives.ReadUInt64LittleEndian(s.Slice(8));
            var r3 = BinaryPrimitives.ReadUInt64LittleEndian(s.Slice(16));
            var r4 = BinaryPrimitives.ReadUInt64LittleEndian(s.Slice(24));

            return r1 ^ r2 ^ r3 ^ r4;
        }

        public int WinningSquare()
        {
            return (int)(RNG() % 25);
        }

        public ulong WinningMinerSample()
        {

            return GetMinerSampleRNG() % Deployed[WinningSquare()];
        }


        public ulong GetMinerSampleRNG()
        {
            return ReverseBits(RNG());
        }

        private ulong ReverseBits(ulong value)
        {
            value = ((value >> 1) & 0x5555555555555555UL) | ((value & 0x5555555555555555UL) << 1);
            value = ((value >> 2) & 0x3333333333333333UL) | ((value & 0x3333333333333333UL) << 2);
            value = ((value >> 4) & 0x0F0F0F0F0F0F0F0FUL) | ((value & 0x0F0F0F0F0F0F0F0FUL) << 4);
            value = ((value >> 8) & 0x00FF00FF00FF00FFUL) | ((value & 0x00FF00FF00FF00FFUL) << 8);
            value = ((value >> 16) & 0x0000FFFF0000FFFFUL) | ((value & 0x0000FFFF0000FFFFUL) << 16);
            value = (value >> 32) | (value << 32);
            return value;
        }

        public override string ToString()
        {
            return
                $"Round #{Id}\n" +
                $"  Deployed (total): {TotalDeployed / Consts.SolDecimals}\n" +
                $"  Total Vaulted: {TotalVaulted / Consts.SolDecimals}\n" +
                $"  Total Winnings: {TotalWinnings / Consts.SolDecimals}\n" +
                $"  ExpiresAt slot: {ExpiresAt}\n" +
                $"  Motherlode: {Motherlode / Consts.OreDecimals}\n" +
                $"  Winning Block: {WinningSquare() + 1}\n" +
                $"  Top Miner: {TopMiner} (+{TopMinerReward / Consts.OreDecimals} ORE)\n" +
                $"  Rent Payer: {RentPayer}\n" +
                $"  SlotHash: {Convert.ToHexString(SlotHash).ToLowerInvariant()}\n" +
                $"  Deployed per square: [{string.Join(", ", Deployed.Select(x => x / Consts.SolDecimals))}]\n" +
                $"  Miner count per square: [{string.Join(", ", Count)}]";
        }
    }
}
