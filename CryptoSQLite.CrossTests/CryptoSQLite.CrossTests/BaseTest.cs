using System;
using CryptoSQLite.CrossTests.Tables;
using Xamarin.Forms;

namespace CryptoSQLite.CrossTests
{
    public class BaseTest
    {
        private readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
                                        18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };


        public const string GostDbFile = "TestGost.db3";
        public const string AesDbFile = "TestAes.db3";
        public const string DesDbFile = "TestDes.db3";
        public const string TripleDesDbFile = "TestTripleDes.db3";

        private static readonly IDatabaseFolderPathGetter Folder;

        static BaseTest()
        {
            Folder = DependencyService.Get<IDatabaseFolderPathGetter>();
        }

        public CryptoSQLiteConnection GetAesConnection()
        {
            var conn = new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(AesDbFile), CryptoAlgoritms.AesWith256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection GetGostConnection()
        {
            var conn = new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(GostDbFile), CryptoAlgoritms.Gost28147With256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection GetDesConnection()
        {
            var conn = new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(DesDbFile), CryptoAlgoritms.DesWith56BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection GetTripleDesConnection()
        {
            var conn = new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(TripleDesDbFile), CryptoAlgoritms.TripleDesWith168BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteConnection[] GetConnections()
        {
            return new[] { GetGostConnection(), GetAesConnection(), GetDesConnection(), GetTripleDesConnection() };
        }

        public CryptoSQLiteConnection[] GetOnlyConnections()
        {
            return new[]
            {
                new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(AesDbFile), CryptoAlgoritms.AesWith256BitsKey),
                new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(GostDbFile), CryptoAlgoritms.Gost28147With256BitsKey),
                new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(DesDbFile), CryptoAlgoritms.DesWith56BitsKey),
                new CryptoSQLiteConnection(Folder.GetDatabaseFolderPath(TripleDesDbFile), CryptoAlgoritms.TripleDesWith168BitsKey)
            };
        }

        public CryptoSQLiteAsyncConnection GetGostAsyncConnection()
        {
            var conn = new CryptoSQLiteAsyncConnection(Folder.GetDatabaseFolderPath(GostDbFile), CryptoAlgoritms.Gost28147With256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteAsyncConnection GetAesAsyncConnection()
        {
            var conn = new CryptoSQLiteAsyncConnection(Folder.GetDatabaseFolderPath(AesDbFile), CryptoAlgoritms.AesWith256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteAsyncConnection GetDesAsyncConnection()
        {
            var conn = new CryptoSQLiteAsyncConnection(Folder.GetDatabaseFolderPath(DesDbFile), CryptoAlgoritms.DesWith56BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteAsyncConnection GetTripleDesAsyncConnection()
        {
            var conn = new CryptoSQLiteAsyncConnection(Folder.GetDatabaseFolderPath(TripleDesDbFile), CryptoAlgoritms.TripleDesWith168BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public CryptoSQLiteAsyncConnection[] GetAsyncConnections()
        {
            return new[]
            {
                GetAesAsyncConnection(),
                GetGostAsyncConnection(),
                GetDesAsyncConnection(),
                GetTripleDesAsyncConnection()
            };
        }

        public SecretTask[] GetTasks()
        {
            return new[]
            {
                new SecretTask
                {
                    Description = "It's the first task",
                    SecretToDo = "I must kill Mefisto",
                    Price = 8772.123,
                    IsDone = true
                },
                new SecretTask
                {
                    Description = "It's the second task",
                    SecretToDo = "I must kill Diablo",
                    Price = 24523.123,
                    IsDone = true
                },
                new SecretTask
                {
                    Description = "It's the third task",
                    SecretToDo = "I must kill Baal",
                    Price = 791233.7383,
                    IsDone = false
                },
                new SecretTask
                {
                    Description = "It's the fourth task",
                    SecretToDo = "I must stop playing games",
                    Price = -97593.2342,
                    IsDone = false
                }
            };
        }

        public AccountsData[] GetAccounts()
        {
            var account0 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = "Barbarian",
                AccountPassword = "Bct ghjcnj 1",
                Age = 21,
                IsAdministrator = false,
            };

            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = "Frodo Beggins",
                AccountPassword = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 22,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            var account2 = new AccountsData
            {
                Id = 66,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = uint.MaxValue,
                AccountName = "Gendalf Gray",
                AccountPassword = "I am master of Anor flame.",
                Age = 23,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            var account3 = new AccountsData
            {
                Id = 166,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 123462,
                AccountName = "Andariel",
                AccountPassword = "Big monster with big teeth.",
                Age = 23,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            var account4 = new AccountsData
            {
                Id = 666,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 7292362,
                AccountName = "Little Bug",
                AccountPassword = "I'm little bug. And you won't find me.",
                Age = 24,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            var account5 = new AccountsData
            {
                Id = 666,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 7292362,
                AccountName = "Big Bug",
                AccountPassword = "I'm big bug.",
                Age = 25,
                IsAdministrator = false
            };

            return new[] { account0, account1, account2, account3, account4, account5 };
        }
    }

    internal static class Extensions
    {
        public static void ZeroMemory(this byte[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void ZeroMemory(this uint[] buf)
        {
            for (var i = 0; i < buf.Length; i++)
                buf[i] = 0;
        }

        public static void MemCpy(this byte[] destination, byte[] source, int len = 0, int destinationStartIndex = 0, int sourceStartIndex = 0)
        {
            var ln = len == 0 ? destination.Length : len;

            if (destination.Length < ln + destinationStartIndex || source.Length < ln + sourceStartIndex)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < ln; i++)
                destination[i + destinationStartIndex] = source[i + sourceStartIndex];
        }

        public static void MemSet(this byte[] destination, byte value, int len = 0, int destinationStartIndex = 0)
        {
            var ln = len == 0 ? destination.Length : len;

            if (destination.Length < ln + destinationStartIndex)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < ln; i++)
                destination[i + destinationStartIndex] = value;
        }

        public static int MemCmp(this byte[] buff1, byte[] buff2, int len = 0)
        {
            if (buff1 == null || buff2 == null)
                throw new ArgumentNullException();

            var ln = len == 0 ? buff1.Length : len;
            var ret = 0;
            for (var i = 0; i < ln; i++)
            {
                if (buff1[i] < buff2[i])
                {
                    ret = -1;
                    break;
                }
                if (buff1[i] > buff2[i])
                {
                    ret = 1;
                    break;
                }
            }
            return ret;
        }

        public static void Xor(this byte[] buff, byte value, int startIndex)
        {
            if (startIndex >= buff.Length)
                throw new ArgumentException(nameof(startIndex));

            for (var i = 0; i < buff.Length; i++)
                buff[i] ^= value;
        }

        public static void Xor(this byte[] destination, byte[] source, int len)
        {
            if (destination.Length < len || source.Length < len)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i] ^= source[i];
        }

        public static void Xor(this uint[] destination, uint[] source, int len)
        {
            if (destination.Length < len || source.Length < len)
                throw new ArgumentException(nameof(len));

            for (var i = 0; i < len; i++)
                destination[i] ^= source[i];
        }
    }
}
