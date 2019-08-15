using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.Tests
{
    [TestFixture]
    public class FindByValueTests : BaseTest
    {
        [Test]
        public void FindRowsInTableThatHaveNullValues()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<SecretTask>();
                     db.CreateTable<SecretTask>();

                     db.InsertItem(st1);
                     db.InsertItem(st2);
                     db.InsertItem(st3);

                    var result =  db.FindByValue<SecretTask>("Description", null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equal(st1));
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
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
        public void FindByValueFunction()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.FindByValue<AccountsData>("Age", 20);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(accounts[0]));
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message + cex.ProbableCause);
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
        public void FindByValueFunctionUsingInvalidColumnName()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();
                    db.FindByValue<AccountsData>("Agee", 123);
                });
                Assert.That(ex.Message, Contains.Substring("doesn't contain column"));
            }
        }

        [Test]
        public void FindByValueFunctionUsingNullColumnName()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();
                    db.FindByValue<AccountsData>(null, 123);
                });
                Assert.That(ex.Message, Contains.Substring("Column name can't be null or empty."));
            }
        }

        [Test]
        public void FindByValueFunctionUsingEncryptedColumnIsForbidden()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();
                    db.FindByValue<AccountsData>("Password", new object());
                });
                Assert.That(ex.Message, Contains.Substring("You can't use [Encrypted] column as a column in which the columnValue should be"));
            }
        }
    }
}
