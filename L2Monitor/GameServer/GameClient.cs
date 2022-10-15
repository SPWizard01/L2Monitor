using PacketDotNet;
using L2Monitor.Common.Packets;
using L2Monitor.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Serilog;
using L2Monitor.Common;
using L2Monitor.GameServer.Packets.Incomming;
using L2Monitor.GameServer.Packets;
using PacketDotNetConnections;

namespace L2Monitor.GameServer
{
    public class GameClient
    {
        private GameCrypt gameCrypt;
        public TcpConnection TcpConnection { get; set; }
        private ILogger Logger;

        private int MAX_FRAME_SIZE = 1460;
        private PacketDirection incompleteDirection;
        private byte[] incompletePacketData = Array.Empty<byte>();

        public GameClient(TcpConnection connection)
        {
            Logger = Log.ForContext<GameClient>();
            TcpConnection = connection;
            connection.OnPacketReceived += Connection_OnPacketReceived;
            connection.OnConnectionClosed += Connection_OnConnectionClosed;
        }

        private void Connection_OnConnectionClosed(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpPacket tcp, TcpConnection.CloseType closeType)
        {
            TcpConnection.OnPacketReceived -= Connection_OnPacketReceived;
            TcpConnection.OnConnectionClosed -= Connection_OnConnectionClosed;
            ClientHandler.RemoveGameClient(this);
        }

        private void Connection_OnPacketReceived(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpFlow flow, TcpPacket tcpPacket)
        {
            if (tcpPacket.PayloadData.Length == 0)
            {
                return;
            }
            var data = (byte[])tcpPacket.PayloadData.Clone();
            var direction = tcpPacket.SourcePort == Constants.GAME_PORT ? PacketDirection.ServerToClient : PacketDirection.ClientToServer;
            //expect another packet
            if (data.Length == MAX_FRAME_SIZE && direction == PacketDirection.ServerToClient)
            {
                incompletePacketData = incompletePacketData.Concat(data).ToArray();
                return;
            }
            if(incompletePacketData.Length > 0 && direction == PacketDirection.ServerToClient)
            {
                data = incompletePacketData.Concat(data).ToArray();
                incompletePacketData = Array.Empty<byte>();
            }


            if (gameCrypt != null)
            {
                //Logger.Information("Before decrypt {data}", BitConverter.ToString(data));
                var offset = Constants.HEADER_SIZE;
                gameCrypt.Decrypt(data, offset, data.Length, direction);
                //gameCrypt.DecryptServerTest(data, direction);
                //Logger.Information("After decrypt {data}", BitConverter.ToString(data));
                var registeredSize = BitConverter.ToUInt16(data, 0);
                if (registeredSize != data.Length)
                {
                    Logger.Error("Size mismatch, Data: {datasize}; Read: {regsize}", data.Length, registeredSize);
                }

            }




            var parsedPacket = ParsePacket(data, direction);
            if (parsedPacket == null)
            {
                return;
            }

            if (parsedPacket.GetType() == typeof(CryptInit) && gameCrypt == null)
            {
                var pck = parsedPacket as CryptInit;
                gameCrypt = new GameCrypt(pck);
                return;
            }

        }

        private IBasePacket ParsePacket(byte[] data, PacketDirection direction)
        {
            var test = new OpCodePacket(new MemoryStream(data));
            if (test.OpCode.Id1 == 0)
            {
                Logger.Error("{direction}: Received Zero Code packet. Payload:{data}", direction, BitConverter.ToString(data));
                return null;
            }
            var packetList = direction == PacketDirection.ServerToClient ? GamePackets.ServerToClientPackets :
                                                               GamePackets.ClientToServerPackets;

            var cp = packetList.Where(p => p.OpCode.Match(test.OpCode)).FirstOrDefault();
            if (cp == null)
            {
                Logger.Warning($"{direction}: Unknown packet {test.OpCode.ToInfoString()} Data:{BitConverter.ToString(data)}");
                return null;
            }
            //Logger.Information($"{direction}: Found Packet {test.OpCode.ToInfoString()} Type:{cp.Packet} Data:{BitConverter.ToString(data)}");
            var packetInstane = Activator.CreateInstance(cp.Packet, new MemoryStream(data)) as IBasePacket;
            return packetInstane;
        }
    }
}
