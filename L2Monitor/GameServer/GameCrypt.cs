using L2Monitor.Classes;
using L2Monitor.Common.Packets;
using L2Monitor.Util;
using Serilog;
using System;

namespace L2Monitor.GameServer
{
    public class GameCrypt : ICrypt
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

        private bool _keySet = false;
        private ILogger logger;
        public GameCrypt()
        {
            logger = Log.ForContext(GetType());
        }

        public void SetKey(byte[] key)
        {
            if (key.Length > 8)
            {
                logger.Error("Game key must be 8 bytes long, got: {len}", key.Length);
            }
            key.CopyTo(serverToClientKey, 0);
            StaticPart.CopyTo(serverToClientKey, 8);
            key.CopyTo(clientToServerKey, 0);
            StaticPart.CopyTo(clientToServerKey, 8);
            _keySet = true;
        }

        public void Decrypt(byte[] raw, PacketDirection direction)
        {
            Decrypt(raw, Constants.HEADER_SIZE, raw.Length, direction);

        }

        public void Decrypt(byte[] raw, int offset, int size, PacketDirection direction)
        {
            if (!_keySet)
            {
                return;
            }
            var useKey = direction == PacketDirection.ServerToClient ? serverToClientKey : clientToServerKey;
            uint temp = 0;
            for (int i = 0; i < size - offset; i++)
            {
                uint temp2 = (uint)raw[offset + i] & 0xFF;
                raw[offset + i] = (byte)(temp2 ^ useKey[i & 15] ^ temp);
                temp = temp2;
            }

            shiftKey(offset, size, useKey);

        }

        private void shiftKey(int offset, int size, byte[] useKey)
        {
            //SHIFT KEY PART
            var old = BitConverter.ToInt32(useKey, 8);
            old += (size - offset); // FUCKING BUG!! min. 24h waste of time!!! =(
            //old += (uint)(size);
            BitConverter.GetBytes(old).CopyTo(useKey, 8);

        }

        public bool Checksum(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
