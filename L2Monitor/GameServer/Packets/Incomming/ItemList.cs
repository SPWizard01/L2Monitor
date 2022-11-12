using L2Monitor.Classes;
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
    public class ItemList : BasePacket
    {
        public byte SendType { get; private set; }
        public uint ItemCountUnknown { get; private set; }
        public uint ItemCount { get; private set; }
        public ItemList(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new ItemList(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            SendType = ReadByte();
            ItemCountUnknown = ReadUInt32();
            ItemCount = ReadUInt32();
            if (SendType == 2)
            {
                //we can read item count here....
            }
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }
    }
}
