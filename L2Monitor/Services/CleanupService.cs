using L2Monitor.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L2Monitor.Services
{
    public sealed class CleanupService : BackgroundService
    {
        private PeriodicTimer timer;
        private ILogger logger;
        private ClientHandler handler;
        public CleanupService(ILogger<CleanupService> loggerInj, ClientHandler handlerInj)
        {
            logger = loggerInj;
            handler = handlerInj;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            return base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var logindcd = handler.LoginClients.RemoveAll(c => c.TcpConnection.ConnectionState == PacketDotNet.Connections.TcpConnection.ConnectionStates.Closed);
                if (logindcd > 0)
                {
                    logger.LogInformation("Removed {0} disconnected Login clients...", logindcd);
                   
                }

                var gamedcd = handler.GameClients.RemoveAll(c => c.TcpConnection.ConnectionState == PacketDotNet.Connections.TcpConnection.ConnectionStates.Closed);
                if (gamedcd > 0)
                {
                    logger.LogInformation("Removed {0} disconnected Game clients...", gamedcd);
                    
                }
            }
        }
    }
}
