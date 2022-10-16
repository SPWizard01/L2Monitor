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
        public ShortcutData(MemoryStream memStream) : base(memStream)
        {
            ShortCutCount = readUInt();
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }




    }
}
