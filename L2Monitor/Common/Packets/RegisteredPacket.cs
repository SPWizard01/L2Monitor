using L2Monitor.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Common.Packets
{
    public sealed class RegisteredPacket
    {
        public string Name { get; }
        public OpCode OpCode { get; }
        public IBasePacket? Packet { get; }
        public ConnectionState[] States { get; }

        public RegisteredPacket(string name, byte id1, IBasePacket? packet, params ConnectionState[] states)
        {
            Name = name;
            OpCode = new OpCode(id1);
            Packet = packet;
            States = states;
        }

        public RegisteredPacket(string name, byte id1, ushort id2, IBasePacket? packet, params ConnectionState[] states)
        {
            Name = name;
            OpCode = new OpCode(id1, id2);
            Packet = packet;
            States = states;
        }
    }
}
