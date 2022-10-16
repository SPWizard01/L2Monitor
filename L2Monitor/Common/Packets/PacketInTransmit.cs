using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Common.Packets
{
    public class PacketInTransmit
    {
        public byte[] PacketData { get; private set; }
        public ushort PackeLength { get; private set; }
        public ushort RemainingDataLength { get; private set; }
        public PacketInTransmit(byte[] packetData, ushort packetLength)
        {
            PacketData = packetData;
            PackeLength = packetLength;
            RemainingDataLength = (ushort)(packetData.Length - packetLength);
        }

        public PacketInTransmit(MemoryStream mem)
        {
            var packetLength = BitConverter.ToUInt16(new byte[] { (byte)mem.ReadByte(), (byte)mem.ReadByte() });
            //rewind the 2 bytes we got so that packet size is inside mem stream
            mem.Seek(-2, SeekOrigin.Current);
            //mem lenght can be more thant packetLength because it can contain multiple packets so we only read the amount we need;
            var remainingDataAmount = mem.Length - mem.Position;
            var readAmount = remainingDataAmount >= packetLength ? packetLength : remainingDataAmount;
            Log.Information("Read packet length {packetLength} read amount {readAmount}", packetLength, readAmount);

            //on the other hand we could initialize array with packet length
            //and then calc offsed during AddData
            PacketData = new byte[packetLength];
            mem.Read(PacketData, 0, (int)readAmount);
            PackeLength = packetLength;
            RemainingDataLength = (ushort)(PacketData.Length - readAmount);
        }

        public void AddData1(MemoryStream mem)
        {
            var bufferLen = mem.Length >= RemainingDataLength ? RemainingDataLength : mem.Length;
            var readData = new byte[bufferLen];
            mem.Read(readData, 0, (int)bufferLen);
            PacketData = PacketData.Concat(readData).ToArray();
            RemainingDataLength = (ushort)(PackeLength - PacketData.Length);
            if (RemainingDataLength < 0)
            {
                throw new InvalidDataException("Remaining data must be greater or equals to 0");
            }
        }

        public void AddData(MemoryStream mem)
        {
            var bufferLen = mem.Length >= RemainingDataLength ? RemainingDataLength : mem.Length;
            var offset = PackeLength - RemainingDataLength;
            mem.Read(PacketData, offset, (int)bufferLen);
            RemainingDataLength -= (ushort)bufferLen;
            if (RemainingDataLength < 0)
            {
                throw new InvalidDataException("Remaining data must be greater or equals to 0");
            }
        }
    }
}
