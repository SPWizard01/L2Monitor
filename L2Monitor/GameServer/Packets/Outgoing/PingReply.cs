using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Outgoing
{
    public class PingReply : BasePacket
    {
        public uint RequestId { get; private set; }
        public uint SomeNumber { get; private set; }
        public uint Unknown { get; private set; }
        public PingReply(MemoryStream raw) : base(raw)
        {
            RequestId = readUInt();
            SomeNumber = readUInt();
            Unknown = readUInt();
            WarnOnRemainingData();
            baseLogger.Information("Ping Reply: {data}", JsonSerializer.Serialize(this));
        }


    }
}
