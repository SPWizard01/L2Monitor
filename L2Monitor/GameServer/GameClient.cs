using L2Monitor.Classes;
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
    public class GameClient : IL2Client
    {
        public ICrypt? Crypt { get; private set; } = new GameCrypt();
        public TcpConnection TcpConnection { get; set; }
        public ConnectionState State { get; set; } = ConnectionState.CONNECTED;
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
                    Logger.Information("{direction} Unfinished data1. Length: {len} Read: {read} Remaining: {remaining}", direction, absentPck.PacketLength, pckStream.Length, absentPck.RemainingDataLength);
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
            }
            //PacketInTransmit bfPck = null;
            //process any full packets in the queue
            while (buffer.Any())
            {
                if (buffer.Peek().RemainingDataLength > 0 && buffer.Count() > 1)
                {
                    Logger.Error("This should not happen");
                    Debugger.Break();
                    return;
                }
                //last item in array awaiting data
                if (buffer.Peek().RemainingDataLength > 0)
                {
                    var incPck = buffer.Peek();
                    Logger.Information("{direction} Unfinished data2. Length: {len} Read: {read} Remaining: {remaining}", direction, incPck.PacketLength, pckStream.Length, incPck.RemainingDataLength);
                    break;
                }

                var bfPck = buffer.Dequeue();
                var dt = bfPck.PacketData;
                Crypt.Decrypt(dt, direction);

                var parsedPacket = ParsePacket(bfPck.PacketData, direction);
                if (parsedPacket == null)
                {
                    continue;
                }

                parsedPacket.Run(this);
            }
        }

        private IBasePacket? ParsePacket(byte[] data, PacketDirection direction)
        {
            var test = new OpCode(data, false, direction);

            var packetList = direction == PacketDirection.ServerToClient ? GamePacketsFromServer.All :
                                                               GamePacketsFromClient.All;

            var cp = packetList.Where(p => p.OpCode.Match(test) && p.States.Contains(State)).FirstOrDefault();
            if (cp == null)
            {
                var closest = packetList.Where(p => p.OpCode.Match(test)).Select(p => p.Name);
                if (closest.Any())
                {
                    Logger.Error("{0}: Packet {1} was not found. Closest matches: {2} Data: {3}", direction, test.ToInfoString(), string.Join(';', closest), BitConverter.ToString(data, 2));
                    return null;
                }
                Logger.Error("{direction}: Unknown Packet {OpCode} Len:{Len} Data:{data}", direction, test.ToInfoString(), data.Length, BitConverter.ToString(data, 2));
                return null;
            }
            if (cp.Packet == null)
            {
                Logger.Warning("{0}: {1} has been registered but no handler is present. Data: {2}", direction, cp.Name, BitConverter.ToString(data, 2));
                return null;
            }
            return cp.Packet?.Factory(data, direction);
        }
    }
}
