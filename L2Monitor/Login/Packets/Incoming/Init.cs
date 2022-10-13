using L2Monitor.Common.Packets;
using L2Monitor.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Login
{
    public class Init : BasePacket
    {
        public uint SessionId { get; private set; }
        public uint ProtocolVersion { get; private set; }
        public byte[] RSAPublicKey { get; private set; }
        public uint Unknown1 { get; private set; }
        public int Unknown2 { get; private set; }
        public int Unknown3 { get; private set; }
        public int Unknown4 { get; private set; }
        public int Unknown5 { get; private set; }
        public byte[] BlowFishKey { get; private set; }
        public int Unknown6 { get; private set; }
        public byte[] UnknownBytes { get; private set; }
        public int Unknown7 { get; private set; }
        public uint Unknown8 { get; private set; }

        public Init(MemoryStream raw) : base(raw)
        {
            SessionId = readUInt();
            
            ProtocolVersion = readUInt();
            if (ProtocolVersion != 50721)
            {
                LogNewDataWarning(nameof(ProtocolVersion), ProtocolVersion);
            }
            
            RSAPublicKey = readBytes(128);

            //GG related, 0 in new client
            Unknown2 = readInt();
            Unknown3 = readInt();
            Unknown4 = readInt();
            Unknown5 = readInt();

            BlowFishKey = readBytes(16);
            Unknown6 = readInt();
            if (Unknown6 != 133123)
            {
                LogNewDataWarning(nameof(Unknown6), Unknown6);
            }
            UnknownBytes = readBytes(11);
            Unknown7 = readInt();
            if (Unknown7 != 0)
            {
                LogNewDataWarning(nameof(Unknown7), Unknown7);
            }
            Unknown8 = readUInt();
            WarnOnRemainingData();
        }

    }
}
