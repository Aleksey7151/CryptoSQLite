namespace CryptoSQLite
{
    /// <summary>
    /// Enumerates crypto algorithms, that can be used for data encryption in CryptoSQLite library
    /// </summary>
    public enum CryptoAlgorithms
    {
        /// <summary>
        /// USSR encryption algorithm. It uses the 256 bit encryption key.
        /// <para/>This algorithm is reliable. Many exsoviet union countries use this algorithm for secret data protection.
        /// </summary>
        Gost28147With256BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 256 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith256BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 192 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith192BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 128 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith128BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 56 bit encryption key.
        /// <para/>This algorithm is Very Fast, but not reliable.
        /// </summary>
        DesWith56BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 168 bit encryption key.
        /// <para/>This algorithm is Fast and more reliable than DES, but not so fast as DES.
        /// </summary>
        TripleDesWith168BitsKey
    }
}