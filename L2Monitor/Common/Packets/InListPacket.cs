using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L2Monitor.Common.Packets
{
    public class InListPacket
    {
        private InListPacket()
        {

        }
        public static InListPacket FromPacketId<T>(byte byteOpCode) where T : IBasePacket 
        {
            return new InListPacket
            {
                OpCode = new OpCode(byteOpCode),
                Packet = typeof(T)
            };
        }
        public static InListPacket FromPacketId<T>(byte byteOpCode, short shortOpCode) where T : IBasePacket
        {
            return new InListPacket
            {
                OpCode = new OpCode(byteOpCode, shortOpCode),
                Packet = typeof(T)
            };
        }


        public OpCode OpCode { get; private set; }

        public Type Packet { get; private set; }
    }
}
