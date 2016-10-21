namespace CryptoSQLite.CryptoProviders
{
    internal class GostExternalCryptoProvider : ICryptoProvider
    {
        public byte[] Encrypt(byte[] data)
        {
            return data;
        }

        public byte[] Decrypt(byte[] data)
        {
            return data;
        }

        public void SetKey(byte[] key)
        {

        }

        public void SetSolt(byte[] solt)
        {
            
        }

        public void ClearKey()
        {
            
        }
    }
}
