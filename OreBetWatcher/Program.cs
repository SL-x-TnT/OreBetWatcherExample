using OreBetWatcher.Ore;
using Solnet.Rpc;
using System.Buffers.Binary;
using System.Text;

namespace OreBetWatcher
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var streamingClient = ClientFactory.GetStreamingClient("ws://api.mainnet-beta.solana.com");
            var client = ClientFactory.GetClient("https://api.mainnet-beta.solana.com");

            await streamingClient.ConnectAsync();

            ulong totalDeployed = 0;

            var boardResult = await client.GetAccountInfoAsync(OreProgram.Board, Solnet.Rpc.Types.Commitment.Processed);

            Board currentBoard = Board.ReadFrom(Convert.FromBase64String(boardResult.Result.Value.Data[0]));


            await streamingClient.SubscribeProgramAsync("oreV3EG1i9BEgiAJ8b177Z2S2rMarzak4NMv1kULvWv", (state, data) =>
            {
                var aData = data.Value.Account.Data;

                byte[] accountData = Convert.FromBase64String(aData[0]);

                ulong identifier = BinaryPrimitives.ReadUInt64LittleEndian(accountData);

                switch(identifier)
                {
                    case 103: //Miner
                        Miner miner = Miner.ReadFrom(accountData);

                        //Either bet on current round, or we just started
                        if(miner.RoundId == currentBoard.RoundId)
                        {
                            var deployed = miner.Deployed.Where(x => x > 0).Select((value, index) => (index, value));

                            if (deployed.Count() > 0)
                            {
                                var deployGroups = deployed.GroupBy(x => x.value);

                                StringBuilder builder = new StringBuilder();
                                //Can pull the board with a normal request to get the end slot
                                builder.Append($"Round {miner.RoundId}. Time remaining: {(currentBoard.WaitingForFirstBid ? "??" : currentBoard.EndSlot - data.Context.Slot)} slots. Miner: {miner.Authority} deployed ");

                                foreach(var group in deployGroups)
                                {
                                    builder.Append($"{group.Key / Consts.SolDecimals:0.000000000} sol on squares {String.Join(", ", group.Select(x => x.index))}");
                                }

                                Console.WriteLine(builder.ToString());
                            }
                        }

                        break;
                    case 105: //Board

                        Board board = Board.ReadFrom(accountData);

                        currentBoard = board;

                        break;
                    case 109: //Round

                        //Round round = Round.ReadFrom(accountData);


                        break;
                }

            }, Solnet.Rpc.Types.Commitment.Processed);

            await Task.Delay(-1);
        }
    }

}
