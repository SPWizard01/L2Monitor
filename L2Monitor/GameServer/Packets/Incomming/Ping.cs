using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System.IO;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class Ping : BasePacket
    {
        public uint RequestId { get; private set; }
        public Ping()
        {

        }
        public Ping(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new Ping(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            RequestId = ReadUInt32();
            WarnOnRemainingData();
            //baseLogger.Information("Ping Request: {data}", JsonSerializer.Serialize(this));
        }
    }
}
