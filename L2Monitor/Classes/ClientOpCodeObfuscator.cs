using L2Monitor.Config;
using L2Monitor.Util;
using Serilog;
using System;

namespace L2Monitor.Classes
{
    public class ClientOpCodeObfuscator
    {
        private readonly ILogger logger;
        private byte[] _decodeTable1;
        private byte[] _encodeTable1;
        private ushort[] _decodeTable2;
        private ushort[] _encodeTable2;
        public bool Inited { get; private set; } = false;
        private readonly AppSettings appSettings;

        public ClientOpCodeObfuscator(AppSettings appSettingsinj)
        {

            logger = Log.ForContext(GetType());
            appSettings = appSettingsinj;
        }
        public void Init(uint seed)
        {
            if (Inited)
            {
                logger.Error("Obfuscator has already been inited, this indicates that there was probably an error in the stream...");
                return;
            }

            Inited = true;
            //208+1
            _decodeTable1 = new byte[appSettings.MaxOp1Code + 1];
            //yet to be found
            _decodeTable2 = new ushort[appSettings.MaxOp2Code + 1];
            _encodeTable1 = new byte[_decodeTable1.Length];
            _encodeTable2 = new ushort[_decodeTable2.Length];

            logger.Information("Initialising obfuscation with {0}", seed);
            for (var i = 0; i < _decodeTable1.Length; ++i)
            {
                _decodeTable1[i] = (byte)i;
            }
            for (var i = 0; i < _decodeTable2.Length; ++i)
            {
                _decodeTable2[i] = (ushort)i;
            }

            if (seed != 0)
            {
                var stateValue = seed;
                //the 16th idx holds the state machine
                var state = new uint[17];
                for (var i = 0; i < 16; ++i)
                {
                    state[i] = stateValue;
                    stateValue = (stateValue << 1) + 51;
                }
                uint srcIdx = 1;
                do
                {
                    uint tmpIndex2 = ShiftRegister(state);
                    var destIdx = tmpIndex2 % (srcIdx + 1);
                    var sourceValue = _decodeTable1[srcIdx];
                    _decodeTable1[srcIdx] = _decodeTable1[destIdx];
                    _decodeTable1[destIdx] = sourceValue;
                } while (++srcIdx < _decodeTable1.Length);


                srcIdx = 1;
                do
                {
                    uint tmpIndex2 = ShiftRegister(state);
                    var destIdx = tmpIndex2 % (srcIdx + 1);
                    var sourceValue = _decodeTable2[srcIdx];
                    _decodeTable2[srcIdx] = _decodeTable2[destIdx];
                    _decodeTable2[destIdx] = sourceValue;
                } while (++srcIdx < _decodeTable2.Length);

                var ignoredOp1Codes = appSettings.ObfuscationIgnoreList.GetIgnoredOp1Codes();
                foreach (var op1 in ignoredOp1Codes)
                {
                    var idx = Array.IndexOf(_decodeTable1, op1);
                    var t = _decodeTable1[op1];
                    _decodeTable1[op1] = op1;
                    _decodeTable1[idx] = t;
                }

                var ignoredOp2Codes = appSettings.ObfuscationIgnoreList.GetIgnoredOp2Codes();
                foreach (var op2 in ignoredOp2Codes)
                {
                    var idx = Array.IndexOf(_decodeTable2, op2);
                    var t = _decodeTable2[op2];
                    _decodeTable2[op2] = op2;
                    _decodeTable2[idx] = t;
                }


                for (var i = 0; i < _encodeTable1.Length; ++i)
                {
                    var idx17 = _decodeTable1[i];
                    _encodeTable1[idx17] = (byte)i;
                }
                for (var i = 0; i < _encodeTable2.Length; ++i)
                {
                    var idx17 = _decodeTable2[i];
                    _encodeTable2[idx17] = (ushort)i;
                }
            }
        }

        public byte GetDecodedOp1Code(byte encoded)
        {
            if (!Inited)
            {
                logger.Error("Obfuscation has not been initialized");
                return encoded;
            }
            if (encoded > _decodeTable1.Length)
            {
                logger.Error("Increase Op1 table size to at least {0}", encoded);
                return encoded;
            }
            return _decodeTable1[encoded];
        }

        public ushort GetDecodedOp2Code(ushort encoded)
        {
            if (!Inited)
            {
                logger.Error("Obfuscation has not been initialized");
                return encoded;
            }
            if (encoded > _decodeTable2.Length)
            {
                logger.Error("Increase Op2 table size to at least {0}", encoded);
                return encoded;
            }
            return _decodeTable2[encoded];
        }

        public void DecodedOpCode(byte[] raw, int offset)
        {
            if (!Inited)
            {
                logger.Error("Obfuscation has not been initialized");
                return;
            }
            var id1 = GetDecodedOp1Code(raw[offset]);
            raw[offset] = id1;
            if (id1 == Constants.MAX_OUTGOING)
            {
                var id2 = BitConverter.ToUInt16(raw, offset + 1);
                var id2Decoded = GetDecodedOp2Code(id2);
                Array.Copy(BitConverter.GetBytes(id2Decoded), 0, raw, 3, 2);
            }
        }

        //is this LFSR???
        private static uint ShiftRegister(uint[] derived)
        {
            var v3 = derived[16];
            var v3_1 = v3 - 3 & 0xF;
            var v4 = derived[v3_1];

            var v5 = derived[v3] ^ v4 ^ (v4 ^ (derived[v3] << 1)) << 15;
            var v3_2 = v3 - 7 & 0xF;
            var tmpForV6 = derived[v3_2];
            var v6 = tmpForV6 >> 11 ^ tmpForV6;
            derived[v3] = v6 ^ v5;
            var v7 = (v3 - 1) & 0xF;
            derived[16] = v7;
            var currentValue = derived[v7];
            derived[v7] = (uint)(currentValue ^ (v5 ^ v6 ^ v5 ^ ((v6 ^ v5) & 0xFED22169L) << 5 ^ (derived[v7] ^ (v5 ^ v6 << 10) << 16) << 2));

            var tmpIndex2 = derived[v7];
            return tmpIndex2;
        }
    }
}
