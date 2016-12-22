using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class DeleteLinqTests : BaseTest
    {
        [Test]
        public void Predicate_Can_Not_Be_Null()
        {
            var item = ULongNumbers.GetDefault();
            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<ULongNumbers>();
                    db.CreateTable<ULongNumbers>();

                    db.InsertItem(item);

                    db.Delete<ULongNumbers>(null);

                }
                catch (ArgumentNullException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Predicate can't be null", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void Delete_Whet_Table_Does_Not_Exist()
        {
            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<ULongNumbers>();

                    db.Delete<ULongNumbers>(i => i.Id == 1);

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Database doesn't contain table with name:", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void Encrypted_Columns_Can_Not_Be_Used_In_Predicate()
        {
            var accounts = GetAccounts();

            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(accounts[0]);

                    db.Delete<AccountsData>(a => a.Password == "Pass");

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("You can't use Encrypted columns for finding elements in database. Colum", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ULong_Forbidden_In_Predicate()
        {
            var item = ULongNumbers.GetDefault();
            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<ULongNumbers>();
                    db.CreateTable<ULongNumbers>();

                    db.InsertItem(item);

                    db.Delete<ULongNumbers>(i => i.ULongMaxVal == 1900);

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Properties with types 'UInt64', 'Int64', 'DateTime' can't be used in Predicates for finding elements.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void Long_Forbidden_In_Predicate()
        {
            var item = LongNumbers.GetDefault();
            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<LongNumbers>();
                    db.CreateTable<LongNumbers>();

                    db.InsertItem(item);

                    db.Delete<LongNumbers>(i => i.LongMaxVal == 1900);

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Properties with types 'UInt64', 'Int64', 'DateTime' can't be used in Predicates for finding elements.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void DateTime_Forbidden_In_Predicate()
        {
            var now = DateTime.Now;
            var item = new DateTimeTable { Date = now, NullAbleDate = DateTime.MinValue };

            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<DateTimeTable>();
                    db.CreateTable<DateTimeTable>();

                    db.InsertItem(item);

                    db.Delete<DateTimeTable>(i => i.Date == now);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Properties with types 'UInt64', 'Int64', 'DateTime' can't be used in Predicates for finding elements.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public async Task Delete_Using_Equal_To_Null_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = null, SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<SecretTask>();
                    await db.CreateTableAsync<SecretTask>();

                    await db.InsertItemAsync(st1);
                    await db.InsertItemAsync(st2);
                    await db.InsertItemAsync(st3);

                    await db.DeleteAsync<SecretTask>(a => a.Description == null);

                    var result = await db.TableAsync<SecretTask>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equal(st3));
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
        public async Task Delete_Using_Not_Equal_To_Null_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<SecretTask>();
                    await db.CreateTableAsync<SecretTask>();

                    await db.InsertItemAsync(st1);
                    await db.InsertItemAsync(st2);
                    await db.InsertItemAsync(st3);

                    await db.DeleteAsync<SecretTask>(a => a.Description != null);
                    var result = await db.TableAsync<SecretTask>();
                    Assert.NotNull(result);
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
        public async Task Delete_Using_Equal_To_Explicit_String_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Name == "Account0");
                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].Equal(accounts[1]));
                    Assert.IsTrue(table[1].Equal(accounts[2]));
                    Assert.IsTrue(table[2].Equal(accounts[4]));
                    Assert.IsTrue(table[3].Equal(accounts[5]));
                    Assert.IsTrue(table[4].Equal(accounts[7]));
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
        public async Task Delete_Using_LessThan_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Age < 27);
                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(accounts[3]));
                    Assert.IsTrue(table[1].Equal(accounts[4]));
                    Assert.IsTrue(table[2].Equal(accounts[7]));
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
        public async Task Delete_Using_GreaterThan_Or_Equal_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Age >= 25);
                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].Equal(accounts[0]));
                    Assert.IsTrue(table[1].Equal(accounts[1]));
                    Assert.IsTrue(table[2].Equal(accounts[2]));
                    Assert.IsTrue(table[3].Equal(accounts[5]));
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
        public async Task Delete_Using_Inclusive_Between_Two_Values_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Posts <= 60 && a.Posts >= 40);

                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(accounts[3]));
                    Assert.IsTrue(table[1].Equal(accounts[4]));
                    Assert.IsTrue(table[2].Equal(accounts[7]));
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
        public async Task Delete_Using_Inclusive_LessThan_Or_GreaterThan_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(accounts[0]));
                    Assert.IsTrue(table[1].Equal(accounts[1]));
                    Assert.IsTrue(table[2].Equal(accounts[5]));
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
        public async Task Delete_Using_Double_Greater_Than_Explicit_Value_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Productivity > 1605.005);

                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(accounts[1]));
                    Assert.IsTrue(table[1].Equal(accounts[2]));
                    Assert.IsTrue(table[2].Equal(accounts[7]));
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
        public async Task NullableULong_And_UShort_Equal_To_NUll_And_LessThan_Predicate()
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

                    await db.DeleteAsync<AccountsData>(a => a.Salary == null || a.Age > 27);

                    var result = await db.TableAsync<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(accounts[1]));
                    Assert.IsTrue(table[1].Equal(accounts[2]));
                    Assert.IsTrue(table[2].Equal(accounts[6]));
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
