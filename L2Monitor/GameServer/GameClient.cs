using L2Monitor.Common;
using L2Monitor.Common.Packets;
using L2Monitor.GameServer.Packets;
using L2Monitor.GameServer.Packets.Incomming;
using L2Monitor.Util;
using PacketDotNet;
using PacketDotNet.Connections;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        private Queue<PacketInTransmit> incommingBuffer = new();
        private Queue<PacketInTransmit> outgoingBuffer = new();

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
            Logger.Information("{dir} received {len} bytes", direction, data.Length);
            var pckStream = new MemoryStream(data);
            var buffer = direction == PacketDirection.ServerToClient ? incommingBuffer : outgoingBuffer;

            //we have incomplete packets, should actualy be 1 if any
            if (buffer.Any())
            {
                var absentPck = buffer.Peek();
                absentPck.AddData(pckStream);
                if (absentPck.RemainingDataLength > 0 && pckStream.Position < pckStream.Length)
                {
                    Logger.Error("This should not happen");
                    Debugger.Break();
                }

                //await for next packet
                if (absentPck.RemainingDataLength > 0)
                {
                    Logger.Information("Unfinished data1. Length: {len} Read: {read} Remaining: {remaining}", absentPck.PackeLength, pckStream.Length, absentPck.RemainingDataLength);
                    return;
                }

            }

            var remainingDatLen = pckStream.Length - pckStream.Position;

            while (remainingDatLen > 0)
            {
                if (remainingDatLen < 2)
                {
                    Logger.Error("Remaining data length less than 2, cannot read packet size, this should not happen");
                    Debugger.Break();
                    return;
                }
                var newPck = new PacketInTransmit(pckStream);
                buffer.Enqueue(newPck);
                remainingDatLen = pckStream.Length - pckStream.Position;
                if ((remainingDatLen > 0 && newPck.RemainingDataLength > 0) || remainingDatLen < 0)
                {
                    Logger.Error("This should not happen");
                    Debugger.Break();
                    return;
                }
                ////wait for next packet...
                //if (newPck.RemainingDataLength > 0)
                //{
                //    //Debugger.Break();
                //    return;
                //}
            }
            //PacketInTransmit bfPck = null;
            //process any full packets in the queue
            while (buffer.Any())
            {
                if(buffer.Peek().RemainingDataLength > 0 && buffer.Count() > 1)
                {
                    Logger.Error("This should not happen");
                    Debugger.Break();
                    return;
                }
                //last item in array awaiting data
                if (buffer.Peek().RemainingDataLength > 0)
                {
                    var incPck = buffer.Peek();
                    Logger.Information("Unfinished data2. Length: {len} Read: {read} Remaining: {remaining}", incPck.PackeLength, pckStream.Length, incPck.RemainingDataLength);
                    break;
                }

                var bfPck = buffer.Dequeue();
                if (gameCrypt != null)
                {
                    var offset = Constants.HEADER_SIZE;
                    var dt = bfPck.PacketData;
                    gameCrypt.Decrypt(dt, offset, dt.Length, direction);
                    gameCrypt.shftKey(offset, dt.Length, direction);
                }

                var parsedPacket = ParsePacket(bfPck.PacketData, direction);
                if (parsedPacket == null)
                {
                    continue;
                }

                if (parsedPacket.GetType() == typeof(CryptInit))
                {
                    if (gameCrypt != null)
                    {
                        Logger.Error("Received {name} packet although crypt already initialized", nameof(CryptInit));
                        continue;
                    }
                    var pck = parsedPacket as CryptInit;
                    gameCrypt = new GameCrypt(pck);
                    continue; ;
                }
            }
            Logger.Information("{direction} parsed, data in buffer {buflen}, stream position {pos}, stream len {len}", direction, buffer.Count, pckStream.Position, pckStream.Length);
            //if(bfPck != null && gameCrypt != null)
            //{
            //    gameCrypt.shftKey(Constants.HEADER_SIZE, bfPck.PackeLength, direction);
            //}
        }

        private IBasePacket ParsePacket(byte[] data, PacketDirection direction)
        {
            var test = new OpCode(data);
            if (test.Id1 == 0)
            {
                Logger.Error("{direction}: Received Zero Code packet. Payload:{data}", direction, BitConverter.ToString(data));
                return null;
            }
            var packetList = direction == PacketDirection.ServerToClient ? GamePackets.ServerToClientPackets :
                                                               GamePackets.ClientToServerPackets;

            var cp = packetList.Where(p => p.OpCode.Match(test)).FirstOrDefault();
            if (cp == null)
            {
                Logger.Warning("{direction}: Unknown {OpCode} Len:{Len} Data:{data}", direction, test.ToInfoString(), data.Length, BitConverter.ToString(data));
                return null;
            }
            //Logger.Information($"{direction}: Found Packet {test.OpCode.ToInfoString()} Type:{cp.Packet} Data:{BitConverter.ToString(data)}");
            var packetInstane = Activator.CreateInstance(cp.Packet, new MemoryStream(data)) as IBasePacket;
            return packetInstane;
        }
    }
}
