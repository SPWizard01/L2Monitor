using System;
using System.IO;

namespace L2Monitor.Common.Packets
{
    public class OpCode
    {
        public byte Id1 { get; set; }
        public ushort Id2 { get; set; }
        public OpCode(byte byteOpCode)
        {
            Id1 = byteOpCode;
            Id2 = 0;
        }

        public OpCode(byte byteOpCode, ushort shortOpCode2)
        {
            Id1 = byteOpCode;
            Id2 = shortOpCode2;
        }


        public OpCode(byte[] raw)
        {
            Id1 = raw[2];
            Id2 = 0;
            if (Id1 == 0xFE)
            {
                Id2 = BitConverter.ToUInt16(raw, 3);
            }
        }

        public bool Match(byte byteOpCode)
        {
            return Id1 == byteOpCode && Id2 == 0;
        }

        public bool Match(byte byteOpCode, ushort shortOpCode)
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
