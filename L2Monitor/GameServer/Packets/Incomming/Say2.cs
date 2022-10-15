using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class Say2 : BasePacket
    {
        public uint RequestId { get; private set; }
        public uint Type { get; private set; }
        public uint Unknown1 { get; private set; }
        public string Actor { get; private set; }
        public int Unknown2 { get; private set; }
        public string Msg { get; private set; }
        public Say2(MemoryStream memStream) : base(memStream)
        {
            RequestId = readUInt();
            Type = readUInt16();
            Unknown1 = readUInt16();
            Actor = readString();
            Unknown2 = readInt();
            Msg = readString();
            WarnOnRemainingData();
            baseLogger.Information("Ping Request: {data}", JsonSerializer.Serialize(this));
        }




    }
}
