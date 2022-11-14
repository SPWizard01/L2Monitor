using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public sealed class MoveToLocation : BasePacket
    {
        public int ObjectId { get; set; }
        public int FromX { get; set; }
        public int FromY { get; set; }
        public int FromZ { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
        public int ToZ { get; set; }
        public MoveToLocation()
        {
        }

        public MoveToLocation(MemoryStream stream, PacketDirection direction) : base(stream, false, direction)
        {
        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new MoveToLocation(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            ObjectId = ReadInt32();
            ToX = ReadInt32();
            ToY = ReadInt32();
            ToZ = ReadInt32();
            FromX = ReadInt32();
            FromY = ReadInt32();
            FromZ = ReadInt32();
            baseLogger.Information(JsonSerializer.Serialize(this));
        }

    }
}
