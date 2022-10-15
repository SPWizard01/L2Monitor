using L2Monitor.Common;
using L2Monitor.Common.Packets;
using L2Monitor.GameServer.Packets.Incomming;
using L2Monitor.Util;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace L2Monitor.GameServer
{
    public class GameCrypt : L2Crypt
    {

        private readonly byte[] StaticPart = new byte[]
        {
            0xC8,
            0x27,
            0x93,
            0x01,
            0xA1,
            0x6C,
            0x31,
            0x97
        };

        private byte[] serverToClientKey = new byte[16];
        private byte[] clientToServerKey = new byte[16];

        private BlowfishEngine _crypt = new BlowfishEngine();
        public GameCrypt(CryptInit cryptPacket)
        {
            if (cryptPacket.EncryptionKey.Length > 8)
            {
                logger.Error("Game key must be 8 bytes long, got: {len}", cryptPacket.EncryptionKey.Length);
            }
            cryptPacket.EncryptionKey.CopyTo(serverToClientKey, 0);
            StaticPart.CopyTo(serverToClientKey, 8);
            cryptPacket.EncryptionKey.CopyTo(clientToServerKey, 0);
            StaticPart.CopyTo(clientToServerKey, 8);
        }

        //public void EncryptToClient(byte[] encryptedData)
        //{
        //    // server uses to encrypt:
        //    //int tempValue = 0;
        //    //while (buf.isReadable())
        //    //{
        //    //    final int thisByteInt = buf.readByte() & 0xFF;
        //    //    var keyIndex = buf.readerIndex();
        //    //    var keyAddress = (keyIndex - 1) & 15;
        //    //    tempValue = thisByteInt ^ _outKey[keyAddress] ^ tempValue;
        //    //    buf.setByte(buf.readerIndex() - 1, tempValue);
        //    //}
        //}

        public void Decrypt(byte[] raw, int offset, int size, PacketDirection direction)
        {

            var useKey = direction == PacketDirection.ServerToClient ? serverToClientKey : clientToServerKey;
            uint temp = 0;
            for (int i = 0; i < size - offset; i++)
            {
                uint temp2 = (uint)raw[offset + i] & 0xFF;
                raw[offset + i] = (byte)(temp2 ^ useKey[i & 15] ^ temp);
                temp = temp2;
            }

            shftKey(offset, size, useKey);

        }

        private static void shftKey(int offset, int size, byte[] useKey)
        {
            //SHIFT KEY PART
            //uint old = useKey[8] & (uint)0xff;
            //old |= (uint)useKey[9] << 8 & 0xff00; 
            //old |= (uint)useKey[10] << 0x10 & (uint)0xff0000;
            //old |= (uint)useKey[11] << 0x18 & 0xff000000;
            
            var old = BitConverter.ToUInt32(useKey, 8);
            //var gg = BitConverter.ToInt32(useKey, 8);
            //if (gg != old)
            //{
            //    Debugger.Break();
            //}
            old += (uint)(size - offset); // FUCKING BUG!! min. 24h waste of time!!! =(
            //old += (uint)(size);
            BitConverter.GetBytes(old).CopyTo(useKey, 8);
            //useKey[8] = (byte)(old & 0xff);
            //useKey[9] = (byte)(old >> 0x08 & 0xff);
            //useKey[10] = (byte)(old >> 0x10 & 0xff);
            //useKey[11] = (byte)(old >> 0x18 & 0xff);
        }

        public void DecryptServerTest(byte[] encryptedData, PacketDirection direction)
        {
            //header(payload size) is never encrypted
            var dataLen = encryptedData.Length - Constants.HEADER_SIZE;
            var _decryptedData = new byte[dataLen];
            Array.Copy(encryptedData, Constants.HEADER_SIZE, _decryptedData, 0, dataLen);

            var usedKey = direction == PacketDirection.ServerToClient ? serverToClientKey : clientToServerKey;
            var tempKey = 0;

            for (var i = 0; i < _decryptedData.Length; i++)
            {
                //from 1 byte to int representation
                var thisByteInt = _decryptedData[i] & 0xFF;
                var keyAddress = i & 15;
                int keyAssignedValue = thisByteInt ^ usedKey[keyAddress] ^ tempKey;

                _decryptedData[i] = (byte)keyAssignedValue;
                tempKey = thisByteInt;
            }
            var keyToShift = direction == PacketDirection.ServerToClient ? serverToClientKey : clientToServerKey;
            shiftKey(keyToShift, dataLen);
            _decryptedData.CopyTo(encryptedData, Constants.HEADER_SIZE);
        }

        private void shiftKey(byte[] key, int size)
        {

            uint old = ((uint)key[8]) & (uint)0xff;
            old |= (uint)(((uint)key[9]) << 8 & (uint)0xff00);
            old |= (uint)(((uint)key[10] << 0x10) & (uint)0xff0000);
            old |= (uint)(((uint)key[11] << 0x18) & (uint)0xff000000);

            old += (uint)size;

            key[8] = (byte)(old & 0xff);
            key[9] = (byte)(old >> 0x08 & 0xff);
            key[10] = (byte)(old >> 0x10 & 0xff);
            key[11] = (byte)(old >> 0x18 & 0xff);

            //long old = key[8] & 0xff;
            //old |= (key[9] << 8) & 0xff00;
            //old |= (key[10] << 0x10) & 0xff0000;
            //old |= (key[11] << 0x18) & 0xff000000;
            //var readInt = BitConverter.ToInt32(key, 8);

            //if(old != readInt)
            //{
            //    //this is a test to see if we can just use bitconverter;
            //    Logger.Error($"shiftKey values did not match old:{old}, readInt:{readInt}");
            //    var a = 1;
            //}
            //old += size;
            //readInt += size;
            //if (old != readInt)
            //{
            //    //this is a test to see if we can just use bitconverter;
            //    Logger.Error($"shiftKey values did not match after resize old:{old}, readInt:{readInt}");
            //    var b = 1;
            //}

            //key[8] = (byte)(old & 0xff);
            //key[9] = (byte)((old >> 0x08) & 0xff);
            //key[10] = (byte)((old >> 0x10) & 0xff);
            //key[11] = (byte)((old >> 0x18) & 0xff);
        }
    }
}
