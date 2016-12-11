using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class FindTests : BaseTest
    {
        [Test]
        public async Task FindRowsInTableThatHaveNullValues()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    accounts[0].Name = null;
                    accounts[1].Name = null;
                    accounts[2].Name = null;

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindByValueAsync<AccountsData>("Name", null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[2]));
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
        public async Task FindFunctionUsingUpperValue()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>("Age", null, 23);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[5]));

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
        public async Task FindFunctionUsingLowerValue()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>("Age", 27);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[7]));
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
        public async Task FindFunctionUsingLowerAndUpperValues()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>("Age", 20, 24);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[5]));
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
        public async Task FindByValueFunction()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindByValueAsync<AccountsData>("Age", 20);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
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
        public async Task FindFunctionWithoutUsingValues()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>("Age");

                    var table = result.ToArray();

                    Assert.IsTrue(table.Length == accounts.Length);

                    for (var i = 0; i < accounts.Length; i++)
                        Assert.IsTrue(table[i].IsTableEqualsTo(accounts[i]));

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
        public async Task FindFunctionUsingInvalidColumnName()
        {
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.FindAsync<AccountsData>("Agee");
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("doesn't contain column", StringComparison.Ordinal) > 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public async Task FindByValueFunctionUsingInvalidColumnName()
        {
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.FindByValueAsync<AccountsData>("Agee", 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("doesn't contain column", StringComparison.Ordinal) > 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public async Task FindFunctionUsingNullColumnName()
        {
            using (var db = GetGostAsyncConnection())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.FindAsync<AccountsData>(null);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Column name can't be null.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public async Task FindByValueFunctionUsingNullColumnName()
        {
            using (var db = GetGostAsyncConnection())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.FindByValueAsync<AccountsData>(null, 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public async Task FindFunctionUsingEncryptedColumnIsForbidden()
        {
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();
                    var result = await db.FindAsync<AccountsData>("Password");
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("has [Encrypted] attribute, so this column is encrypted. Find function can't work with encrypted columns", StringComparison.Ordinal) > 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public async Task FindByValueFunctionUsingEncryptedColumnIsForbidden()
        {
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();
                    var result = await db.FindByValueAsync<AccountsData>("Password", new object());
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("You can't use [Encrypted] column as a column in which the columnValue should be", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }
    }
}
