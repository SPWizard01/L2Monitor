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
    public class Ping : BasePacket
    {
        public uint RequestId { get; private set; }
        public Ping(MemoryStream memStream) : base(memStream)
        {
            RequestId = readUInt();
            WarnOnRemainingData();
            baseLogger.Information("Ping Request: {data}", JsonSerializer.Serialize(this));
        }




    }
}
