using System;
using CryptoSQLite.Extensions;

namespace CryptoSQLite.CryptoProviders
{
    internal class DesCryptoProvider : ICryptoProvider
    {
        // initial permutation for input data
        private readonly BaseDesAlgoritm _baseDes;
        private bool _keyInstalled;
        private ulong _solt;

        public DesCryptoProvider()
        {
            _baseDes = new BaseDesAlgoritm();
            _keyInstalled = false;
        }
        
        public void Dispose()
        {
            _solt = 0;
            _baseDes.Dispose();
        }

        public void SetKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            _baseDes.SetKey(BitConverter.ToUInt64(key, 0));

            _keyInstalled = true;
        }

        public void SetSolt(byte[] solt)
        {
            _solt = BitConverter.ToUInt64(solt, 0);
        }

        public void XorGamma(byte[] data, int dataLen = 0)
        {
            if (!_keyInstalled)
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
                // ReSharper disable once PossibleInvalidOperationException
                var tmp = _baseDes.ElectronicCodeBookEncrypt(_solt);

                _solt ^= tmp;

                for(var i = 0; i < 8; i++)
                    gamma[8*t + i] = (byte)(tmp >> 8*i);
            }

            return gamma;
        }
    }
}
