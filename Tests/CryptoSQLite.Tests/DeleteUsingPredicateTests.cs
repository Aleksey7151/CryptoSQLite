using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.Tests
{
    [TestFixture]
    public class DeleteUsingPredicateTests : BaseTest
    {
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
                    db.Delete<ULongNumbers>(null);
                });
                Assert.That(ex.Message, Contains.Substring("Predicate can't be null"));
            }
        }

        [Test]
        public void Delete_Whet_Table_Does_Not_Exist()
        {
            using (var db = GetAes256Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<ULongNumbers>();
                    db.Delete<ULongNumbers>(i => i.Id == 1);
                });
                Assert.That(ex.Message, Contains.Substring("Database doesn't contain table with name:"));
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
                    db.Delete<AccountsData>(a => a.Password == "Pass");
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
                    db.Delete<ULongNumbers>(i => i.ULongMaxVal == 1900);
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
                    db.Delete<LongNumbers>(i => i.LongMaxVal == 1900);
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
                    db.Delete<DateTimeTable>(i => i.Date == now);
                });
                Assert.That(ex.Message, Contains.Substring("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements."));
            }
        }

        [Test]
        public void Delete_Using_Equal_To_Null_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = null, SecretToDo = "Some Secret Task" };
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

                     db.Delete<SecretTask>(a => a.Description == null);

                    var result =  db.Table<SecretTask>();
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
        public void Delete_Using_Not_Equal_To_Null_Predicate()
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

                     db.Delete<SecretTask>(a => a.Description != null);
                    var result =  db.Table<SecretTask>();
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
        public void Delete_Using_Equal_To_True_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = null, SecretToDo = "Some Secret Task" };
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

                     db.Delete<SecretTask>(a => a.IsDone);

                    var result =  db.Table<SecretTask>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equal(st2));
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
        public void Delete_Using_Equal_To_False_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = null, SecretToDo = "Some Secret Task" };
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

                     db.Delete<SecretTask>(a => !a.IsDone);

                    var result =  db.Table<SecretTask>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 2);
                    Assert.IsTrue(table[0].Equal(st1));
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
        public void Delete_Using_Equal_To_Explicit_String_Predicate()
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

                     db.Delete<AccountsData>(a => a.Name == "Account0");
                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 5);
                    Assert.IsTrue(table[0].Equals(accounts[1]));
                    Assert.IsTrue(table[1].Equals(accounts[2]));
                    Assert.IsTrue(table[2].Equals(accounts[4]));
                    Assert.IsTrue(table[3].Equals(accounts[5]));
                    Assert.IsTrue(table[4].Equals(accounts[7]));
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
        public void Delete_Using_LessThan_Predicate()
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

                     db.Delete<AccountsData>(a => a.Age < 27);
                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equals(accounts[3]));
                    Assert.IsTrue(table[1].Equals(accounts[4]));
                    Assert.IsTrue(table[2].Equals(accounts[7]));
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
        public void Delete_Using_GreaterThan_Or_Equal_Predicate()
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

                     db.Delete<AccountsData>(a => a.Age >= 25);
                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 4);
                    Assert.IsTrue(table[0].Equals(accounts[0]));
                    Assert.IsTrue(table[1].Equals(accounts[1]));
                    Assert.IsTrue(table[2].Equals(accounts[2]));
                    Assert.IsTrue(table[3].Equals(accounts[5]));
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
        public void Delete_Using_Inclusive_Between_Two_Values_Predicate()
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

                     db.Delete<AccountsData>(a => a.Posts <= 60 && a.Posts >= 40);

                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equals(accounts[3]));
                    Assert.IsTrue(table[1].Equals(accounts[4]));
                    Assert.IsTrue(table[2].Equals(accounts[7]));
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
        public void Delete_Using_Inclusive_LessThan_Or_GreaterThan_Predicate()
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

                     db.Delete<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equals(accounts[0]));
                    Assert.IsTrue(table[1].Equals(accounts[1]));
                    Assert.IsTrue(table[2].Equals(accounts[5]));
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
        public void Delete_Using_Double_Greater_Than_Explicit_Value_Predicate()
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

                     db.Delete<AccountsData>(a => a.Productivity > 1605.005);

                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equals(accounts[1]));
                    Assert.IsTrue(table[1].Equals(accounts[2]));
                    Assert.IsTrue(table[2].Equals(accounts[7]));
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
        public void NullableULong_And_UShort_Equal_To_NUll_And_LessThan_Predicate()
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

                     db.Delete<AccountsData>(a => a.Salary == null || a.Age > 27);

                    var result =  db.Table<AccountsData>();
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equals(accounts[1]));
                    Assert.IsTrue(table[1].Equals(accounts[2]));
                    Assert.IsTrue(table[2].Equals(accounts[6]));
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
