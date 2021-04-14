using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Packets.Incomming
{
    public class KeyPacket : BasePacket
    {
        ILogger logger;
        public bool AuthSuccess { get; set; }
        public byte[] EncryptionKey { get; set; }
        public int Unknown1 { get; set; }
        public int ServerId { get; set; }
        public byte Unknown2 { get; set; }
        public int Unknown3 { get; set; }
        public bool IsClassic { get; set; }
        public KeyPacket(MemoryStream raw) : base(raw)
        {
            logger = Log.ForContext<KeyPacket>();

            AuthSuccess = readBool();
            EncryptionKey = readBytes(8);
            Unknown1 = readInt();
            ServerId = readInt();
            Unknown2 = readByte();
            Unknown3 = readInt();
            IsClassic = readBool();
            if (HasRemainingData())
            {
                var data = GetRemainingData();
                logger.Warning($"This packet has remaining data: {BitConverter.ToString(data)}");
            }
        }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
