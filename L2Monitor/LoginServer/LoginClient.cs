using L2Monitor.Common;
using L2Monitor.Common.Packets;
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
    public class LoginClient
    {

        public TcpConnection TcpConnection { get; set; }
        public LoginCrypt Crypt;
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
            ClientHandler.RemoveLoginClient(this);
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
            var opcode = GetOpCodePacket(data, direction);
            var correct = Crypt.Checksum(data);
            if (!correct)
            {
                logger.Warning("Checksum was not correct");
            }

            var instance = GetInstance(opcode, direction, data);
            var a = 1;
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
                var test = GetOpCodePacket(data, direction);
                //not init packet.
                if (!test.Match(LoginOpCodes.Init) && !inited)
                {
                    logger.Warning("{direction} Unknown packet before init {data}", direction, BitConverter.ToString(data));
                    return;
                }

                //if it is init packet unxor it.
                Crypt.DecXORPass(data);
                //Crypt.javaDecXORPass(data);

                var instance = GetInstance(test, direction, data) as Init;
                if (instance != null)
                {
                    Crypt.SetKey(instance.BlowFishKey);
                    inited = true;
                }

            }
            catch (Exception e)
            {
                logger.Error($"Exception occured: {e}");
            }

        }

        private OpCode GetOpCodePacket(byte[] data, PacketDirection direction)
        {
            Crypt.Decrypt(data);

            var test = new OpCode(data);
            if (test.Id1 == 0 && inited && direction == PacketDirection.ServerToClient)
            {
                logger.Error($"{direction}: Received Zero Code packet. Payload:{BitConverter.ToString(data)}");
            }
            return test;
        }

        private IBasePacket GetInstance(OpCode test, PacketDirection direction, byte[] data)
        {
            var packetList = direction == PacketDirection.ServerToClient ? LoginPackets.ServerToClientPackets : LoginPackets.ClientToServerPackets;
            var cp = packetList.Where(p => p.OpCode.Match(test)).FirstOrDefault();
            if (cp == null)
            {
                logger.Warning($"{direction}: Unknown packet {test.ToInfoString()} Data:{BitConverter.ToString(data)}");
                return null;
            }

            //logger.Information($"{direction}: Found Packet {test.OpCode.ToInfoString()} Type:{cp.Packet}");
            var packetInstane = Activator.CreateInstance(cp.Packet, new MemoryStream(data)) as IBasePacket;
            return packetInstane;
        }

    }
}
