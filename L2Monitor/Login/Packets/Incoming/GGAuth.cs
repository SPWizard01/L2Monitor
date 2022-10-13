using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Login.Packets.Incoming
{
    public class GGAuth : BasePacket
    {
        public uint SessionId { get; private set; }
        public GGAuth(MemoryStream memoryStream) : base(memoryStream)
        {
            SessionId = readUInt();
            WarnOnRemainingData();
        }
    }
}
