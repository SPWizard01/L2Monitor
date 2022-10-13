using Serilog;
using System.IO;

namespace L2Monitor.Common.Packets
{
    public class OpCodePacket : BasePacket
    {
        public OpCodePacket(MemoryStream raw) : base(raw)
        {
        }

    }
}
