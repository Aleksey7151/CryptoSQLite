using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class FindUsingPredicateTests : BaseTest
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
            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    db.DeleteTable<ULongNumbers>();
                    db.CreateTable<ULongNumbers>();

                    db.InsertItem(item);

                    db.Find<ULongNumbers>(null);
                });
                Assert.That(ex.Message, Contains.Substring("Predicate can't be null"));
            }
        }

        [Test]
        public void Encrypted_Columns_Can_Not_Be_Used_In_Predicate()
        {
            var accounts = GetAccounts();

            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(accounts[0]);

                    db.Find<AccountsData>(a => a.Password == "Pass");
                });
                Assert.That(ex.Message, Contains.Substring("You can't use Encrypted columns for finding elements in database. Colum"));
            }
        }

        [Test]
        public void ULong_Forbidden_In_Predicate()
        {
            var item = ULongNumbers.GetDefault();
            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<ULongNumbers>();
                    db.CreateTable<ULongNumbers>();

                    db.InsertItem(item);

                    db.Find<ULongNumbers>(i => i.ULongMaxVal == 1900);
                });
                Assert.That(ex.Message, Contains.Substring("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements."));
            }
        }

        [Test]
        public void Long_Forbidden_In_Predicate()
        {
            var item = LongNumbers.GetDefault();
            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<LongNumbers>();
                    db.CreateTable<LongNumbers>();

                    db.InsertItem(item);

                    db.Find<LongNumbers>(i => i.LongMaxVal == 1900);
                });
                Assert.That(ex.Message, Contains.Substring("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements."));
            }
        }

        [Test]
        public void DateTime_Forbidden_In_Predicate()
        {
            var now = DateTime.Now;
            var item = new DateTimeTable { Date = now, NullAbleDate = DateTime.MinValue };

            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<DateTimeTable>();
                    db.CreateTable<DateTimeTable>();

                    db.InsertItem(item);

                    db.Find<DateTimeTable>(i => i.Date == now);
                });

                Assert.That(ex.Message, Contains.Substring("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements."));
            }
        }

        [Test]
        public async Task Strings_Find_Using_Equal_To_Null_Predicate()
        {
            var st1 = new SecretTask {IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task"};
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

                    var result = await db.FindAsync<SecretTask>(a => a.Description == null);

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
        public async Task Strings_Find_Using_Not_Equal_To_Null_Predicate()
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

                    var result = await db.FindAsync<SecretTask>(a => a.Description != null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].Equal(st2));
                    Assert.IsTrue(table[1].Equal(st3));
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
                    Assert.IsTrue(table[0].Equals(accounts[1]));
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
                    var correct = new[] {accounts[0], accounts[3], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    var correct = new[] {accounts[0], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
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
                    var correct = new[] {accounts[0], accounts[3], accounts[4], accounts[6], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    var correct = new[] { accounts[1], accounts[2], accounts[5] };
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    var correct = new[] {accounts[1], accounts[2], accounts[5]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    var correct = new[] {accounts[2], accounts[5], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId != null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    var correct = new[] {accounts[0], accounts[1], accounts[3], accounts[4], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.SocialSecureId == 4);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(accounts[4]));
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
                    Assert.IsTrue(table[0].Equals(accounts[0]));
                    Assert.IsTrue(table[1].Equals(accounts[5]));
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts < 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);

                    var correct = new[] {accounts[0], accounts[2], accounts[3], accounts[5]};
      
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts <= 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    var correct = new[] {accounts[0], accounts[1], accounts[2], accounts[3], accounts[5]};

                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts > 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    var correct = new[] {accounts[4], accounts[6], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts >= 50);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    var correct = new[] {accounts[1], accounts[4], accounts[6], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts < 60 && a.Posts > 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(accounts[1]));
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts <= 60 && a.Posts >= 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    var correct = new[] {accounts[0], accounts[1], accounts[5], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    var correct = new[] {accounts[2], accounts[3], accounts[4], accounts[6], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    await db.ClearTableAsync<AccountsData>();
                    await db.DeleteTableAsync<AccountsData>();
                    
                    await db.CreateTableAsync<AccountsData>();

                    foreach (var account in accounts)
                        await db.InsertItemAsync(account);

                    var result = await db.FindAsync<AccountsData>(a => a.Posts > 60 || a.Posts < 45);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);

                    var correct = new[] { accounts[2], accounts[3], accounts[4], accounts[7] };

                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    var correct = new[] {accounts[1], accounts[2], accounts[5], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    var correct = new[] {accounts[0], accounts[1], accounts[2], accounts[5], accounts[7]};

                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    var correct = new[] {accounts[3], accounts[4], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    var correct = new[] {accounts[0], accounts[3], accounts[4], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
                    var correct = new[] {accounts[0], accounts[3], accounts[6]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
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
                    var correct = new[] {accounts[1], accounts[2], accounts[4], accounts[5], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[4])) == 1);
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
                    Assert.IsTrue(table[0].Equals(accounts[7]));
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
                    var correct = new[] {accounts[2], accounts[5]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
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
                    var correct = new[] {accounts[0], accounts[3], accounts[5], accounts[7]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[3])) == 1);
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
        public void NullableULong_Equal_To_Explicit_Value_Forbidden_In_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                        db.InsertItem(account);

                    db.Find<AccountsData>(a => a.Salary == 1900);
                });
                Assert.That(ex.Message, Contains.Substring("or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate statements."));
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
                    var correct = new[] {accounts[0], accounts[5]};
                    Assert.IsTrue(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.IsTrue(correct.Count(c => c.Equals(table[1])) == 1);
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
                    Assert.IsTrue(table[0].Equals(accounts[5]));
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
        public async Task NullableDecimal_Equal_To_Null_Predicate()
        {
            var item1 = DecimalNumbers.GetDefault();
            var item2 = DecimalNumbers.GetDefault();
            item2.NullAble2 = 9492893892.29384928m;
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<DecimalNumbers>();
                    await db.CreateTableAsync<DecimalNumbers>();

  
                    await db.InsertItemAsync(item1);
                    await db.InsertItemAsync(item2);

                    var result = await db.FindAsync<DecimalNumbers>(a => a.NullAble2 == null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(item1));

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
        public async Task NullableDecimal_Not_Equal_To_Null_Predicate()
        {
            var item1 = DecimalNumbers.GetDefault();
            var item2 = DecimalNumbers.GetDefault();
            item2.NullAble2 = 9492893892.29384928m;
            foreach (var db in GetAsyncConnections())
            {
                try
                {
                    await db.DeleteTableAsync<DecimalNumbers>();
                    await db.CreateTableAsync<DecimalNumbers>();


                    await db.InsertItemAsync(item1);
                    await db.InsertItemAsync(item2);

                    var result = await db.FindAsync<DecimalNumbers>(a => a.NullAble2 != null);

                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(item2));

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
        public void NullableDecimal_Equal_To_Explicit_Value_Forbidden_In_Predicate()
        {
            var item1 = DecimalNumbers.GetDefault();
            var item2 = DecimalNumbers.GetDefault();
            item2.NullAble2 = 9492893892.29384928m;
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<DecimalNumbers>();
                    db.CreateTable<DecimalNumbers>();
                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.Find<DecimalNumbers>(a => a.NullAble2 == 1231312333m);
                });

                Assert.That(ex.Message, Contains.Substring("or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate statements."));
            }
        }

        [Test]
        public void OrderByColumnCanNotBeEncrypted()
        {
            using (var db = GetAes128Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<AccountsData>();
                    db.Find<AccountsData>(d => d.Age < 100, d => d.Password);
                });
                Assert.That(ex.Message, Contains.Substring("Order By column can't be encrypted."));
            }
        }

        [Test]
        public void LimitNumberCanNotBeLessOrEqualToZero()
        {
            using (var db = GetAes128Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<AccountsData>();
                    db.Find<AccountsData>(d => d.Age < 100, 0);
                });
                Assert.That(ex.Message, Contains.Substring("Limit number can't be less or equal to 0."));
            }
        }

        [Test]
        public void FindUsingPredicateWithOrderByColumnAscending()
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

                    var result = db.Find<AccountsData>(a => a.Age < 80, a => a.Age);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 8);

                    Assert.IsTrue(table[0].Equals(accounts[2]));
                    Assert.IsTrue(table[1].Equals(accounts[0]));
                    Assert.IsTrue(table[2].Equals(accounts[1]));
                    Assert.IsTrue(table[3].Equals(accounts[5]));
                    Assert.IsTrue(table[4].Equals(accounts[6]));
                    Assert.IsTrue(table[5].Equals(accounts[3]));
                    Assert.IsTrue(table[6].Equals(accounts[7]));
                    Assert.IsTrue(table[7].Equals(accounts[4]));
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
        public void FindUsingPredicateWithOrderByColumnDescending()
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

                    var result = db.Find<AccountsData>(a => a.Age < 80, a => a.Age, SortOrder.Desc);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 8);
                    Assert.IsTrue(table[0].Equals(accounts[4]));
                    Assert.IsTrue(table[1].Equals(accounts[7]));
                    Assert.IsTrue(table[2].Equals(accounts[3]));
                    Assert.IsTrue(table[3].Equals(accounts[6]));
                    Assert.IsTrue(table[4].Equals(accounts[5]));
                    Assert.IsTrue(table[5].Equals(accounts[1]));
                    Assert.IsTrue(table[6].Equals(accounts[0]));
                    Assert.IsTrue(table[7].Equals(accounts[2]));
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
        public void FindUsingPredicateWithOrderByColumnAndLimitNumber()
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

                    var result = db.Find<AccountsData>(a => a.Age < 80, 5, a => a.Age);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].Equals(accounts[2]));
                    Assert.IsTrue(table[1].Equals(accounts[0]));
                    Assert.IsTrue(table[2].Equals(accounts[1]));
                    Assert.IsTrue(table[3].Equals(accounts[5]));
                    Assert.IsTrue(table[4].Equals(accounts[6]));
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
