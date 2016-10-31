using System;

namespace CryptoSQLite.CryptoProviders
{
    internal class GostExternalCryptoProvider : ICryptoProvider, IDisposable
    {
        private byte[] _key;
        private uint[] _solt;
        private readonly Gost28147 _gost;
        public GostExternalCryptoProvider()
        {
            _solt = new uint[2];
            _gost = new Gost28147();
        }

        private byte[] GetGamma(int count)
        {
            if(count < 1)
                throw new ArgumentException();

            var takts = count/8;
            if (count%8 > 0)
                takts += 1;

            var gamma = new byte[takts*8];
            var tmp = new uint[2];
            for (var t = 0; t < takts; t++)
            {
                _gost.GostSimpleReplacement(_solt, tmp);    // get gamma from gost
                _solt[0] ^= tmp[0];
                _solt[1] ^= tmp[1];

                gamma[8 * t] = BitConverter.GetBytes(tmp[0])[0];
                gamma[8 * t + 1] = BitConverter.GetBytes(tmp[0])[1];
                gamma[8 * t + 2] = BitConverter.GetBytes(tmp[0])[2];
                gamma[8 * t + 3] = BitConverter.GetBytes(tmp[0])[3];
                gamma[8 * t + 4] = BitConverter.GetBytes(tmp[1])[0];
                gamma[8 * t + 5] = BitConverter.GetBytes(tmp[1])[1];
                gamma[8 * t + 6] = BitConverter.GetBytes(tmp[1])[2];
                gamma[8 * t + 7] = BitConverter.GetBytes(tmp[1])[3];

                tmp.ZeroMemory();
            }
            return gamma;
        }

        public byte[] Encrypt(byte[] openData)
        {
            if(_key == null)
                throw new NullReferenceException("Encryption key has not been installed");
            if(_solt == null)
                throw new NullReferenceException("Solt has not been installed");

            var closedData = new byte[openData.Length];
            var gamma = GetGamma(openData.Length);
            for (var i = 0; i < openData.Length; i++)
            {
                closedData[i] = (byte)(openData[i] ^ gamma[i]);
            }

            for (var i = 0; i < gamma.Length; i++)      // cleaning is important
                gamma[i] = 0;

            return closedData;
        }

        public byte[] Decrypt(byte[] closedData)
        {
            return Encrypt(closedData); // gamma-cyphers works that way
        }

        public void SetKey(byte[] key)
        {
            if(key == null)
                throw new ArgumentNullException(nameof(key));

            _key = key;

            _gost.InitKey(key);
        }

        public void SetSolt(byte[] solt)
        {
            if(solt == null)
                throw new ArgumentNullException(nameof(solt));
            if(solt.Length < 8)
                throw new ArgumentException("Solt must contain at least 8 baytes");

            _solt = new uint[2];
            _solt[0] = BitConverter.ToUInt32(solt, 0);
            _solt[1] = BitConverter.ToUInt32(solt, 4);
        }

        public void Dispose()
        {
            _key?.ZeroMemory(); // TODO think about it.
            _gost.Dispose();
        }
    }


