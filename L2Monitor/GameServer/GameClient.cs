using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.Config;
using L2Monitor.GameServer.Packets;
using L2Monitor.Util;
using PacketDotNet;
using PacketDotNet.Connections;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace L2Monitor.GameServer
{
    public class GameClient : IL2Client
    {
        public ICrypt? Crypt { get; private set; } = new GameCrypt();
        public TcpConnection TcpConnection { get; set; }

        public ClientOpCodeObfuscator Obfuscator { get; set; }
        public ConnectionState State { get; set; } = ConnectionState.CONNECTED;
        private ILogger Logger;
        private readonly AppSettings appSettings;

        private Queue<PacketInTransmit> incommingBuffer = new();
        private Queue<PacketInTransmit> outgoingBuffer = new();

        public GameClient(TcpConnection connection, AppSettings appSettingsInj)
        {
            Logger = Log.ForContext<GameClient>();
            TcpConnection = connection;
            appSettings = appSettingsInj;

            Obfuscator = new ClientOpCodeObfuscator(appSettings);
            connection.OnPacketReceived += Connection_OnPacketReceived;
            connection.OnConnectionClosed += Connection_OnConnectionClosed;
        }

        private void Connection_OnConnectionClosed(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpPacket tcp, TcpConnection.CloseType closeType)
        {
            TcpConnection.OnPacketReceived -= Connection_OnPacketReceived;
            TcpConnection.OnConnectionClosed -= Connection_OnConnectionClosed;
            //ClientHandler.RemoveGameClient(this);
        }

        private void Connection_OnPacketReceived(SharpPcap.PosixTimeval timeval, TcpConnection connection, TcpFlow flow, TcpPacket tcpPacket)
        {
            if (tcpPacket.PayloadData.Length == 0)
            {
                return;
            }

            var frameData = new byte[tcpPacket.PayloadData.Length];//(byte[])tcpPacket.PayloadData.Clone();
            Buffer.BlockCopy(tcpPacket.PayloadData, 0, frameData, 0, frameData.Length);

            var direction = tcpPacket.SourcePort == Constants.GAME_PORT ? PacketDirection.ServerToClient : PacketDirection.ClientToServer;

            var frameStream = new MemoryStream(frameData);
            var buffer = direction == PacketDirection.ServerToClient ? incommingBuffer : outgoingBuffer;

            //we have incomplete packets, should actualy be 1 if any
            if (buffer.Any())
            {
                var absentPck = buffer.Peek();
                absentPck.AddData(frameStream);
                if (absentPck.RemainingDataLength > 0 && frameStream.Position < frameStream.Length)
                {
                    throw new InvalidDataException("There is still remaining data to be read inside buffer but Frame has remaning data. This should not happen");
                }

                //await for next packet
                if (absentPck.RemainingDataLength > 0)
                {
                    Logger.Information("{direction} Unfinished packet waiting in buffer. Length: {len} Remaining: {remaining} Frame data: {read}", direction, absentPck.PacketLength, absentPck.RemainingDataLength, frameStream.Length);
                    return;
                }

            }

            var remainingDatLen = frameStream.Length - frameStream.Position;

            while (remainingDatLen > 0)
            {
                if (remainingDatLen < 2)
                {
                    Logger.Warning("Remaining data length less than 2, will try to reconstruct from next frame");
                    var partPck = new PacketInTransmit((byte)frameStream.ReadByte());
                    buffer.Enqueue(partPck);
                    return;
                }

                //if (remainingDatLen == 2)
                //{
                //    Logger.Warning("Remaining data length is exactly 2 bytes");
                //    var partPck = new PacketInTransmit((byte)frameStream.ReadByte());
                //    buffer.Enqueue(partPck);
                //    return;
                //}

                var newPck = new PacketInTransmit(frameStream);
                buffer.Enqueue(newPck);
                remainingDatLen = frameStream.Length - frameStream.Position;
                if (newPck.RemainingDataLength > 0)
                {
                    Logger.Warning("The remainig data is not 0, this means that arrived packet only has lenght left, will try to reconstruct from next packet.");
                    Logger.Warning("At this point, remaining data in frame: {0}, remaining data in packet: {1}", remainingDatLen, newPck.RemainingDataLength);
                    return;
                }
            }
            //PacketInTransmit bfPck = null;
            //process any full packets in the queue
            while (buffer.Any())
            {
                var pckInTransmit = buffer.Peek();
                if (pckInTransmit.RemainingDataLength > 0 && buffer.Count() > 1)
                {
                    throw new InvalidDataException("Remaining data to be received is non 0 however the buffer still has some leftovers.");
                }
                //last item in array awaiting data
                if (pckInTransmit.RemainingDataLength > 0)
                {
                    Logger.Information("{direction} Split data found. Length: {len} Remaining: {remaining} Packet data: {read}", direction, pckInTransmit.PacketLength, pckInTransmit.RemainingDataLength, frameStream.Length);
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
                try
                {
                    parsedPacket.Run(this);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error while running packet {0}", parsedPacket.GetType().Name);
                }
            }
        }

        private IBasePacket? ParsePacket(byte[] data, PacketDirection direction)
        {

            var originalOpcode = new OpCode(data, false, direction);
            var isDecoded = false;
            if (direction == PacketDirection.ClientToServer && Obfuscator.Inited)
            {
                isDecoded = true;
                Obfuscator.DecodedOpCode(data, Constants.HEADER_SIZE);
            }
            var opCodeInfo = string.Format("Original: {0}", originalOpcode.ToInfoString());

            var decoded = isDecoded ? new OpCode(data, false, direction) : originalOpcode;

            if (isDecoded)
            {
                opCodeInfo += string.Format(", Decoded: {0}", decoded.ToInfoString());
            }


            var packetList = direction == PacketDirection.ServerToClient ?
                                               GamePacketsFromServer.All :
                                               GamePacketsFromClient.All;
            RegisteredPacket? cp = null;
            foreach (var packet in packetList)
            {
                if (packet.OpCode.Match(decoded) && packet.States.Contains(State))
                {
                    cp = packet;
                    break;
                }
            }

            if (cp == null)
            {
                var closest = new List<string>();
                foreach (var packet in packetList)
                {
                    if (packet.OpCode.Match(decoded))
                    {
                        closest.Add(packet.Name);
                    }
                }
                if (closest.Any())
                {
                    //if (direction == PacketDirection.ClientToServer)
                    Logger.Warning("{0}: Packet {1} was not found. Closest matches: {2} Data: {3}", direction, opCodeInfo, string.Join(';', closest), BitConverter.ToString(data, 2));
                    return null;
                }
                //if (direction == PacketDirection.ClientToServer)
                Logger.Error("{0}: Unknown Packet {1} Len:{2} Data:{3}", direction, opCodeInfo, data.Length, BitConverter.ToString(data, 2));
                return null;
            }
            if (cp.Packet == null)
            {
                //if (direction == PacketDirection.ClientToServer)
                Logger.Warning("{0}: {1} ({2}) has been registered but no handler is present. Data: {3}", direction, cp.Name, opCodeInfo, BitConverter.ToString(data, 2));
                return null;
            }
            return cp.Packet?.Factory(data, direction);
        }
    }
}
