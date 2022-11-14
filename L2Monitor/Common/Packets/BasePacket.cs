using L2Monitor.Classes;
using L2Monitor.Util;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;

namespace L2Monitor.Common.Packets
{
    public abstract class BasePacket : BinaryReader, IBasePacket
    {
        internal readonly ILogger baseLogger;

        [JsonIgnore]
        public ushort PacketSize { get; set; }

        [JsonIgnore]
        public OpCode OpCode { get; set; }

        [JsonIgnore]
        public override Stream BaseStream { get => base.BaseStream; }

        public BasePacket() : base(new MemoryStream())
        {

        }

        public BasePacket(MemoryStream stream, bool isLogin, PacketDirection direction) : base(stream, Encoding.Unicode)
        {
            baseLogger = Log.ForContext(GetType());
            PacketSize = ReadUInt16();
            var id1 = ReadByte();
            var id2 = (ushort)0;
            if (!isLogin)
            {
                var maxSize = direction == PacketDirection.ServerToClient ? Constants.MAX_INCOMMING : Constants.MAX_OUTGOING;
                if (id1 >= maxSize)
                {
                    id2 = ReadUInt16();
                }
            }

            OpCode = new OpCode(id1, id2);
        }


        public abstract IBasePacket Factory(byte[] raw, PacketDirection direction);

        public abstract void Run(IL2Client client);


        internal void LogNewDataWarning(string dataName, object expected, object data)
        {
            if (!expected.Equals(data))
            {

                baseLogger.Warning("NEW DATA INSIDE PACKET {0}: {1} Expected {2}", dataName, data, expected);
            }
        }
        public bool HasRemainingData()
        {
            return BaseStream.Position != BaseStream.Length;
        }

        public byte[] GetRemainingData()
        {
            return ReadBytes((int)(BaseStream.Length - BaseStream.Position));
        }



        public ushort ReadByteAsShort()
        {
            return Convert.ToUInt16(ReadByte());
        }

        public override string ReadString()
        {
            StringBuilder sb = new StringBuilder();
            char chr;

            while ((chr = ReadChar()) != char.MinValue)
            {
                sb.Append(chr);
            }
            return sb.ToString();
        }

        public byte[] ToArray()
        {
            return ((MemoryStream)BaseStream).ToArray();
        }

        public override byte[] ReadBytes(int count)
        {
            var readBytes = base.ReadBytes(count);
            if (readBytes.Length < count)
            {
                baseLogger.Error("Attempting to read {count} resulted in {readLen} bytes read. End of stream was reached.", count, readBytes.Length);
                baseLogger.Error("Full packet {packet}", BitConverter.ToString(ToArray()));
            }
            return readBytes;
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
