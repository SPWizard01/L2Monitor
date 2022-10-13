using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Packets.Incomming
{
    public class CryptInit : BasePacket
    {
        public bool AuthSuccess { get; set; }
        public byte[] EncryptionKey { get; set; }
        public uint Unknown1 { get; set; }
        public uint ServerId { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public CryptInit(MemoryStream raw) : base(raw)
        {

            AuthSuccess = readBool();
            EncryptionKey = readBytes(8);
            Unknown1 = readUInt();
            ServerId = readUInt();
            Unknown2 = readUInt();
            Unknown3 = readUInt();
            WarnOnRemainingData();
        }


    }
}
