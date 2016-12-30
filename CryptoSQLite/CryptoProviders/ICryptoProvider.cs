using System;

namespace CryptoSQLite.CryptoProviders
{
    internal interface ICryptoProvider : IDisposable
    {
        /// <summary>
        /// Sets the encryption key. Key is secret parameter of encryption algoritm. 
        /// </summary>
        /// <param name="key">Buffer that contains encryption key. Length must be 32 bytes.</param>
        void SetKey(byte[] key);
        /// <summary>
        /// The solt of encryption algoritm. Solt is not secret parameter of encryption algoritm.
        /// But it is recommened to use unique solt for each database file. 
        /// </summary>
        /// <param name="solt">Buffer that contains solt. Length must be 8 bytes.</param>
        void SetSolt(byte[] solt);

        /// <summary>
        /// Encrypts the open data <paramref name="data"/> by adding to 
        /// <paramref name="data"/> gamma.
        /// </summary>
        /// <param name="data">Data that must be encrypted or decrypted</param>
        /// <param name="columnNumber">Number of column that must be encrypted</param>
        /// <param name="dataLen">Count of bytes that must be encrypted</param>
        void XorGamma(byte[] data, int columnNumber, int dataLen = 0);
    }
}
