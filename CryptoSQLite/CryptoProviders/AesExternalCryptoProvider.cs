using System;

namespace CryptoSQLite.CryptoProviders
{
    internal class AesExternalCryptoProvider : ICryptoProvider
    {
        private byte[] _key;
        private byte[] _solt;
        public AesExternalCryptoProvider()
        {
            
        }

        public byte[] Encrypt(byte[] openData)
        {
            if (_key == null)
                throw new NullReferenceException("Encryption key has not been installed");
            if (_solt == null)
                throw new NullReferenceException("Solt has not been installed");

            var toRet = new byte[openData.Length];
            for (var i = 0; i < openData.Length; i++)
            {
                toRet[i] = (byte)(openData[i]^_solt[i%_solt.Length]^_key[i%_key.Length]);
            }
            return toRet;
        }

        public byte[] Decrypt(byte[] closedData)
        {
            if (_key == null)
                throw new NullReferenceException("Encryption key has not been installed");
            if (_solt == null)
                throw new NullReferenceException("Solt has not been installed");

            var toRet = new byte[closedData.Length];
            for (var i = 0; i < closedData.Length; i++)
            {
                toRet[i] = (byte)(closedData[i] ^ _solt[i % _solt.Length] ^ _key[i % _key.Length]);
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
