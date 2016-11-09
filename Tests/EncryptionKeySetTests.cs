﻿using System;
using CryptoSQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tables;

namespace Tests
{
    [TestClass]
    public class EncryptionKeySetTests : BaseTest
    {
        [TestMethod]
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

        [TestMethod]
        public void SetKeyWithIncorrectLen()
        {
            foreach (var db in GetOnlyConnections())
            {
                try
                {
                    var key = new byte[31];
                    db.SetEncryptionKey(key);
                }
                catch (ArgumentException ae)
                {
                    Assert.IsTrue(ae.Message.IndexOf("Key length must be 32 bytes.", StringComparison.Ordinal)>=0);
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
