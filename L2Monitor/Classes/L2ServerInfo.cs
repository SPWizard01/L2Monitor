using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Classes
{
    public class L2ServerInfo
    {
        public ushort ServerId { get; set; }
        public IPAddress ServerIP { get; set; }
        public uint ServerPort { get; set; }
        public bool AgeRestricted { get; set; }
        public bool IsPvP { get; set; }
        public ushort OnlineCount { get; set; }
        public ushort MaxCount { get; set; }
        public bool IsOnline { get; set; }
        public bool Unknown1 { get; set; }
        public bool Unknown2 { get; set; }
        public bool Unknown3 { get; set; }
        public ushort Unknown4 { get; set; }
    }
}
