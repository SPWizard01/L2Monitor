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
    public class ShortcutData : BasePacket
    {
        public uint ShortCutCount { get; private set; }
        public ShortcutData()
        {

        }
        public ShortcutData(MemoryStream memStream, PacketDirection direction) : base(memStream, false, direction)
        {

        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new ShortcutData(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            ShortCutCount = ReadUInt32();
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }
    }
}
