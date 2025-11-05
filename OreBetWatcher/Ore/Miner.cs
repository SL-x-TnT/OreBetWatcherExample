using Solnet.Wallet;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal sealed class Miner : StateAccount
    {
        public PublicKey Authority { get; set; }       // 32 bytes
        public ulong[] Deployed { get; set; }           // 25 × 8 = 200 bytes
        public ulong[] Cumulative { get; set; }         // 25 × 8 = 200 bytes
        public ulong CheckpointFee { get; set; }
        public ulong CheckpointId { get; set; }
        public long LastClaimOreAt { get; set; }
        public long LastClaimSolAt { get; set; }
        public BigInteger RewardsFactor { get; set; }   // 16 bytes (u128)
        public ulong RewardsSol { get; set; }
        public ulong RewardsOre { get; set; }
        public ulong RefinedOre { get; set; }
        public ulong RoundId { get; set; }
        public ulong LifetimeRewardsSol { get; set; }
        public ulong LifetimeRewardsOre { get; set; }

        public ulong TotalDeployed => (ulong)Deployed.Select(x => (long)x).Sum();

        public static Miner ReadFrom(ReadOnlySpan<byte> data)
        {
            if (data[0] != 103)
            {
                return null;
            }

            int offset = 8; // skip first 8 bytes
            Miner miner = new Miner();

            // PublicKey (32 bytes)
            miner.Authority = new PublicKey(data.Slice(offset, 32).ToArray());
            offset += 32;

            miner.Deployed = new ulong[25];
            for (int i = 0; i < 25; i++)
            {
                miner.Deployed[i] = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8));
                offset += 8;
            }

            miner.Cumulative = new ulong[25];
            for (int i = 0; i < 25; i++)
            {
                miner.Cumulative[i] = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8));
                offset += 8;
            }

            miner.CheckpointFee = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.CheckpointId = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.LastClaimOreAt = BinaryPrimitives.ReadInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.LastClaimSolAt = BinaryPrimitives.ReadInt64LittleEndian(data.Slice(offset, 8)); offset += 8;

            // RewardsFactor (u128 = 16 bytes)
            miner.RewardsFactor = new BigInteger(data.Slice(offset, 16), isUnsigned: true, isBigEndian: false);
            offset += 16;

            miner.RewardsSol = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.RewardsOre = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.RefinedOre = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.RoundId = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.LifetimeRewardsSol = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;
            miner.LifetimeRewardsOre = BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, 8)); offset += 8;

            return miner;
        }
    }
}
