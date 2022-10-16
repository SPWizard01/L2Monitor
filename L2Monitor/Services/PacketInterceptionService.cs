using L2Monitor.Common;
using L2Monitor.LoginServer;
using L2Monitor.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using PacketDotNet.Connections;
using SharpPcap;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace L2Monitor.Services
{
    public class PacketInterceptionService : IHostedService
    {
        private TcpConnectionManager ConnectionManager;
        private ICaptureDevice CaptureDevice;
        private ILogger Logger;
        private LoginCrypt Crypt;
        //private ILogger
        public PacketInterceptionService(ILogger<PacketInterceptionService> logger)
        {
            Crypt = new LoginCrypt();
            Logger = logger;
        }
        public void Run()
        {
            var ver = Pcap.Version;
            /* Print SharpPcap version */
            Console.WriteLine("SharpPcap {0}", ver);
            Console.WriteLine();


            /* Retrieve the device list */
            var devices = CaptureDeviceList.Instance;

            /*If no device exists, print error */
            if (devices.Count < 1)
            {
                Console.WriteLine("No device found on this machine");
                return;
            }

            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            /* Scan the list printing every entry */
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
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
            Logger.LogInformation("Ruoff Login: 5.63.132.147");
            Logger.LogInformation("Ruoff Elcardia Game: 109.105.133.38");
            Console.WriteLine("-- Enter login server IP i.e. 127.0.0.1: ");
            var lognIp = Console.ReadLine();

            Console.WriteLine("-- Enter game server IP i.e. 127.0.0.1: ");
            var gameIp = Console.ReadLine();
            //109.105.133.38 ru game
            //5.63.132.147 ru login

            //52.44.183.4 us game naia
            //50.16.39.66 us game chronos
            //52.201.101.83 us login
            string filter = $"(host {(string.IsNullOrEmpty(gameIp) ? "50.16.39.66" : gameIp)} or " +
                             $"host {(string.IsNullOrEmpty(lognIp) ? "52.201.101.83" : lognIp)}) and " +
                            $"(tcp src port 2106 or " +
                              "tcp dst port 2106 or " +
                              "tcp src port 7777 or " +
                              "tcp dst port 7777)";


            //string filter = $"host {(string.IsNullOrEmpty(gameIp) ? "109.105.133.76" : gameIp)} and (tcp src port 7777 or tcp dst port 7777)";
            Console.WriteLine($"Filter: {filter}");
            CaptureDevice.Filter = filter;

            Console.WriteLine
                ("-- Listening on {0}, hit 'Ctrl-C' to exit...",
                CaptureDevice.Description);

            // Start capture 'INFINTE' number of packets
            CaptureDevice.Capture();
        }

        private void TcpConnectionManager_OnConnectionFound(TcpConnection connection)
        {
            if (connection.Flows.Any(f => f.port == Constants.LOGIN_PORT))
            {
                ClientHandler.HandleLoginClient(connection);
            }

            if (connection.Flows.Any(f => f.port == Constants.GAME_PORT))
            {
                ClientHandler.HandleGameClient(connection);
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
            //var dest = tcpPacket.SourcePort == Constants.LOGIN_PORT ? PacketDirection.ServerToClient : PacketDirection.ClientToServer;
            //var data = new byte[tcpPacket.PayloadData.Length];
            //tcpPacket.PayloadData.CopyTo(data, 0);
            //var logAddr = $"{dest} Data: {BitConverter.ToString(data)}";
            //Logger.LogInformation(logAddr);
            //Crypt.Decrypt(data, 2, data.Length - 2);
            //if(data[2] == 0 && Crypt.IsInitial)
            //{
            //    Logger.LogInformation($"{dest} BXOR Data: {BitConverter.ToString(data)}");
            //    Crypt.DecXORPass(data);
            //    var init = new Init(new System.IO.MemoryStream(data));
            //    Crypt.SetKey(init.BlowFishKey);
            //}
            //Logger.LogInformation($"{dest} DEC Data: {BitConverter.ToString(data)}");
            //var ipv4Packet = packet.Extract<IPv4Packet>();

            ConnectionManager.ProcessPacket(pck.Timeval, tcpPacket);

        }



        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(Run);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            ConnectionManager.OnConnectionFound -= TcpConnectionManager_OnConnectionFound;
            CaptureDevice.OnPacketArrival -= device_OnPacketArrival;
            CaptureDevice.StopCapture();
            CaptureDevice.Close();
            return Task.CompletedTask;
        }
    }
}
