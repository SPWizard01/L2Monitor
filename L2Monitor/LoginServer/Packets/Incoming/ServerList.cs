using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.Collections.Generic;
using System.IO;

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
            ServerCount = readByteAsShort();
            SelectedServer = readByteAsShort();
            for (var i = 0; i < ServerCount; i++)
            {
                Servers.Add(new L2ServerInfo
                {
                    ServerId = readByteAsShort(),
                    ServerIP = new System.Net.IPAddress(readBytes(4)),
                    ServerPort = readUInt(),
                    AgeRestricted = readBool(),
                    IsPvP = readBool(),
                    OnlineCount = readUInt16(),
                    MaxCount = readUInt16(),
                    IsOnline = readBool(),
                    Unknown1 = readBool(),
                    Unknown2 = readBool(),
                    Unknown3 = readBool(),
                    Unknown4 = readUInt16()
                });
            }
            WarnOnRemainingData();
        }
    }
}