    #region Implementation GOST
    internal enum CryptoMode
    {
        Encrypt = 3,    
        Decrypt         
    }
    internal class Gost28147 : IDisposable
    {
        private readonly uint[,] _table =
        {
            {0xF, 0xB, 0x2, 0xC, 0x4, 0x6, 0xE, 0x9, 0x1, 0xA, 0x3, 0x5, 0xD, 0x0, 0x8, 0x7},
            {0xA0, 0x90, 0x30, 0x40, 0xE0, 0x00, 0xD0, 0x20, 0x50, 0x80, 0xF0, 0x60, 0x10, 0x70, 0xC0, 0xB0},
            {0xF00, 0x300, 0x900, 0x400, 0x200, 0xC00, 0x700, 0x500, 0x800, 0xD00, 0x600, 0xA00, 0x100, 0xE00, 0xB00, 0x000},
            {0xC000, 0xD000, 0x6000, 0xA000, 0x7000, 0xB000, 0x9000, 0x4000, 0x8000, 0x2000, 0xF000, 0x5000, 0x1000, 0xE000, 0x0000, 0x3000},
            {0xE0000, 0xB0000, 0x90000, 0x40000, 0x80000, 0x60000, 0x70000, 0x20000, 0x30000, 0x50000, 0x00000, 0xF0000, 0xA0000, 0x10000, 0xD0000, 0xC0000},
            {0x300000, 0xA00000, 0xC00000, 0x000000, 0x400000, 0x900000, 0x100000, 0xF00000, 0x800000, 0x600000, 0xB00000, 0x500000, 0xE00000, 0xD00000, 0x700000, 0x200000},
            {0xA000000, 0x5000000, 0x3000000, 0x8000000, 0x4000000, 0x9000000, 0xE000000, 0xB000000, 0x1000000, 0x6000000, 0xD000000, 0xC000000, 0x7000000, 0xF000000, 0x0000000, 0x2000000},
            {0xB0000000, 0x50000000, 0x20000000, 0xE0000000, 0x80000000, 0x10000000, 0xC0000000, 0x30000000, 0x40000000, 0xF0000000, 0xD0000000, 0xA0000000, 0x00000000, 0x60000000, 0x70000000, 0x90000000}
        };

        private readonly uint[] _roundKeysForEncrypt =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0
        };

        private uint _n1, _n2;


        private void GostTakt(uint key)
        {
            var a = _n1 + key;
            var b = _n1;

            _n1 = _table[0, a & 0x0F];
            a >>= 4;
            _n1 |= _table[1, a & 0x0F];
            a >>= 4;
            _n1 |= _table[2, a & 0x0F];
            a >>= 4;
            _n1 |= _table[3, a & 0x0F];
            a >>= 4;
            _n1 |= _table[4, a & 0x0F];
            a >>= 4;
            _n1 |= _table[5, a & 0x0F];
            a >>= 4;
            _n1 |= _table[6, a & 0x0F];
            a >>= 4;
            _n1 |= _table[7, a & 0x0F];
            _n1 = (_n1 << 11) | ((_n1 & 0xFFE00000) >> 21);

            _n1 ^= _n2;
            _n2 = b;
        }

        public void GostSimpleReplacement(uint[] inputData, uint[] outputData)
        {
            if (inputData.Length % 2 != 0 || inputData.Length < 2 || outputData.Length % 2 != 0 || outputData.Length < 2)
                return;

            for (var i = 0; i < inputData.Length; i += 2)
            {
                GostSimpleReplacementEncrypt(inputData[i], inputData[i + 1], 32);
                outputData[i] = _n2;
                outputData[i + 1] = _n1;
            }
        }

        private void GostSimpleReplacementEncrypt(uint block1, uint block2, uint countOfTakts)
        {
            _n1 = block1;
            _n2 = block2;

            for (var i = 0; i < countOfTakts; i++)
                GostTakt(_roundKeysForEncrypt[i]);
        }

        public void InitKey(byte[] key)
        {
            if (key.Length < 32) throw new ArgumentException("Gost key len incorrect");

            var tmp = new uint[8];

            for (var i = 0; i < tmp.Length; i++)
                tmp[i] = BitConverter.ToUInt32(key, 4 * i);
            

            //keys schedule
            for (var i = 0; i < 24; i++)
                _roundKeysForEncrypt[i] = tmp[i % 8];

            _roundKeysForEncrypt[24] = tmp[7];
            _roundKeysForEncrypt[25] = tmp[6];
            _roundKeysForEncrypt[26] = tmp[5];
            _roundKeysForEncrypt[27] = tmp[4];
            _roundKeysForEncrypt[28] = tmp[3];
            _roundKeysForEncrypt[29] = tmp[2];
            _roundKeysForEncrypt[30] = tmp[1];
            _roundKeysForEncrypt[31] = tmp[0];

            tmp.ZeroMemory();
        }

        public void Dispose()
        {
            _roundKeysForEncrypt.ZeroMemory();
        }
    }
    #endregion
}
