using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Common.Packets
{
    public class OpCodePacket : BasePacket
    {
        public OpCodePacket(MemoryStream raw) : base(raw)
        {
        }

        public override void Parse()
        {
            
        }
    }
}
