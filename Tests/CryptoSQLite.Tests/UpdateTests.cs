using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.Tests
{
    [TestFixture]
    public class UpdateTests : BaseTest
    {
        [Test]
        public void Update_Using_Not_Equal_To_Null_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };

            var newItem = new SecretTask { IsDone = false, Price = 19191, Description = "Updated descriptionen", SecretToDo = "Updated Secret to DO." };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    db.InsertItem(st1);
                    db.InsertItem(st2);
                    db.InsertItem(st3);

                    db.Update(newItem, t => t.IsDone);
                    var result = db.Table<SecretTask>();

                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(newItem) && table[1].Equal(st2) && table[2].Equal(newItem));
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
        public void Update_Using_Composite_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 1.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            var st4 = new SecretTask { IsDone = false, Price = 12.0, Description = "Description 3", SecretToDo = "Some Secret Task" };
            var st5 = new SecretTask { IsDone = true, Price = 17.7, Description = null, SecretToDo = "Some Secret Task" };
            var st6 = new SecretTask { IsDone = true, Price = 22.02, Description = "Description 5", SecretToDo = "Some Secret Task" };


            var newItem = new SecretTask { IsDone = false, Price = 19191, Description = "Updated descriptionen", SecretToDo = "Updated Secret to DO." };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    db.InsertItem(st1);
                    db.InsertItem(st2);
                    db.InsertItem(st3);
                    db.InsertItem(st4);
                    db.InsertItem(st5);
                    db.InsertItem(st6);

                    db.Update(newItem, t => t.IsDone && t.Price < 5);
                    var result = db.Table<SecretTask>();

                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 6);
                    Assert.IsTrue(table[0].Equal(newItem) && table[1].Equal(st2) && table[2].Equal(st3) && table[3].Equal(st4) && table[4].Equal(st5) && table[5].Equal(st6));
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
        public void Update_Using_LessThan_Predicate()
        {
            var st1 = new SecretTask { IsDone = true, Price = 1.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 8.0, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 11.99, Description = "Description 2", SecretToDo = "Some Secret Task" };


            var newItem = new SecretTask { IsDone = false, Price = 19191, Description = "Updated descriptionen", SecretToDo = "Updated Secret to DO." };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    db.InsertItem(st1);
                    db.InsertItem(st2);
                    db.InsertItem(st3);

                    db.Update(newItem, t => t.Price < 9);
                    var result = db.Table<SecretTask>();

                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 3);
                    Assert.IsTrue(table[0].Equal(newItem) && table[1].Equal(newItem) && table[2].Equal(st3));
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
