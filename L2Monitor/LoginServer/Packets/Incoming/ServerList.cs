using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace L2Monitor.LoginServer.Packets.Incoming
{
    public class ServerList : BasePacket
    {
        public ushort ServerCount { get; set; }
        public ushort SelectedServer { get; set; }
        public List<L2ServerInfo> Servers { get; set; } = new();
        public ServerList()
        {

        }
        public ServerList(MemoryStream memoryStream, PacketDirection direction) : base(memoryStream, true, direction)
        {

            
        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new ServerList(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            ServerCount = ReadByteAsShort();
            SelectedServer = ReadByteAsShort();
            for (var i = 0; i < ServerCount; i++)
            {
                Servers.Add(new L2ServerInfo
                {
                    ServerId = ReadByteAsShort(),
                    ServerIP = new System.Net.IPAddress(ReadBytes(4)).ToString(),
                    ServerPort = ReadUInt32(),
                    AgeRestricted = ReadBoolean(),
                    IsPvP = ReadBoolean(),
                    OnlineCount = ReadUInt16(),
                    MaxCount = ReadUInt16(),
                    IsOnline = ReadBoolean(),
                    ServerType = ReadByteAsShort(),
                    HideBrackets = ReadBoolean(),
                    ClientType = ReadUInt16(),
                    Unknown1 = ReadByte()
                });
            }
            ReadUInt32(); //unknown
            //here the charlist for each server should go
            //baseLogger.Information(JsonSerializer.Serialize(Servers));
            //WarnOnRemainingData();
        }
    }
}
