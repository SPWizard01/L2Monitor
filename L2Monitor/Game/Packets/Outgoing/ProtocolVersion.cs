using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Packets.Outgoing
{
    public class ProtocolVersion : BasePacket
    {
        public int Version { get; set; }
        public ProtocolVersion(MemoryStream raw) : base(raw)
        {
            Version = readInt();
            //The rest is unknown
        }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
