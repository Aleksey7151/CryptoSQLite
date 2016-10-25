using System;

namespace CryptoSQLite.CryptoProviders
{
    internal class GostExternalCryptoProvider : ICryptoProvider
    {
        private byte[] _key;
        private byte[] _solt;
        public GostExternalCryptoProvider()
        {
        }
        public byte[] Encrypt(byte[] data)
        {
            if(_key == null)
                throw new NullReferenceException("Encryption key has not been installed");
            if(_solt == null)
                throw new NullReferenceException("Solt has not been installed");

            var toRet = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                toRet[i] = (byte)(data[i] ^ _solt[i % _solt.Length] ^ _key[i % _key.Length]);
            }
            return toRet;
        }

        public byte[] Decrypt(byte[] data)
        {
            if (_key == null)
                throw new NullReferenceException("Encryption key has not been installed");
            if (_solt == null)
                throw new NullReferenceException("Solt has not been installed");

            var toRet = new byte[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                toRet[i] = (byte)(data[i] ^ _solt[i % _solt.Length] ^ _key[i % _key.Length]);
            }
            return toRet;
        }

        public void SetKey(byte[] key)
        {
            _key = key;
        }

        public void SetSolt(byte[] solt)
        {
            _solt = solt;
        }
    }
}
