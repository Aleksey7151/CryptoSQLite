using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class AsyncTests : BaseTest
    {
        [Test]
        public async Task InsertNormalElementInCryptoTableAsync()
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = "Frodo Beggins",
                AccountPassword = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            var account2 = new AccountsData
            {
                Id = 66,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = uint.MaxValue,
                AccountName = "Gendalf Gray",
                AccountPassword = "I am master of Anor flame.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            var account3 = new AccountsData
            {
                Id = 166,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 123462,
                AccountName = "Andariel",
                AccountPassword = "Big monster with big teeth.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            var account4 = new AccountsData
            {
                Id = 666,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 7292362,
                AccountName = "Little Bug",
                AccountPassword = "I'm little bug. And you won't find me.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    await db.InsertItemAsync(account1);
                    await db.InsertItemAsync(account2);
                    await db.InsertItemAsync(account3);
                    await db.InsertItemAsync(account4);

                    var result = await db.TableAsync<AccountsData>();

                    var table = result.ToArray();

                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account2)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account3)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account4)));

                    await db.DeleteItemAsync<AccountsData>(1);

                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account2)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account3)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account4)));

                    await db.DeleteItemAsync<AccountsData>(3);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account2)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account3)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account4)));

                    await db.DeleteItemAsync<AccountsData>(2);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account2)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account3)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account4)));

                    await db.DeleteItemAsync<AccountsData>(4);
                    result = await db.TableAsync<AccountsData>();

                    table = result.ToArray();

                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account2)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account3)));
                    Assert.IsFalse(table.Any(e => e.IsTableEqualsTo(account4)));
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
