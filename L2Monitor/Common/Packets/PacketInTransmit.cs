using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Common.Packets
{
    public class PacketInTransmit
    {
        public byte[] PacketData { get; private set; }
        public ushort PacketLength { get; private set; }
        public ushort RemainingDataLength { get; private set; }
        public bool IsPartial { get; private set; } = false;
        private readonly ILogger logger;

        public PacketInTransmit(MemoryStream mem)
        {
            logger = Log.ForContext(GetType());
            var packetLength = BitConverter.ToUInt16(new byte[] { (byte)mem.ReadByte(), (byte)mem.ReadByte() });
            //rewind the 2 bytes we got so that packet size is inside mem stream
            mem.Seek(-2, SeekOrigin.Current);
            //mem length can be more than packetLength because it can contain multiple packets so we only read the amount we need;
            var remainingDataAmount = mem.Length - mem.Position;
            var needToReadAmount = remainingDataAmount >= packetLength ? packetLength : remainingDataAmount;
            //logger.Information("Read packet length {packetLength} read amount {readAmount}", packetLength, readAmount);

            //on the other hand we could initialize array with packet length
            //and then calc offsed during AddData
            PacketData = new byte[packetLength];
            mem.Read(PacketData, 0, (int)needToReadAmount);
            PacketLength = packetLength;
            RemainingDataLength = (ushort)(PacketData.Length - needToReadAmount);
        }

        public PacketInTransmit(byte partialByte)
        {
            logger = Log.ForContext(GetType());
            IsPartial = true;
            PacketData = new byte[2] { partialByte, 0 };
            PacketLength = ushort.MaxValue;
            RemainingDataLength = ushort.MaxValue - 1;
            logger.Warning("Partial packet, 1st byte: {0}", partialByte);
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
                //read 1 byte as we already have 1 in memory
                var secondByte = (byte)mem.ReadByte();
                logger.Warning("Reconstructing, First byte: {0}", PacketData[0]);
                logger.Warning("Reconstructing, Second byte: {0}", secondByte);
                PacketData[1] = secondByte;
                var packetLength = BitConverter.ToUInt16(PacketData, 0);
                logger.Warning("Reconstructing, Packet length", packetLength);
                PacketLength = packetLength;
                PacketData = new byte[packetLength];
                Array.Copy(BitConverter.GetBytes(packetLength), PacketData, 2);
                RemainingDataLength = (ushort)(packetLength - 2);
                IsPartial = false;
            }

            var bufferLen = mem.Length >= RemainingDataLength ? RemainingDataLength : mem.Length;
            var offset = PacketLength - RemainingDataLength;
            mem.Read(PacketData, offset, (int)bufferLen);
            RemainingDataLength -= (ushort)bufferLen;
            if (RemainingDataLength < 0)
            {
                throw new InvalidDataException("Remaining data must be greater or equals to 0");
            }
        }
    }
}
