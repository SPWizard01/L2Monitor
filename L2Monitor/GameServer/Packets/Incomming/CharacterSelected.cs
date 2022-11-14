using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace L2Monitor.GameServer.Packets.Incomming
{
    public class CharacterSelected : BasePacket
    {
        public string CharName { get; set; }
        public uint ObjectId { get; set; }
        public string Title { get; set; }
        public uint SessionId { get; set; }
        public uint ClanId { get; set; }
        public uint Unknown1 { get; set; }
        public int IsFemale { get; set; }
        public uint RaceId { get; set; }
        public uint ClassId { get; set; }
        public uint Unknown2 { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public double CurrentHp { get; set; }
        public double CurrentMp { get; set; }
        public long CurrentSP { get; set; }
        public long CurrentXP { get; set; }
        public int Level { get; set; }
        public int Reputation { get; set; }
        public int PKKills { get; set; }
        public int GameTime { get; set; }
        public int Unknown3 { get; set; }
        public int ClassId2 { get; set; }
        /// <summary>
        /// 16 non 0 bytes, unknown use
        /// </summary>
        public byte[] Unknown4 { get; set; }
        /// <summary>
        /// 36 bytes, should all be 0
        /// </summary>
        public byte[] Unknown5 { get; set; }
        /// <summary>
        /// 28 bytes, should all be 0
        /// </summary>
        public byte[] Unknown6 { get; set; }

        public uint ObfuscationKey { get; set; }

        public CharacterSelected()
        {
        }

        public CharacterSelected(MemoryStream stream, PacketDirection direction) : base(stream, false, direction)
        {
        }

        public override IBasePacket Factory(byte[] raw, PacketDirection direction)
        {
            return new CharacterSelected(new MemoryStream(raw), direction);
        }

        public override void Run(IL2Client client)
        {
            CharName = ReadString();
            ObjectId = ReadUInt32();
            Title = ReadString();
            SessionId = ReadUInt32();
            ClanId = ReadUInt32();
            Unknown1 = ReadUInt32();
            if (Unknown1 != 0)
            {
                LogNewDataWarning(nameof(Unknown1), 0, Unknown1);
            }
            IsFemale = ReadInt32();
            RaceId = ReadUInt32();
            ClassId = ReadUInt32();
            Unknown2 = ReadUInt32();
            if (Unknown2 != 1)
            {
                LogNewDataWarning(nameof(Unknown2), 1, Unknown2);
            }
            X = ReadInt32();
            Y = ReadInt32();
            Z = ReadInt32();
            CurrentHp = ReadDouble();
            CurrentMp = ReadDouble();
            CurrentSP = ReadInt64();
            CurrentXP = ReadInt64();
            Level = ReadInt32();
            Reputation = ReadInt32();
            PKKills = ReadInt32();
            GameTime = ReadInt32();
            Unknown3 = ReadInt32();
            if (Unknown3 != 0)
            {
                LogNewDataWarning(nameof(Unknown3), 0, Unknown3);
            }
            ClassId2 = ReadInt32();
            Unknown4 = ReadBytes(16);
            Unknown5 = ReadBytes(36);
            Unknown6 = ReadBytes(28);
            ObfuscationKey = ReadUInt32();
            
            var cl = (GameClient)client;
            cl.Obfuscator.Init(ObfuscationKey);

            baseLogger.Information(JsonSerializer.Serialize(this));
        }
    }
}
