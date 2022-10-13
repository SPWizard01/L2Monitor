using L2Monitor.Common.Packets;
using L2Monitor.Game.Packets.Incomming;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace L2Monitor.Game.Packets.Outgoing
{
    public class AuthLogin : BasePacket
    {
        public string Login { get; set; }
        public int PlayKey2 { get; set; }
        public int PlayKey1 { get; set; }
        public int LoginKey1 { get; set; }
        public int LoginKey2 { get; set; }
        public AuthLogin(MemoryStream raw) : base(raw)
        {
            Login = readString();
            PlayKey2 = readInt();
            PlayKey1 = readInt();
            LoginKey1 = readInt();
            LoginKey2 = readInt();
        }


    }
}
