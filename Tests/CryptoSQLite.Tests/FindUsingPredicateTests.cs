using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    [Collection("Sequential")]
    public class FindUsingPredicateTests : BaseTest
    {

        [Fact]
        public void Empty_If_Not_Found()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                    {
                        db.InsertItem(account);
                    }

                    var result = db.Find<AccountsData>(a => a.Name == "Frodo");

                    var table = result.ToArray();
                    Assert.True(table.Length == 0);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
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
                Assert.Contains("Predicate can't be null", ex.Message);
            }
        }

        [Fact]
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
                Assert.Contains("You can't use Encrypted columns for finding elements in database", ex.Message);
            }
        }

        [Fact]
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
                Assert.Contains("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements.", ex.Message);
            }
        }

        [Fact]
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
                Assert.Contains("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements.", ex.Message);
            }
        }

        [Fact]
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

                Assert.Contains("Properties with types 'UInt64', 'Int64', 'DateTime', 'Decimal' can't be used in Predicates for finding elements.", ex.Message);
            }
        }

        [Fact]
        public void Strings_Find_Using_Equal_To_Null_Predicate()
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

                    var result = db.Find<SecretTask>(a => a.Description == null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equal(st1));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Strings_Find_Using_Not_Equal_To_Null_Predicate()
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

                    var result = db.Find<SecretTask>(a => a.Description != null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 2);
                    Assert.True(table[0].Equal(st2));
                    Assert.True(table[1].Equal(st3));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Strings_Find_Using_Equal_To_Explicit_String_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                    {
                        db.InsertItem(account);
                    }

                    var result = db.Find<AccountsData>(a => a.Name == "Account1");

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[1]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Strings_Multiple_Results_Find_Using_Equal_To_Explicit_String_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                    {
                        db.InsertItem(account);
                    }

                    var result = db.Find<AccountsData>(a => a.Name == "Account0");

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[0], accounts[3], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Strings_Multiple_Results_Find_Using_Equal_To_Explicit_String_And_Age_Less_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                    {
                        db.InsertItem(account);
                    }

                    var result = db.Find<AccountsData>(a => a.Name == "Account0" && a.Age < 27);

                    var table = result.ToArray();
                    Assert.True(table.Length == 2);
                    var correct = new[] { accounts[0], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Bools_Find_Using_Equal_To_True_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                    {
                        db.InsertItem(account);
                    }

                    var result = db.Find<AccountsData>(a => a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[0], accounts[3], accounts[4], accounts[6], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Bools_Find_Using_Equal_To_False_Predicate()
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

                    var result = db.Find<AccountsData>(a => a.IsAdministrator == false);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[1], accounts[2], accounts[5] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Bools_Find_Using_Equal_Not_True_Predicate()
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

                    var result = db.Find<AccountsData>(a => !a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[1], accounts[2], accounts[5] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableUInt_Equal_To_Null_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.ClearTable<AccountsData>();
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                        db.InsertItem(account);

                    var result = db.Find<AccountsData>(a => a.SocialSecureId == null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[2], accounts[5], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableUInt_Equal_Not_Null_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.ClearTable<AccountsData>();
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                        db.InsertItem(account);

                    var result = db.Find<AccountsData>(a => a.SocialSecureId != null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[0], accounts[1], accounts[3], accounts[4], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableUInt_Equal_To_Explicit_Value_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.ClearTable<AccountsData>();
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                        db.InsertItem(account);

                    var result = db.Find<AccountsData>(a => a.SocialSecureId == 4);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[4]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_Equal_To_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Posts == 45);

                    var table = result.ToArray();
                    Assert.True(table.Length == 2);
                    Assert.True(table[0].Equals(accounts[0]));
                    Assert.True(table[1].Equals(accounts[5]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_LessThan_Explicit_Value_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts < 50);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);

                    var correct = new[] { accounts[0], accounts[2], accounts[3], accounts[5] };

                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_LessThan_Or_Equal_To_Explicit_Value_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts <= 50);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[0], accounts[1], accounts[2], accounts[3], accounts[5] };

                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_GreaterThan_Explicit_Value_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts > 50);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[4], accounts[6], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_GreaterThan_Or_Equal_To_Explicit_Value_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts >= 50);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);
                    var correct = new[] { accounts[1], accounts[4], accounts[6], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_Between_Two_Values_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts < 60 && a.Posts > 45);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[1]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_Inclusive_Between_Two_Values_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts <= 60 && a.Posts >= 45);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);
                    var correct = new[] { accounts[0], accounts[1], accounts[5], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_Inclusive_LessThan_Or_GreaterThan_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();
                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts >= 60 || a.Posts <= 40);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[2], accounts[3], accounts[4], accounts[6], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_Exclusive_LessThan_Or_GreaterThan_Predicate()
        {
            var accounts = GetAccounts();
            foreach (var db in GetConnections())
            {
                try
                {
                     db.ClearTable<AccountsData>();
                     db.DeleteTable<AccountsData>();

                     db.CreateTable<AccountsData>();

                    foreach (var account in accounts)
                         db.InsertItem(account);

                    var result =  db.Find<AccountsData>(a => a.Posts > 60 || a.Posts < 45);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);

                    var correct = new[] { accounts[2], accounts[3], accounts[4], accounts[7] };

                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_Equal_To_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity == 1778.99998);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[0]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_Less_Than_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity < 1778.99998);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);
                    var correct = new[] { accounts[1], accounts[2], accounts[5], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_LessOrEqual_To_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity <= 1778.99998);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[0], accounts[1], accounts[2], accounts[5], accounts[7] };

                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_Greater_Than_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity > 1778.99998);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[3], accounts[4], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_GreaterOrEqual_Than_Explicit_Value_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity >= 1778.99998);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);
                    var correct = new[] { accounts[0], accounts[3], accounts[4], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_Between_Explicit_Values_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity > 1700 && a.Productivity < 1900);

                    var table = result.ToArray();
                    Assert.True(table.Length == 3);
                    var correct = new[] { accounts[0], accounts[3], accounts[6] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_Greater_Or_Less_Explicit_Values_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Productivity < 1700 || a.Productivity > 1900);

                    var table = result.ToArray();
                    Assert.True(table.Length == 5);
                    var correct = new[] { accounts[1], accounts[2], accounts[4], accounts[5], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[4])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableInt_And_Bool_Equal_To_Null_And_True_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[7]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableInt_And_Bool_Equal_To_Null_And_False_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.SocialSecureId == null && a.IsAdministrator == false);

                    var table = result.ToArray();
                    Assert.True(table.Length == 2);
                    var correct = new[] { accounts[2], accounts[5] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableULong_Equal_To_Null_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Salary == null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 4);
                    var correct = new[] { accounts[0], accounts[3], accounts[5], accounts[7] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[2])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[3])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
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
                Assert.Contains("or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate statements.", ex.Message);
            }
        }

        [Fact]
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

                    var result =  db.Find<AccountsData>(a => a.Salary == null && a.Age < 27);

                    var table = result.ToArray();
                    Assert.True(table.Length == 2);
                    var correct = new[] { accounts[0], accounts[5] };
                    Assert.True(correct.Count(c => c.Equals(table[0])) == 1);
                    Assert.True(correct.Count(c => c.Equals(table[1])) == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableULong_And_UShort_Equal_To_NUll_And_Between_Predicate()
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

                    var result =  db.Find<AccountsData>(a => a.Salary == null && a.Age < 27 && a.Age > 20);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(accounts[5]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableDecimal_Equal_To_Null_Predicate()
        {
            var item1 = DecimalNumbers.GetDefault();
            var item2 = DecimalNumbers.GetDefault();
            item2.NullAble2 = 9492893892.29384928m;
            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<DecimalNumbers>();
                     db.CreateTable<DecimalNumbers>();


                     db.InsertItem(item1);
                     db.InsertItem(item2);

                    var result =  db.Find<DecimalNumbers>(a => a.NullAble2 == null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(item1));

                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NullableDecimal_Not_Equal_To_Null_Predicate()
        {
            var item1 = DecimalNumbers.GetDefault();
            var item2 = DecimalNumbers.GetDefault();
            item2.NullAble2 = 9492893892.29384928m;
            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<DecimalNumbers>();
                     db.CreateTable<DecimalNumbers>();


                     db.InsertItem(item1);
                     db.InsertItem(item2);

                    var result =  db.Find<DecimalNumbers>(a => a.NullAble2 != null);

                    var table = result.ToArray();
                    Assert.True(table.Length == 1);
                    Assert.True(table[0].Equals(item2));

                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
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

                Assert.Contains("or 'Byte[]' can be used only in Equal To NULL (==null) or Not Equal To NULL (!=null) Predicate statements.", ex.Message);
            }
        }

        [Fact]
        public void OrderByColumnCanNotBeEncrypted()
        {
            using (var db = GetAes128Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<AccountsData>();
                    db.Find<AccountsData>(d => d.Age < 100, d => d.Password);
                });
                Assert.Contains("Order By column can't be encrypted.", ex.Message);
            }
        }

        [Fact]
        public void LimitNumberCanNotBeLessOrEqualToZero()
        {
            using (var db = GetAes128Connection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<AccountsData>();
                    db.Find<AccountsData>(d => d.Age < 100, 0);
                });
                Assert.Contains("Limit number can't be less or equal to 0.", ex.Message);
            }
        }

        [Fact]
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
                    Assert.True(table.Length == 8);

                    Assert.True(table[0].Equals(accounts[2]));
                    Assert.True(table[1].Equals(accounts[0]));
                    Assert.True(table[2].Equals(accounts[1]));
                    Assert.True(table[3].Equals(accounts[5]));
                    Assert.True(table[4].Equals(accounts[6]));
                    Assert.True(table[5].Equals(accounts[3]));
                    Assert.True(table[6].Equals(accounts[7]));
                    Assert.True(table[7].Equals(accounts[4]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
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
                    Assert.True(table.Length == 8);
                    Assert.True(table[0].Equals(accounts[4]));
                    Assert.True(table[1].Equals(accounts[7]));
                    Assert.True(table[2].Equals(accounts[3]));
                    Assert.True(table[3].Equals(accounts[6]));
                    Assert.True(table[4].Equals(accounts[5]));
                    Assert.True(table[5].Equals(accounts[1]));
                    Assert.True(table[6].Equals(accounts[0]));
                    Assert.True(table[7].Equals(accounts[2]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
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
                    Assert.True(table.Length == 5);
                    Assert.True(table[0].Equals(accounts[2]));
                    Assert.True(table[1].Equals(accounts[0]));
                    Assert.True(table[2].Equals(accounts[1]));
                    Assert.True(table[3].Equals(accounts[5]));
                    Assert.True(table[4].Equals(accounts[6]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
