using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Serilog;
using System;

namespace L2Monitor.Common
{
    public abstract class L2Crypt
    {
        private static readonly byte[] STATIC_BLOWFISH_KEY = {
             0x6b,
             0x60,
             0xcb,
             0x5b,
             0x82,
             0xce,
             0x90,
             0xb1,
             0xcc,
             0x2b,
             0x6c,
             0x55,
             0x6c,
             0x6c,
             0x6c,
             0x6c
        };

        internal BlowfishEngine STATIC_CRYPT = new BlowfishEngine();
        internal readonly ILogger logger;
        public L2Crypt()
        {
            STATIC_CRYPT.Init(false, new KeyParameter(STATIC_BLOWFISH_KEY));
            logger = Log.ForContext(GetType());
        }

        public void StaticDecrypt(byte[] raw, int offset, int size)
        {
            //BouncyCastle uses big endian for everything so we need to reverse
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < (offset + size); i += 4)
                {
                    Array.Reverse(raw, i, 4);
                }
            }
            for (int i = offset; i < (offset + size); i += 8)
            {
                STATIC_CRYPT.ProcessBlock(raw, i, raw, i);
            }
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < (offset + size); i += 4)
                {
                    Array.Reverse(raw, i, 4);
                }
            }
        }

    }
}
