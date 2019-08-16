using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    
    public class FindByValueTests : BaseTest
    {
        [Fact]
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
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equal(st1));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }


        [Fact]
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
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[0]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }


        [Fact]
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
                Assert.Contains("doesn't contain column", ex.Message);
            }
        }

        [Fact]
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
                Assert.Contains("Column name can't be null or empty.", ex.Message);
            }
        }

        [Fact]
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
                Assert.Contains("You can't use [Encrypted] column as a column in which the columnValue should be", ex.Message);
            }
        }
    }
}
