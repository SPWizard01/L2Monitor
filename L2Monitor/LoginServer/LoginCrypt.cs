using L2Monitor.Common;
using L2Monitor.Util;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.LoginServer
{
    public class LoginCrypt : L2Crypt
    {

        private BlowfishEngine _crypt = new BlowfishEngine();
        private bool _keySet = false;

        public void SetKey(byte[] initPacketBlowfishKey)
        {
            if (_keySet)
            {
                logger.Error("Encryption key is already set");
                return;
            }
            if (initPacketBlowfishKey.Length != 16)
            {
                logger.Error("Blowfish key has to be 16 bytes long, got key {len} long.", initPacketBlowfishKey.Length);
            }
            _keySet = true;
            _crypt.Init(false, new KeyParameter(initPacketBlowfishKey));
        }

        public void Decrypt(byte[] rawData)
        {
            Decrypt(rawData, Constants.HEADER_SIZE, rawData.Length - Constants.HEADER_SIZE);
        }

        public void Decrypt(byte[] raw, int offset, int size)
        {
            //BouncyCastle uses big endian for everything so we need to reverse
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < offset + size; i += 4)
                {
                    Array.Reverse(raw, i, 4);
                }
            }

            var cryptToUse = _keySet ? _crypt : STATIC_CRYPT;

            for (int i = offset; i < offset + size; i += 8)
            {
                cryptToUse.ProcessBlock(raw, i, raw, i);
            }
            if (BitConverter.IsLittleEndian)
            {
                for (int i = offset; i < offset + size; i += 4)
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

        public void javaDecXORPass(byte[] raw)
        {
            int count = raw.Length / 4;
            int pos = (count - 1) * 4;
            int ecx;

            ecx = (raw[--pos] & 0xFF) << 24;
            ecx |= (raw[--pos] & 0xFF) << 16;
            ecx |= (raw[--pos] & 0xFF) << 8;
            ecx |= raw[--pos] & 0xFF;

            int val;
            while (pos > 4)
            {
                raw[--pos] ^= (byte)(ecx >> 24);
                val = (raw[pos] & 0xFF) << 24;
                raw[--pos] ^= (byte)(ecx >> 16);
                val += (raw[pos] & 0xFF) << 16;
                raw[--pos] ^= (byte)(ecx >> 8);
                val += (raw[pos] & 0xFF) << 8;
                raw[--pos] ^= (byte)ecx;
                val += raw[pos] & 0xFF;

                ecx = ecx - val;
            }
        }

        public bool JavaChecksum(byte[] raw)
        {
            long chksum = 0;
            int count = raw.Length - 4;
            long ecx = -1; //avoids ecs beeing == chksum if an error occured in the try
            int i = 0;
            try
            {
                for (i = 0; i < count; i += 4)
                {
                    ecx = raw[i] & 0xff;
                    ecx |= raw[i + 1] << 8 & 0xff00;
                    ecx |= raw[i + 2] << 0x10 & 0xff0000;
                    ecx |= raw[i + 3] << 0x18 & 0xff000000;

                    chksum ^= ecx;
                }

                ecx = raw[i] & 0xff;
                ecx |= raw[i + 1] << 8 & 0xff00;
                ecx |= raw[i + 2] << 0x10 & 0xff0000;
                ecx |= raw[i + 3] << 0x18 & 0xff000000;

            }
            catch (Exception e)
            {
                logger.Error("Error validating checksum");
                //Looks like this will only happen on incoming packets as outgoing ones are padded
                //and the error can only happen in last raw[i] =, raw [i+1] = ... and it doesnt really matters for incomming packets
            }

            return ecx == chksum;
        }

        public bool Checksum(byte[] raw)
        {
            uint chksum = 0;
            int count = raw.Length - 4;
            uint ecx = 1; //avoids ecs beeing == chksum if an error occured in the try
            try
            {
                var i = 0;
                for (; i < count; i += 4)
                {
                    ecx = BitConverter.ToUInt32(raw, i);
                    chksum ^= ecx;
                }

                //ecx = BitConverter.ToUInt32(raw, i);

            }
            catch (Exception e)
            {
                logger.Error("Error validating checksum");
                //Looks like this will only happen on incoming packets as outgoing ones are padded
                //and the error can only happen in last raw[i] =, raw [i+1] = ... and it doesnt really matters for incomming packets
            }

            return ecx == chksum;
        }
    }
}
