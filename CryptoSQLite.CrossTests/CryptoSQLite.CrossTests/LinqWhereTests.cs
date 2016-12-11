using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class LinqWhereTests : BaseTest
    {
        [Test]
        public async Task Not_Found()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Name == "Frodo");

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 0);
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
        public async Task Rule_Equal_Null_Strings()
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
        public async Task Rule_Equal_NotNull_Strings()
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
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[5]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[6]));
                    Assert.IsTrue(table[4].IsTableEqualsTo(accounts[7]));
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
        public async Task Rule_Equal_Null()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.SocialSecureId == null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[5]));
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
        public async Task Rule_Equal_NotNull()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.SocialSecureId != null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[4].IsTableEqualsTo(accounts[6]));
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
        public async Task Rule_LessThan_Ints()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts < 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[3]));
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
        public async Task Rule_LessThan_Or_Equal()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts <= 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[4].IsTableEqualsTo(accounts[5]));
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
        public async Task Rule_GreaterThan()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts > 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[6]));
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
        public async Task Rule_GreaterThanOrEqual()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts >= 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[6]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[7]));
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
        public async Task Rule_Between()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts < 60 && a.Posts > 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
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
        public async Task Rule_Between_Inclusive()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts <= 60 && a.Posts >= 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[5]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[6]));
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
        public async Task Rule_LessThan_And_GreaterThan()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[6]));
                    Assert.IsTrue(table[4].IsTableEqualsTo(accounts[7]));
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
        public async Task Rule_LessThan_And_GreaterThan_Inclusive()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Posts > 60 || a.Posts < 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[7]));
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
        public async Task Two_Rules_Equals_Null_And_True()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[7]));
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
        public async Task Two_Rules_Equals_Null_And_False()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator == false);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[2]));
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
        public async Task Two_Rules_Equal_And_LessThan()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Salary == null && a.Age < 27);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
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
        public async Task Three_Rules_Equal_LessThan_And_GreaterThan()
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

                    var result = await db.WhereAsync<AccountsData>(a => a.Salary == null && a.Age < 27 && a.Age > 20);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[5]));
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
