using L2Monitor.Config;
using L2Monitor.GameServer;
using L2Monitor.LoginServer;
using PacketDotNet.Connections;
using System.Collections.Generic;
using System.Linq;

namespace L2Monitor.Common
{
    public sealed class ClientHandler
    {
        public List<LoginClient> LoginClients { get; } = new();
        public List<GameClient> GameClients { get; } = new();
        private readonly AppSettings appSettings;
        public LoginClient GetLoginClient()
        {
            return LoginClients.FirstOrDefault();
        }

        public ClientHandler(AppSettings appSettingsInj)
        {
            appSettings = appSettingsInj;
        }

        public void HandleLoginClient(TcpConnection connection)
        {
            if (LoginClients.Any(c => c.TcpConnection == connection))
            {
                return;
            }
            var newClient = new LoginClient(connection);
            LoginClients.Add(newClient);
        }

        internal void HandleGameClient(TcpConnection connection)
        {
            if (GameClients.Any(c => c.TcpConnection == connection))
            {
                return;
            }
            var newClient = new GameClient(connection, appSettings);
            GameClients.Add(newClient);
        }

        internal void RemoveGameClient(GameClient client)
        {
            if (GameClients.Contains(client))
            {
                GameClients.Remove(client);
            }
        }

        internal void RemoveLoginClient(LoginClient client)
        {
            if (LoginClients.Contains(client))
            {
                LoginClients.Remove(client);
            }
        }
    }
}
