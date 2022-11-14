using L2Monitor.Classes;
using L2Monitor.Common;
using L2Monitor.Common.Packets;
using L2Monitor.GameServer.Packets;
using L2Monitor.LoginServer.Packets;
using L2Monitor.LoginServer.Packets.Incoming;
using L2Monitor.Util;
using PacketDotNet;
using PacketDotNet.Connections;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace L2Monitor.LoginServer
{
    public class LoginClient : IL2Client
    {

        public TcpConnection TcpConnection { get; set; }
        public ConnectionState State { get; set; } = ConnectionState.CONNECTED;

        public ICrypt? Crypt { get; private set; }
        private ILogger logger;
        private bool inited = false;

        public LoginClient(TcpConnection connection)
        {
            logger = Log.ForContext<LoginClient>();
            Crypt = new LoginCrypt();
            TcpConnection = connection;
            connection.OnPacketReceived += Connection_OnPacketReceived;
            connection.OnConnectionClosed += Connection_OnConnectionClosed;
        }

        private void Connection_OnConnectionClosed(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpPacket tcp, TcpConnection.CloseType closeType)
        {
            connection.OnConnectionClosed -= Connection_OnConnectionClosed;
            connection.OnPacketReceived -= Connection_OnPacketReceived;
            //ClientHandler.RemoveLoginClient(this);
        }

        private void Connection_OnPacketReceived(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpFlow flow, TcpPacket tcp)
        {
            //incoming packets, first one is Init always
            if (tcp.SourcePort == Constants.LOGIN_PORT && !inited)
            {
                TryInit(tcp);
                return;
            }

            ParsePacket(tcp);

        }

        public void ParsePacket(TcpPacket tcpPacket)
        {
            if (tcpPacket.PayloadData.Length == 0)
            {
                return;
            }
            var direction = tcpPacket.SourcePort == Constants.LOGIN_PORT ? PacketDirection.ServerToClient : PacketDirection.ClientToServer;
            var data = (byte[])tcpPacket.PayloadData.Clone();
            var opcode = DecryptAndGetOpCode(data, direction);
            var correct = Crypt.Checksum(data);
            if (!correct)
            {
                logger.Warning("Checksum was not correct");
            }

            var instance = GetInstance(opcode, direction, data);
            if(instance != null)
            {
                instance.Run(this);
            }
        }

        public void TryInit(TcpPacket tcpPacket)
        {
            if (tcpPacket.PayloadData.Length == 0)
            {
                return;
            }
            try
            {
                var data = (byte[])tcpPacket.PayloadData.Clone();
                var direction = PacketDirection.ServerToClient;
                var test = DecryptAndGetOpCode(data, direction);
                //not init packet.
                if (!test.Match(LoginOpCodes.Init) && !inited)
                {
                    logger.Warning("{direction} Unknown packet before init {data}", direction, BitConverter.ToString(data));
                    return;
                }

                var instance = GetInstance(test, direction, data);
                if (instance is Init)
                {
                    inited = true;
                }

                if (instance != null)
                {
                    instance.Run(this);
                }

            }
            catch (Exception e)
            {
                logger.Error($"Exception occured: {e}");
            }

        }

        private OpCode DecryptAndGetOpCode(byte[] data, PacketDirection direction)
        {
            Crypt.Decrypt(data, direction);

            var test = new OpCode(data, true, direction);
            if (test.Id1 == 0 && inited && direction == PacketDirection.ServerToClient)
            {
                logger.Error($"{direction}: Received Zero Code packet. Payload:{BitConverter.ToString(data)}");
            }
            return test;
        }

        private IBasePacket GetInstance(OpCode test, PacketDirection direction, byte[] data)
        {
            var packetList = direction == PacketDirection.ServerToClient ? LoginPacketsFromServer.All :
                                                               LoginPacketsFromClient.All;

            var cp = packetList.Where(p => p.OpCode.Match(test) && p.States.Contains(State)).FirstOrDefault();
            if (cp == null)
            {
                var closest = packetList.Where(p => p.OpCode.Match(test)).Select(p => p.Name);
                if (closest.Any())
                {
                    logger.Error("Packet {0} was not found. Closest matches: {1}", test.ToInfoString(), string.Join(';', closest));
                    return null;
                }
                logger.Error("{direction}: Unknown Packet {OpCode} Len:{Len} Data:{data}", direction, test.ToInfoString(), data.Length, BitConverter.ToString(data));
                return null;
            }
            if (cp.Packet == null)
            {
                logger.Warning("{0} has been registered but no handler is present. Data: {1}", cp.Name, BitConverter.ToString(data));
                return null;
            }
            return cp.Packet?.Factory(data, direction);
        }

    }
}
