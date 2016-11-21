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
        public async Task FindUsingUpperValue()
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
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[3]));
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
        public async Task FindUsingLowerValue()
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

                    var result = await db.FindAsync<AccountsData>("Age", 24);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[5]));
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
        public async Task FindUsingLowerAndUpperValues()
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

                    var result = await db.FindAsync<AccountsData>("Age", 22, 24);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[4]));
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
        public async Task FindWithoutUsingValues()
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
    }
}
