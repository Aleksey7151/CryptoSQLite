﻿using System;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    [Collection("Sequential")]
    public class EncryptionKeySetTests : BaseTest
    {
        [Fact]
        public void SetNullKey()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    db.SetEncryptionKey(null);
                });
                Assert.Contains("key", ex.ParamName);
            }
        }

        [Fact]
        public void SetWrongKeyToGost()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var smallKey = new byte[31];
                    db.SetEncryptionKey(smallKey);
                });
                Assert.Contains("Key length for AES with 256 bit key and GOST must be 32 bytes.", ex.Message);
            }
        }

        [Fact]
        public void SetWrongKeyToAes256()
        {
            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var smallKey = new byte[31];
                    db.SetEncryptionKey(smallKey);
                });
                Assert.Contains("Key length for AES with 256 bit key and GOST must be 32 bytes.", ex.Message);
            }
        }

        [Fact]
        public void SetWrongKeyToAes192()
        {
            using (var db = GetAes192Connection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var bigKey = new byte[23];
                    db.SetEncryptionKey(bigKey);
                });
                Assert.Contains("Key length for AES with 192 bit key must be 24 bytes.", ex.Message);
            }
        }

        [Fact]
        public void SetWrongKeyToAes128()
        {
            using (var db = GetAes128Connection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var bigKey = new byte[15];
                    db.SetEncryptionKey(bigKey);
                });
                Assert.Contains("Key length for AES with 128 bit key must be 16 bytes.", ex.Message);
            }
        }

        [Fact]
        public void SetWrongKeyToDes()
        {
            using (var db = GetDesConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var smallKey = new byte[7];
                    db.SetEncryptionKey(smallKey);
                });
                Assert.Contains("Key length for DES must be at least 8 bytes", ex.Message);
            }
        }

        [Fact]
        public void SetWrongKeyToTripleDes()
        {
            using (var db = GetTripleDesConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    var smallKey = new byte[23];
                    db.SetEncryptionKey(smallKey);
                });
                Assert.Contains("Key length for 3DES must be at least 24 bytes.", ex.Message);
            }
        }

        [Fact]
        public void InsertItemFunctionIsForbiddenWhenEncryptionKeyIsNotSetted()
        {
            var tasks = GetTasks();
            foreach (var db in GetOnlyConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    //Key is not setted!
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();
                    db.InsertItem(tasks[0]);
                });
                Assert.Contains("Encryption key has not been installed.", ex.Message);
            }
        }

        [Fact]
        public void GetItemFunctionIsForbiddenWhenEncryptionKeyIsNotSetted()
        {
            var tasks = GetTasks();
            // Insert items in database
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                }
                finally
                {
                    db.Dispose();
                }
            }

            // trying to get items from database without encryption key
            foreach (var db in GetOnlyConnections())    // connections without setted encryption key
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.Find<SecretTask>(st => st.Id == 1);
                });
                Assert.Contains("Encryption key has not been installed.", ex.Message);
            }
        }

        [Fact]
        public void DeleteItemFunctionIs_Allowed_WhenEncryptionKeyIsNotSetted()
        {
            var task = new SecretTask { Description = "Some descriptionen 1", Price = 99.45, IsDone = false, SecretToDo = "Some Secret Info" };
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    db.InsertItem(task);
                }
                finally
                {
                    db.Dispose();
                }
            }

            foreach (var db in GetOnlyConnections())    // connections without setted encryption key
            {
                try
                {
                    db.Delete<SecretTask>(st => st.Id == 1);       // delete function is allowed
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
