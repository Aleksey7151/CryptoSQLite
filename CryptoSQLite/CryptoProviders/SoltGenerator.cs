namespace CryptoSQLite.CryptoProviders
{
    internal interface ISoltGenerator
    {
        /// <summary>
        /// Creates the pseudorandom Solt for encryption algoritms
        /// </summary>
        /// <returns>8 bytes of pseudorandom data</returns>
        byte[] GetSolt();
    } 
    internal class SoltGenerator : ISoltGenerator
    {
        public byte[] GetSolt()
        {
            return new byte[] {12, 23, 34, 45, 56, 67, 78, 89};
        }
    }
}
