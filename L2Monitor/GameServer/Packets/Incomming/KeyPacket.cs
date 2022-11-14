using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class KeyPacket : BasePacket
    {
        public bool AuthSuccess { get; set; }
        public byte[] EncryptionKey { get; set; }
        public uint Unknown1 { get; set; }
        public uint ServerId { get; set; }
        public byte Unknown2 { get; set; }
        public uint ObfuscationKey { get; set; }
        public KeyPacket()
        {

        }

        public KeyPacket(MemoryStream raw, PacketDirection direction) : base(raw, false, direction)
        {


        }
        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new KeyPacket(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            var cl = (GameClient)client;
            AuthSuccess = ReadBoolean();
            EncryptionKey = ReadBytes(8);
            Unknown1 = ReadUInt32();
            ServerId = ReadUInt32();
            Unknown2 = ReadByte();
            ObfuscationKey = ReadUInt32();
            WarnOnRemainingData();
            cl.Crypt.SetKey(EncryptionKey);
            cl.Obfuscator.Init(ObfuscationKey);
            baseLogger.Information(JsonSerializer.Serialize(this));
        }

    }
}
