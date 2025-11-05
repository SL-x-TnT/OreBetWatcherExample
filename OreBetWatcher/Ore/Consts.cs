using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OreBetWatcher.Ore
{
    internal static class Consts
    {
        public static readonly double SolDecimals = Math.Pow(10, 9);
        public static readonly double OreDecimals = Math.Pow(10, 11);

        public static readonly string OreTokenMint = "oreoU2P8bN6jkk3jbaiVxYnG1dCXcYxwhwyK9jSybcp";
        public static readonly string SolTokenMint = "So11111111111111111111111111111111111111112";

        public const string Rpc = "http://tx.arby.cc/a70b4c704b398a35de538ccfafac";
        public const string JitoRpc = "https://mainnet.block-engine.jito.wtf";
        public const string StreamRpc = "ws://tx.arby.cc/a70b4c704b398a35de538ccfafac";
    }
}
