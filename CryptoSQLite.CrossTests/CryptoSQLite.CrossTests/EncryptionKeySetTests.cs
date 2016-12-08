using System;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class EncryptionKeySetTests : BaseTest
    {
        [Test]
        public void SetNullKey()
        {
            foreach (var db in GetOnlyConnections())
            {
                try
                {
                    db.SetEncryptionKey(null);
                }
                catch (ArgumentNullException)
                {
                    Assert.IsTrue(true);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Test]
        public void SetSmallKeyToGost()
        {
            using (var db = GetGostConnection())
            {
                var smallKey = new byte[31];

                try
                {
                    db.SetEncryptionKey(smallKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for AES and GOST must be 32 bytes", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }

        }

        [Test]
        public void SetBigKeyToGost()
        {
            using (var db = GetGostConnection())
            {
                var bigKey = new byte[33];

                try
                {
                    db.SetEncryptionKey(bigKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for AES and GOST must be 32 bytes", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void SetSmallKeyToAes()
        {
            using (var db = GetAesConnection())
            {
                var smallKey = new byte[31];

                try
                {
                    db.SetEncryptionKey(smallKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for AES and GOST must be 32 bytes", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }

        }

        [Test]
        public void SetBigKeyToAes()
        {
            using (var db = GetAesConnection())
            {
                var bigKey = new byte[33];

                try
                {
                    db.SetEncryptionKey(bigKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for AES and GOST must be 32 bytes", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void SetSmallKeyToDes()
        {
            using (var db = GetDesConnection())
            {
                var smallKey = new byte[7];

                try
                {
                    db.SetEncryptionKey(smallKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for DES must be at least 8 bytes", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }

        }

        [Test]
        public void SetSmallKeyToTripleDes()
        {
            using (var db = GetTripleDesConnection())
            {
                var smallKey = new byte[23];

                try
                {
                    db.SetEncryptionKey(smallKey);
                }
                catch (ArgumentException ex)
                {
                    Assert.IsTrue(ex.Message.IndexOf("Key length for 3DES must be at least 24 bytes.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }

        }

        [Test]
        public void InsertItemFunctionIsForbiddenWhenEncryptionKeyIsNotSetted()
        {
            var tasks = GetTasks();
            foreach (var db in GetOnlyConnections())
            {
                try
                {
                    //Key is not setted!
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();
                    db.InsertItem(tasks[0]);
                }
                catch (NullReferenceException ae)
                {
                    Assert.IsTrue(ae.Message.IndexOf("Encryption key has not been installed.", StringComparison.Ordinal) >= 0);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Test]
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
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }

            // trying to get items from database without encryption key
            foreach (var db in GetOnlyConnections())    // connections without setted encryption key
            {
                try
                {
                    db.GetItem<SecretTask>(1);
                }
                catch (NullReferenceException ae)
                {
                    Assert.IsTrue(ae.Message.IndexOf("Encryption key has not been installed.", StringComparison.Ordinal) >= 0);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Test]
        public void DeleteItemFunctionIs_Allowed_WhenEncryptionKeyIsNotSetted()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
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
                    db.DeleteItem<SecretTask>(1);       // delete function is allowed
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
