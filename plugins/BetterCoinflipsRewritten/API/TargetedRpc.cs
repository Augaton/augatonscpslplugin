using System;
using LabApi.Features.Wrappers;
using Mirror;

namespace BetterCoinflipsRewritten.API
{
    // Ces RPC sont diffuses a tout le serveur par le jeu : on rejoue le message Mirror vers un seul client.
    public static class TargetedRpc
    {
        private const string DimScreenRpc = "System.Void RoundSummary::RpcDimScreen()";
        private const string UndimScreenRpc = "System.Void RoundSummary::RpcUndimScreen()";
        private const string WarheadShakeRpc = "System.Void AlphaWarheadController::RpcShake(System.Boolean)";

        public static void DimScreen(Player player) =>
            Send(player, RoundSummary.singleton, DimScreenRpc);

        public static void UndimScreen(Player player) =>
            Send(player, RoundSummary.singleton, UndimScreenRpc);

        public static void WarheadShake(Player player, bool achieve)
        {
            if (!AlphaWarheadController.SingletonSet)
                return;

            Send(
                player,
                AlphaWarheadController.Singleton,
                WarheadShakeRpc,
                writer => writer.WriteBool(achieve));
        }

        private static void Send(
            Player player,
            NetworkBehaviour behaviour,
            string rpcFullName,
            Action<NetworkWriter> writePayload = null)
        {
            if (player is null || player.IsDestroyed || behaviour == null)
                return;

            using NetworkWriterPooled writer = NetworkWriterPool.Get();

            writePayload?.Invoke(writer);

            player.Connection.Send(new RpcMessage
            {
                netId = behaviour.netId,
                componentIndex = behaviour.ComponentIndex,
                functionHash = (ushort)rpcFullName.GetStableHashCode(),
                payload = writer.ToArraySegment(),
            });
        }
    }
}
