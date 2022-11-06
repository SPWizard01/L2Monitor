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
        public ServerList(MemoryStream memoryStream) : base(memoryStream)
        {
            var memArr = memoryStream.ToArray();
            baseLogger.Information(BitConverter.ToString(memArr));
            ServerCount = readByteAsShort();
            SelectedServer = readByteAsShort();
            for (var i = 0; i < ServerCount; i++)
            {
                Servers.Add(new L2ServerInfo
                {
                    ServerId = readByteAsShort(),
                    ServerIP = new System.Net.IPAddress(readBytes(4)).ToString(),
                    ServerPort = readUInt(),
                    AgeRestricted = readBool(),
                    IsPvP = readBool(),
                    OnlineCount = readUInt16(),
                    MaxCount = readUInt16(),
                    IsOnline = readBool(),
                    ServerType = readByteAsShort(),
                    HideBrackets = readBool(),
                    ClientType = readUInt16(),
                    Unknown1 = readByte()
                });
            }
            readUInt(); //unknown
            //here the charlist for each server should go
            //baseLogger.Information(JsonSerializer.Serialize(Servers));
            //WarnOnRemainingData();
            
        }
    }
}
