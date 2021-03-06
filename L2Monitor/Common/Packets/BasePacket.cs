using System;
using System.IO;
using System.Linq;
using System.Text;

namespace L2Monitor.Common.Packets
{
    public abstract class BasePacket : IBasePacket
    {
        private readonly MemoryStream _decryptedStream;
        public ushort PacketSize { get; set; }
        public OpCode OpCode { get; set; }
        public BasePacket(MemoryStream memStream)
        {
            _decryptedStream = memStream;
            PacketSize = readUInt16();
            var byteOpCode = readByte();
            short shortOpCode = 0;

            if(byteOpCode == 0xFE)
            {
                //this is extended packet
                shortOpCode = readInt16();
            }
            OpCode = new OpCode(byteOpCode, shortOpCode);
        }

        public bool HasRemainingData()
        {
            return _decryptedStream.Position != _decryptedStream.Length;
        }

        public byte[] GetRemainingData()
        {
            var len = (int)_decryptedStream.Length - (int)_decryptedStream.Position;
            return readBytes(len);
        }

        public byte readByte()
        {
            var result = Convert.ToByte(readBytes(1)[0]);
            return result;
        }

        public bool readBool()
        {
            var result = BitConverter.ToBoolean(readBytes(1), 0);
            return result;
        }

        public int readInt()
        {
            var result = BitConverter.ToInt32(readBytes(4), 0);
            return result;
        }

        public double readDouble()
        {
            var result = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(readBytes(8), 0));
            return result;
        }

        public float readFloat()
        {
            var result = BitConverter.ToSingle(readBytes(4), 0);
            return result;
        }

        public long readLong()
        {
            var result = BitConverter.ToInt64(readBytes(8), 0);
            return result;
        }

        public ushort readUInt16()
        {

            var result = BitConverter.ToUInt16(readBytes(2));
            return result;
        }

        public short readInt16()
        {

            var result = BitConverter.ToInt16(readBytes(2));
            return result;
        }

        public char readChar()
        {
            var character = BitConverter.ToChar(readBytes(2));
            return character;
        }


        public string readString()
        {
            StringBuilder sb = new StringBuilder();
            char chr;

            while ((chr = readChar()) != char.MinValue)
            {
                sb.Append(chr);
            }
            return sb.ToString();
        }


        public byte[] readBytes(int length)
        {
            var outData = new byte[length];
            if (!_decryptedStream.CanRead || _decryptedStream.Position == _decryptedStream.Length)
            {
                
                return outData;
            }
            _decryptedStream.Read(outData, 0, length);
            return outData;
        }

        public abstract void Parse();
    }
}
