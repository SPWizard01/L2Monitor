using L2Monitor.Common;
using L2Monitor.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using PacketDotNet.Connections;
using SharpPcap;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace L2Monitor.Services
{
    public class L2MainService : BackgroundService
    {
        private TcpConnectionManager ConnectionManager;
        private ICaptureDevice CaptureDevice;
        private ILogger logger;

        private readonly ClientHandler handler;

        private IPAddress gameIp;
        private IPAddress loginIp;

        //private ILogger
        public L2MainService(ILogger<L2MainService> loggerInj, ClientHandler handlerInj)
        {
            logger = loggerInj;
            handler = handlerInj;
        }
        public void Run()
        {
            /* Print SharpPcap version */
            logger.LogInformation("SharpPcap {0}", Pcap.Version);


            /* Retrieve the device list */
            var devices = CaptureDeviceList.Instance;

            /*If no device exists, print error */
            if (devices.Count < 1)
            {
                logger.LogWarning("No device found on this machine, ensure Npcap {0} installed.", new Uri("https://npcap.com/#download"));
                return;
            }

            logger.LogInformation("The following devices are available on this machine:");
            logger.LogInformation("----------------------------------------------------");
            logger.LogInformation("");

            int i = 0;

            /* Scan the list printing every entry */
            foreach (var dev in devices)
            {
                /* Description */
                logger.LogInformation("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            logger.LogInformation("");
            logger.LogInformation("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine());

            CaptureDevice = devices[i];
            //Register our handler function to the 'packet arrival' event
            CaptureDevice.OnPacketArrival += device_OnPacketArrival;

            // Open the device for capturing
            int readTimeoutMilliseconds = 500;
            CaptureDevice.Open(DeviceModes.None, readTimeoutMilliseconds);
            ConnectionManager = new TcpConnectionManager();
            ConnectionManager.OnConnectionFound += TcpConnectionManager_OnConnectionFound; ;
            //tcpdump filter to capture only TCP/IP packets
            //host 109.105.133.76 and
            logger.LogInformation("Ruoff Login: 5.63.132.147");
            logger.LogInformation("Ruoff Elcardia Game: 109.105.133.38");
            logger.LogInformation("-- Enter login server IP i.e. 127.0.0.1: ");
            var lgIp = Console.ReadLine();
            lgIp = string.IsNullOrEmpty(lgIp) ? "52.201.101.83" : lgIp;
            loginIp = IPAddress.Parse(lgIp);

            logger.LogInformation("-- Enter game server IP i.e. 127.0.0.1: ");
            var gmIp = Console.ReadLine();
            gmIp = string.IsNullOrEmpty(gmIp) ? "50.16.39.66" : gmIp;
            gameIp = IPAddress.Parse(gmIp);

            //109.105.133.38 ru game
            //5.63.132.147 ru login

            //52.44.183.4 us game naia
            //50.16.39.66 us game chronos
            //52.201.101.83 us login
            string filter = $"(host {gameIp} or " +
                             $"host {loginIp}) and " +
                            $"(tcp src port 2106 or " +
                              "tcp dst port 2106 or " +
                              "tcp src port 7777 or " +
                              "tcp dst port 7777)";


            //string filter = $"host {(string.IsNullOrEmpty(gameIp) ? "109.105.133.76" : gameIp)} and (tcp src port 7777 or tcp dst port 7777)";
            logger.LogInformation("Filter: {0}", filter);
            CaptureDevice.Filter = filter;

            logger.LogInformation("Listening on {0}, hit 'Ctrl-C' to exit...", CaptureDevice.Description);

            // Start capture 'INFINTE' number of packets
            CaptureDevice.Capture();
        }

        private void TcpConnectionManager_OnConnectionFound(TcpConnection connection)
        {
            if (connection.Flows.Any(f => f.port == Constants.LOGIN_PORT && f.address.Equals(loginIp)))
            {
                handler.HandleLoginClient(connection);
            }
            if (connection.Flows.Any(f => f.port == Constants.GAME_PORT && f.address.Equals(gameIp)))
            {
                handler.HandleGameClient(connection);
            }

        }


        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            var pck = e.GetPacket();
            var packet = Packet.ParsePacket(pck.LinkLayerType,
                                             pck.Data);

            var tcpPacket = packet.Extract<TcpPacket>();
            if (tcpPacket == null || tcpPacket.PayloadData.Length == 0)
            {
                return;
            }
            ConnectionManager.ProcessPacket(pck.Timeval, tcpPacket);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ConnectionManager.OnConnectionFound -= TcpConnectionManager_OnConnectionFound;
            CaptureDevice.OnPacketArrival -= device_OnPacketArrival;
            CaptureDevice.StopCapture();
            CaptureDevice.Close();
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(Run);
        }
    }
}
