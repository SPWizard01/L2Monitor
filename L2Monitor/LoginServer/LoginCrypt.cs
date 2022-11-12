using L2Monitor.Classes;
using L2Monitor.Common;
using L2Monitor.Common.Packets;
using L2Monitor.Util;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace L2Monitor.LoginServer
{
    public class LoginCrypt : ICrypt
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

        private BlowfishEngine _crypt = new BlowfishEngine();
        private bool _keySet = false;
        private ILogger logger;

        public LoginCrypt()
        {
            STATIC_CRYPT.Init(false, new KeyParameter(STATIC_BLOWFISH_KEY));
            logger = Log.ForContext(GetType());
        }

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

        public void Decrypt(byte[] rawData, PacketDirection direction)
        {
            Decrypt(rawData, Constants.HEADER_SIZE, rawData.Length - Constants.HEADER_SIZE);

            //init will need to be unxored
            if(!_keySet)
            {
                DecXORPass(rawData);
            }
        }

        public void Decrypt(byte[] raw, int offset, int size)
        {
            //BouncyCastle uses big endian for everything so we need to reverse

            //NEW: assumption above might be incorrect, might be just custom stuff from login, game server does not exert this behaviour
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

        public bool Checksum(byte[] raw)
        {
            int chksum = 0;
            int count = raw.Length - 8;
            int ecx = 1; //avoids ecs beeing == chksum if an error occured in the try
            try
            {
                //header offset?
                var i = 2;
                for (; i < count; i += 4)
                {
                    ecx = BitConverter.ToInt32(raw, i);
                    chksum ^= ecx;
                }

                ecx = BitConverter.ToInt32(raw, i);

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
