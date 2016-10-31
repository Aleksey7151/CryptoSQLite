namespace CryptoSQLite.CryptoProviders
{
    internal interface ICryptoProvider : IEncryptor, IKeysSetter
    {
        
    }
    /// <summary>
    /// Interface of key destribution
    /// </summary>
    public interface IKeysSetter
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
    }

    /// <summary>
    /// Interface of encryptor that can encrypt and decrypt data, 
    /// and can be implemented by programmer
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypts the open data <paramref name="openData"/>
        /// </summary>
        /// <param name="openData">Data that must be encrypted</param>
        /// <returns>Encrypted (closed) data</returns>
        byte[] Encrypt(byte[] openData);

        /// <summary>
        /// Decrypts the closed data <paramref name="closedData"/>
        /// </summary>
        /// <param name="closedData">Data that must be decrypted</param>
        /// <returns>Decrypted (open) data</returns>
        byte[] Decrypt(byte[] closedData);
    }
}
