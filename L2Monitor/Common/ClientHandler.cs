using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using L2Monitor.GameServer;
using PacketDotNetConnections;
using L2Monitor.LoginServer;

namespace L2Monitor.Common
{
    public static class ClientHandler
    {
        private static List<LoginClient> loginClients = new List<LoginClient>();
        private static List<GameClient> gameClients = new List<GameClient>();

        public static LoginClient GetLoginClient()
        {
            return loginClients.FirstOrDefault();
        }

        public static void HandleLoginClient(TcpConnection connection)
        {
            if(loginClients.Any(c=>c.TcpConnection == connection))
            {
                return;
            }
            var newClient = new LoginClient(connection);
            loginClients.Add(newClient);

        }

        internal static void HandleGameClient(TcpConnection connection)
        {
            if (gameClients.Any(c => c.TcpConnection == connection))
            {
                return;
            }
            var newClient = new GameClient(connection);
            gameClients.Add(newClient);
        }

        internal static void RemoveGameClient(GameClient client)
        {
            if(gameClients.Contains(client))
            {
                gameClients.Remove(client);
            }
        }

        internal static void RemoveLoginClient(LoginClient client)
        {
            if (loginClients.Contains(client))
            {
                loginClients.Remove(client);
            }
        }
    }
}
