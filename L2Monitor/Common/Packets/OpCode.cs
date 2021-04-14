using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Common.Packets
{
    public class OpCode
    {
        public byte Id1 { get; set; }
        public short Id2 { get; set; }
        public OpCode(byte byteOpCode)
        {
            Id1 = byteOpCode;
            Id2 = 0;
        }

        public OpCode(byte byteOpCode, short shortOpCode2)
        {
            Id1 = byteOpCode;
            Id2 = shortOpCode2;
        }

        public bool Match(byte byteOpCode)
        {
            return Id1 == byteOpCode && Id2 == 0;
        }

        public bool Match(byte byteOpCode, short shortOpCode)
        {
            return Id1 == byteOpCode && Id2 == shortOpCode;
        }

        public bool Match(OpCode matchOpCode)
        {
            return Match(matchOpCode.Id1, matchOpCode.Id2);
        }

        public byte[] GetBytes()
        {
            byte[] byteArr = new byte[3];
            var shortBytes = BitConverter.GetBytes(Id2);
            byteArr[0] = Id1;
            byteArr[1] = shortBytes[0];
            byteArr[2] = shortBytes[1];
            return byteArr;
        }

        public string ToInfoString()
        {
            return $"{BitConverter.ToString(GetBytes())}(0x{Id1:X2}, 0x{Id2:X3})";
        }
    }
}
