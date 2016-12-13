using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class LinqTests : BaseTest
    {
        [Test]
        public async Task Empty_If_Not_Found()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name == "Frodo");

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

                    db.Find<ULongNumbers>(null);

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

                    db.Find<AccountsData>(a => a.Password == "Pass");

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

                    db.Find<ULongNumbers>(i => i.ULongMaxVal == 1900);

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

                    db.Find<LongNumbers>(i => i.LongMaxVal == 1900);

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

                    db.Find<DateTimeTable>(i => i.Date == now);
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
        public async Task Strings_Find_Using_Equal_To_Null_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name == null);

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
        public async Task Strings_Find_Using_Not_Equal_To_Null_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name != null);

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
        public async Task Strings_Find_Using_Equal_To_Explicit_String_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name == "Account1");

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
        public async Task Strings_Multiple_Results_Find_Using_Equal_To_Explicit_String_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name == "Account0");

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[6]));
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
        public async Task Strings_Multiple_Results_Find_Using_Equal_To_Explicit_String_And_Age_Less_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Name == "Account0" && a.Age < 27);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[6]));
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
        public async Task Bools_Find_Using_Equal_To_True_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
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
        public async Task Bools_Find_Using_Equal_To_False_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.IsAdministrator == false);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
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
        public async Task Bools_Find_Using_Equal_Not_True_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => !a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
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
        public async Task NullableUInt_Equal_To_Null_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == null);

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
        public async Task NullableUInt_Equal_Not_Null_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId != null);

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
        public async Task NullableUInt_Equal_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == 4);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[4]));
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
        public async Task Int_Equal_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts == 45);

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
        public async Task Int_LessThan_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts < 50);

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
        public async Task Int_LessThan_Or_Equal_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts <= 50);

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
        public async Task Int_GreaterThan_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts > 50);

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
        public async Task Int_GreaterThan_Or_Equal_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts >= 50);

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
        public async Task Int_Between_Two_Values_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts < 60 && a.Posts > 45);

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
        public async Task Int_Inclusive_Between_Two_Values_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts <= 60 && a.Posts >= 45);

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
        public async Task Int_Inclusive_LessThan_Or_GreaterThan_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

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
        public async Task Int_Exclusive_LessThan_Or_GreaterThan_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Posts > 60 || a.Posts < 45);

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
        public async Task Double_Equal_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity == 1778.99998);

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
        public async Task Double_Less_Than_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity < 1778.99998);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[5]));
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
        public async Task Double_LessOrEqual_To_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity <= 1778.99998);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[5]));
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
        public async Task Double_Greater_Than_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity > 1778.99998);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[6]));
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
        public async Task Double_GreaterOrEqual_Than_Explicit_Value_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity >= 1778.99998);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[4]));
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
        public async Task Double_Between_Explicit_Values_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity > 1700 && a.Productivity < 1900);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[6]));
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
        public async Task Double_Greater_Or_Less_Explicit_Values_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Productivity < 1700 || a.Productivity > 1900);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[1]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[2]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[4]));
                    Assert.IsTrue(table[3].IsTableEqualsTo(accounts[5]));
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
        public async Task NullableInt_And_Bool_Equal_To_Null_And_True_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator);

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
        public async Task NullableInt_And_Bool_Equal_To_Null_And_False_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator == false);

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
        public async Task NullableULong_Equal_To_Null_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Salary == null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].IsTableEqualsTo(accounts[0]));
                    Assert.IsTrue(table[1].IsTableEqualsTo(accounts[3]));
                    Assert.IsTrue(table[2].IsTableEqualsTo(accounts[5]));
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
        public async Task NullableULong_Equal_To_Explicit_Value_Forbidden_In_Predicate()
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

                    await db.FindAsync<AccountsData>(a => a.Salary == 1900);

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Properties with types 'UInt64?', 'Int64?', 'DateTime?' or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate", StringComparison.Ordinal)>=0);
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

                    var result = await db.FindAsync<AccountsData>(a => a.Salary == null && a.Age < 27);

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
        public async Task NullableULong_And_UShort_Equal_To_NUll_And_Between_Predicate()
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

                    var result = await db.FindAsync<AccountsData>(a => a.Salary == null && a.Age < 27 && a.Age > 20);

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
