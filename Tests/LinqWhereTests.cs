using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class LinqWhereTests : BaseTest
    {
        [Test]
        public async Task FindElementsWithNullValuesStrings()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Name == null);

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
        public async Task FindElementsWithNotNullValuesStrings()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Name != null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[4]));
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
    }
}
