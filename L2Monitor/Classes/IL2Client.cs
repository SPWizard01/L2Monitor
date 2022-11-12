using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Classes
{
    public interface IL2Client
    {
        public ConnectionState State { get; set; }
        public ICrypt? Crypt { get; }
    }
}
