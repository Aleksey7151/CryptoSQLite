using System;

namespace CryptoSQLite.CryptoProviders
{
    internal interface ISoltGenerator
    {
        /// <summary>
        /// Creates the pseudorandom Solt for encryption algoritms
        /// </summary>
        /// <param name="count">Count of bytes to generate</param>
        /// <returns><paramref name="count"/> bytes of pseudorandom data</returns>
        byte[] GetSolt(int count = 8);
    } 

    /// <summary>
    /// Generates the solt (initialization secuense) for cryptografic algoritms. solt is not secret parameter, 
    /// so it can be open and written in database.
    /// It's more important to have different solt values for different rows, and that's all.
    /// </summary>
    internal class SoltGenerator : ISoltGenerator
    {
        public byte[] GetSolt(int count = 8)
        {
            if(count < 1 || count > 128)
                throw new ArgumentException(nameof(count));

            var solt = new byte[count];

            var random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 10000 + DateTime.Now.Minute * 100000);

            random.NextBytes(solt);

            return solt;
        }
    }
}
