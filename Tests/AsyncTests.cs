using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class AsyncTests : BaseTest
    {
        [Test]
        public async Task InsertNormalElementInCryptoTableAsync()
        {
            var accounts = GetAccounts();
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.InsertItemAsync(accounts[1]);
                    await db.InsertItemAsync(accounts[2]);
                    await db.InsertItemAsync(accounts[3]);
                    await db.InsertItemAsync(accounts[4]);

                    var result = await db.TableAsync<AccountsData>();

                    var table = result.ToArray();

                    Assert.IsTrue(table.Any(e => e.Equal(accounts[1])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[2])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[3])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[4])));

                    await db.DeleteItemAsync<AccountsData>(1);

                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.Equal(accounts[1])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[2])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[3])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[4])));

                    await db.DeleteItemAsync<AccountsData>(3);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.Equal(accounts[1])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[2])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[3])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[4])));

                    await db.DeleteItemAsync<AccountsData>(2);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.Equal(accounts[1])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[2])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[3])));
                    Assert.IsTrue(table.Any(e => e.Equal(accounts[4])));

                    await db.DeleteItemAsync<AccountsData>(4);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.Equal(accounts[1])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[2])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[3])));
                    Assert.IsFalse(table.Any(e => e.Equal(accounts[4])));
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
