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
        public ItemList(MemoryStream memStream) : base(memStream)
        {
            SendType = readByte();
            ItemCountUnknown = readUInt();
            ItemCount = readUInt();
            if(SendType == 2)
            {
                //we can read item count here....
            }
            WarnOnRemainingData();
            baseLogger.Information("{data}", JsonSerializer.Serialize(this));
        }




    }
}
