using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.IO;

namespace L2Monitor.GameServer.Packets.Outgoing
{
    public class ProtocolVersion : BasePacket
    {
        public int Version { get; set; }
        public ProtocolVersion(MemoryStream raw) : base(raw)
        {
            Version = readInt();
            //The rest is unknown
        }


    }
}
