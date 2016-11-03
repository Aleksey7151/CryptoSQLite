using System.Collections.Generic;
using CryptoSQLite;

namespace Tests
{
    public class BaseTest
    {
        private readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
                                        18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };


        public const string GostDbFile = "TestGost.db3";
        public const string AesDbFile = "TestAes.db3";
        public CryptoSQLiteConnection GetAesConnection()
        {
            var conn = new CryptoSQLiteConnection(AesDbFile, CryptoAlgoritms.AesWith256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection GetGostConnection()
        {
            var conn = new CryptoSQLiteConnection(GostDbFile, CryptoAlgoritms.Gost28147With256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection[] GetConnections()
        {
            return new[] {GetGostConnection(), GetAesConnection()};
        }
    }
}
