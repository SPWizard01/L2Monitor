using Serilog;
using System;
using System.IO;
using System.Linq;

namespace L2Monitor.Common.Packets
{
    public sealed class PacketInTransmit
    {
        public byte[] PacketData { get; private set; }
        public ushort PacketLength { get; private set; }
        public ushort RemainingDataLength { get; private set; }
        public bool IsPartial { get; private set; } = false;
        private readonly ILogger logger;

        public PacketInTransmit(MemoryStream mem)
        {
            logger = Log.ForContext(GetType());
            var firstByte = (byte)mem.ReadByte();
            var secondByte = (byte)mem.ReadByte();
            var packetLength = BitConverter.ToUInt16(new byte[] { firstByte, secondByte });

            PacketData = new byte[packetLength];
            PacketLength = packetLength; //100
            PacketData[0] = firstByte;
            PacketData[1] = secondByte;
            //since 2 bytes are size
            RemainingDataLength = (ushort)(packetLength - 2); //98

            //mem length can be more than packetLength because it can contain multiple packets so we only read the amount we need;
            var remainingDataInMemory = mem.Length - mem.Position;
            //we read 2 bytes previously
            var shouldReadAmount = remainingDataInMemory >= RemainingDataLength ? RemainingDataLength : remainingDataInMemory;
            var amountRead = mem.Read(PacketData, 2, (int)shouldReadAmount);
            RemainingDataLength -= (ushort)amountRead;
        }

        public PacketInTransmit(byte firstByte)
        {
            logger = Log.ForContext(GetType());
            IsPartial = true;
            PacketData = new byte[2] { firstByte, 0 };
            PacketLength = ushort.MaxValue;
            RemainingDataLength = ushort.MaxValue - 1;
            logger.Warning("Partial packet, 1st byte: {0}", firstByte);
        }

        public void AddData1(MemoryStream mem)
        {
            var bufferLen = mem.Length >= RemainingDataLength ? RemainingDataLength : mem.Length;
            var readData = new byte[bufferLen];
            mem.Read(readData, 0, (int)bufferLen);
            PacketData = PacketData.Concat(readData).ToArray();
            RemainingDataLength = (ushort)(PacketLength - PacketData.Length);
            if (RemainingDataLength < 0)
            {
                throw new InvalidDataException("Remaining data must be greater or equals to 0");
            }
        }

        public void AddData(MemoryStream mem)
        {
            if (IsPartial)
            {
                var firstByte = PacketData[0];
                //read 1 byte as we already have 1 in memory
                var secondByte = (byte)mem.ReadByte();
                logger.Warning("Reconstructing, First byte was: {0}", firstByte);
                logger.Warning("Reconstructing, Second byte is: {0}", secondByte);
                PacketData[1] = secondByte;
                var packetLength = BitConverter.ToUInt16(PacketData, 0);
                logger.Warning("Reconstructing, Packet length {0}", packetLength);

                PacketLength = packetLength;
                RemainingDataLength = (ushort)(packetLength - 2);
                PacketData = new byte[packetLength];
                PacketData[0] = firstByte;
                PacketData[1] = secondByte;
                IsPartial = false;
            }

            var toReadAmount = mem.Length >= RemainingDataLength ? RemainingDataLength : mem.Length;
            var offset = PacketLength - RemainingDataLength;
            mem.Read(PacketData, offset, (int)toReadAmount);
            RemainingDataLength -= (ushort)toReadAmount;
            if (RemainingDataLength < 0)
            {
                throw new InvalidDataException("Remaining data must be greater or equals to 0");
            }
        }
    }
}
