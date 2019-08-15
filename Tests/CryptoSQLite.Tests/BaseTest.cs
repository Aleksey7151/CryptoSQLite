using System;
using CryptoSQLite.Tests.Tables;

namespace CryptoSQLite.Tests
{
    public class BaseTest
    {
        private readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
                                        18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
        public const string GostDbFile = "TestGost.db3";
        public const string Aes256DbFile = "TestAes256.db3";
        public const string Aes192DbFile = "TestAes192.db3";
        public const string Aes128DbFile = "TestAes128.db3";
        public const string DesDbFile = "TestDes.db3";
        public const string TripleDesDbFile = "TestTripleDes.db3";

        private static readonly IDatabaseFolderPathGetter Folder;

        static BaseTest()
        {
            Folder = DependencyService.Get<IDatabaseFolderPathGetter>();
        }

        public ICryptoSQLite GetAes256Connection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes256DbFile), CryptoAlgorithms.AesWith256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite GetAes192Connection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes192DbFile), CryptoAlgorithms.AesWith192BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite GetAes128Connection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes128DbFile), CryptoAlgorithms.AesWith128BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite GetGostConnection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(GostDbFile), CryptoAlgorithms.Gost28147With256BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite GetDesConnection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(DesDbFile), CryptoAlgorithms.DesWith56BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite GetTripleDesConnection()
        {
            var conn = CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(TripleDesDbFile), CryptoAlgorithms.TripleDesWith168BitsKey);
            conn.SetEncryptionKey(_key);
            return conn;
        }

        public ICryptoSQLite[] GetConnections()
        {
            return new[] { GetGostConnection(), GetAes256Connection(), GetAes192Connection(), GetAes128Connection(), GetDesConnection(), GetTripleDesConnection() };
        }

        public ICryptoSQLite[] GetOnlyConnections()
        {
            return new[]
            {
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes256DbFile), CryptoAlgorithms.AesWith256BitsKey),
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes192DbFile), CryptoAlgorithms.AesWith192BitsKey),
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(Aes128DbFile), CryptoAlgorithms.AesWith128BitsKey),
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(GostDbFile), CryptoAlgorithms.Gost28147With256BitsKey),
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(DesDbFile), CryptoAlgorithms.DesWith56BitsKey),
                CryptoSQLiteFactory.Current.Create(Folder.GetDatabaseFolderPath(TripleDesDbFile), CryptoAlgorithms.TripleDesWith168BitsKey)
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
            return new[]
            {
                new AccountsData
                {
                    Name = "Account0",
                    Password = "Password0",
                    SocialSecureId = 1,
                    Productivity = 1778.99998,
                    Posts = 45,
                    Age = 20,
                    Salary = null,
                    IsAdministrator = true
                },
                new AccountsData
                {
                    Name = "Account1",
                    Password = "Password1",
                    SocialSecureId = 2,
                    Productivity = -123456.78901,
                    Posts = 50,
                    Age = 21,
                    Salary = 2700,
                    IsAdministrator = false
                },
                new AccountsData
                {
                    Name = "Account2",
                    Password = "Password2",
                    SocialSecureId = null,
                    Productivity = 0.998176532,
                    Posts = 40,
                    Age = 18,
                    Salary = 1900,
                    IsAdministrator = false
                },
                new AccountsData
                {
                    Name = "Account0",
                    Password = "Password3",
                    SocialSecureId = 3,
                    Productivity = 1873.20007,
                    Posts = 30,
                    Age = 27,
                    Salary = null,
                    IsAdministrator = true
                },
                new AccountsData
                {
                    Name = "Account4",
                    Password = "Password4",
                    SocialSecureId = 4,
                    Productivity = 1907.6710173,
                    Posts = 70,
                    Age = 33,
                    Salary = 3200,
                    IsAdministrator = true
                },
                new AccountsData
                {
                    Name = "Account5",
                    Password = "Password5",
                    SocialSecureId = null,
                    Productivity = 1605.37217,
                    Posts = 45,
                    Age = 22,
                    Salary = null,
                    IsAdministrator = false
                },
                new AccountsData
                {
                    Name = "Account0",
                    Password = "Password6",
                    SocialSecureId = 5,
                    Productivity = 1801.77781,
                    Posts = 60,
                    Age = 25,
                    Salary = 1500,
                    IsAdministrator = true
                },
                new AccountsData
                {
                    Name = "Account7",
                    Password = "Password7",
                    SocialSecureId = null,
                    Productivity = -0.91178654321,
                    Posts = 65,
                    Age = 29,
                    Salary = null,
                    IsAdministrator = true
                }
            };
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
