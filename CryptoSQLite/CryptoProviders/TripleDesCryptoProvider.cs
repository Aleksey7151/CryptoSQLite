using System;

namespace CryptoSQLite.CryptoProviders
{
    internal class TripleDesCryptoProvider :ICryptoProvider
    {
        // initial permutation for input data
        private readonly BaseDesAlgoritm _baseDes;

        private ulong _solt;

        private ulong _key1;
        private ulong _key2;
        private ulong _key3;

        public TripleDesCryptoProvider()
        {
            _baseDes = new BaseDesAlgoritm();
        }

        public void Dispose()
        {
            _solt = 0;
            //Clean Up
            _key1 = 0;
            _key2 = 0;
            _key3 = 0;

            _baseDes.Dispose();
        }

        public void SetKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _key1 = 0;
            _key2 = 0;
            _key3 = 0;

            _key1 = BitConverter.ToUInt64(key, 0);
            _key2 = BitConverter.ToUInt64(key, 8);
            _key3 = BitConverter.ToUInt64(key, 16);
        }

        public void SetSolt(byte[] solt)
        {
            _solt = BitConverter.ToUInt64(solt, 0);
        }

        public void XorGamma(byte[] data, int dataLen = 0)
        {
            if (_key1 == 0 && _key2 == 0 && _key3 == 0)
                throw new NullReferenceException("Encryption key has not been installed.");
            if (_solt == 0)
                throw new NullReferenceException("Solt has not been installed.");

            var len = dataLen == 0 ? data.Length : dataLen;

            var gamma = GetGamma(len);

            data.Xor(gamma, len);

            gamma.ZeroMemory();     // clean up
        }

        private byte[] GetGamma(int count)
        {
            if (count < 1)
                throw new ArgumentException();

            var takts = count / 8;
            if (count % 8 > 0)
                takts += 1;

            var gamma = new byte[takts * 8];

            for (var t = 0; t < takts; t++)
            {
                _baseDes.SetKey(_key1);     // encrypt data
                var tmp = _baseDes.ElectronicCodeBookEncrypt(_solt);

                _baseDes.SetKey(_key2);     // decrypt data using different key
                tmp = _baseDes.ElectronicCodeBookDecrypt(tmp);

                _baseDes.SetKey(_key3);     // encrypt data using differend key
                tmp = _baseDes.ElectronicCodeBookEncrypt(tmp);

                _solt ^= tmp;

                for (var i = 0; i < 8; i++)
                    gamma[8 * t + i] = (byte)(tmp >> 8 * i);
            }

            return gamma;
        }

    }
}
