using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.Login
{
    public class LoginCrypt
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

        private BlowfishEngine STATIC_CRYPT = new BlowfishEngine();
        private BlowfishEngine _crypt = new BlowfishEngine();
        private KeyParameter _decryptKey;
        public bool IsInitial { get; private set; }
        public LoginCrypt()
        {
            IsInitial = true;
            KeyParameter staticKey = new KeyParameter(STATIC_BLOWFISH_KEY);
            STATIC_CRYPT.Init(false, staticKey);
        }

        public void SetKey(byte[] BlowFishKey)
        {
            _decryptKey = new KeyParameter(BlowFishKey);
            _crypt.Init(false, _decryptKey);
        }

        public void Decrypt(byte[] raw, int offset, int size)
        {
            //BouncyCastle uses big endian for everything so we need to reverse
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < (offset + size); i += 4)
                {
                    Array.Reverse(raw, i, 4);
                }
            }

            var cryptToUse = IsInitial ? STATIC_CRYPT : _crypt;

            for (int i = offset; i < (offset + size); i += 8)
            {
                cryptToUse.ProcessBlock(raw, i, raw, i);
            }
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < (offset + size); i += 4)
                {
                    Array.Reverse(raw, i, 4);
                }
            }
        }

        
        public void DecXORPass(byte[] raw)
        {
            //2 byte header
            //first 4 bytes are not xored

            int stop = 6;

            //last 4 bytes are empty after decryption so -4
            //4 bytes after that is initial key so another -4
            int pos = raw.Length - 8;
            int edx;
            // last key 
            int ecx = BitConverter.ToInt32(raw, pos);
            Array.Copy(BitConverter.GetBytes(0), 0, raw, pos, 4);
            pos -= 4;

            while (stop <= pos)
            {
                edx = BitConverter.ToInt32(raw, pos);

                edx ^= ecx;

                ecx -= edx;

                var edxBytes = BitConverter.GetBytes(edx);
                Array.Copy(edxBytes, 0, raw, pos, edxBytes.Length);
                pos -= 4;
            }
        }
    }
}
