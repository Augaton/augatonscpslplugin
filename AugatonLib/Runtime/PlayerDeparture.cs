using System;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using Mirror;

namespace AugatonLib.Runtime
{
    /// <summary>
    /// Annonce le depart d'un joueur tant que son role, sa vie et son inventaire sont intacts.
    /// PlayerEvents.Left arrive trop tard : disconnect_drop tue d'abord le joueur, puis Mirror passe
    /// son role a Destroyed via OnStopClient, car le serveur tourne en mode hote.
    /// OnDisconnectedEvent est appele juste avant CustomNetworkManager.OnServerDisconnect, le point
    /// d'entree du kill. StartHost le reecrit, d'ou sa pose a WaitingForPlayers.
    /// </summary>
    public static class PlayerDeparture
    {
        private static readonly Action<NetworkConnectionToClient> Interceptor = OnDisconnected;

        private static Action<NetworkConnectionToClient> forward;
        private static bool hooked;

        public static event Action<Player> Departing;

        internal static void Hook()
        {
            if (hooked)
                return;

            hooked = true;
            ServerEvents.WaitingForPlayers += Intercept;
        }

        private static void Intercept()
        {
            Action<NetworkConnectionToClient> current = NetworkServer.OnDisconnectedEvent;

            if (current is null || current == Interceptor)
                return;

            forward = current;
            NetworkServer.OnDisconnectedEvent = Interceptor;
        }

        private static void OnDisconnected(NetworkConnectionToClient connection)
        {
            Announce(connection);
            forward(connection);
        }

        private static void Announce(NetworkConnectionToClient connection)
        {
            try
            {
                Player player = Player.Get(connection.identity);

                if (player is not null && !player.IsHost)
                    Departing?.Invoke(player);
            }
            catch (Exception e)
            {
                Logger.Error($"PlayerDeparture: {e}");
            }
        }
    }
}
