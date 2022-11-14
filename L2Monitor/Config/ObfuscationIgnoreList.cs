using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Config
{
    public sealed class ObfuscationIgnoreList
    {
        public List<string> Op1 { get; set; } = new();
        public List<string> Op2 { get; set; } = new();

        public List<byte> GetIgnoredOp1Codes()
        {
            var retList = new List<byte>();
            foreach (var op in Op1)
            {
                try
                {
                    retList.Add(Convert.ToByte(op, 16));
                }
                catch
                {
                    throw new InvalidCastException($"Could not convert {op} to byte");
                }
            }
            return retList;
        }

        public List<ushort> GetIgnoredOp2Codes()
        {
            var retList = new List<ushort>();
            foreach (var op in Op2)
            {
                try
                {
                    retList.Add(Convert.ToUInt16(op, 16));
                }
                catch
                {
                    throw new InvalidCastException($"Could not convert {op} to byte");
                }
            }
            return retList;
        }
    }
}
