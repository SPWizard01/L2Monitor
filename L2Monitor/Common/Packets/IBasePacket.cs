using L2Monitor.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace L2Monitor.Common.Packets
{
    public interface IBasePacket
    {
        ushort PacketSize { get; set; }
        OpCode OpCode { get; set; }
        public IBasePacket Factory(byte[] raw, PacketDirection direction);

        public void Run(IL2Client client);
    }
}
