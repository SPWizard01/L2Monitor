using L2Monitor.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Classes
{
    public interface ICrypt
    {
        public bool KeySet { get; }
        public void SetKey(byte[] key);
        public void Decrypt(byte[] data, PacketDirection direction);
        public bool Checksum(byte[] data);
    }
}
