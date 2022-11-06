using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Serilog;

namespace L2Monitor.Common.Packets
{
    public abstract class BasePacket : IBasePacket
    {
        private readonly MemoryStream _decryptedStream;
        internal readonly ILogger baseLogger;
        [JsonIgnore]
        public ushort PacketSize { get; set; }
        [JsonIgnore]
        public OpCode OpCode { get; set; }
        public BasePacket(MemoryStream memStream)
        {
            _decryptedStream = memStream;
            baseLogger = Log.ForContext(GetType());
            PacketSize = readUInt16();
            OpCode = new OpCode(memStream.ToArray());
            //skip opcode
            readByte();
            if (OpCode.Id2 > 0)
            {
                //this is extended packet skip another 2 bytes
                readUInt16();
            }
        }
        internal void LogNewDataWarning(string dataName, object data)
        {
            baseLogger.Warning("NEW DATA INSIDE PACKET {packetname}: {data}", dataName, data);
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

        public ushort readByteAsShort()
        {
            return Convert.ToUInt16(readBytes(1)[0]);
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

        public uint readUInt()
        {
            var result = BitConverter.ToUInt32(readBytes(4), 0);
            return result;
        }

        public double readDouble()
        {
            //var result = BitConverter.Int64BitsToDouble(BitConverter.ToInt64(readBytes(8), 0));
            var result = BitConverter.ToDouble(readBytes(8), 0);
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
        public ulong readULong()
        {
            var result = BitConverter.ToUInt64(readBytes(8), 0);
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
            var remainingData = _decryptedStream.Length - _decryptedStream.Position;
            if (_decryptedStream.Position == _decryptedStream.Length || remainingData < length)
            {
                baseLogger.Error("Attempting to read {readcount} bytes at {position}. Remaining data: {remdata}", length, _decryptedStream.Position, remainingData);
                baseLogger.Error("Full packet {packet}", BitConverter.ToString(_decryptedStream.ToArray()));
                return outData;
            }
            _decryptedStream.Read(outData, 0, length);
            return outData;
        }

        public void WarnOnRemainingData()
        {
            if (HasRemainingData())
            {
                var data = GetRemainingData();
                baseLogger.Warning($"This packet has remaining data: {BitConverter.ToString(data)}");
            }
        }

    }
}
