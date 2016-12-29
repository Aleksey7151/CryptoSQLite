using System;
using System.Linq;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class DeleteItemTests : BaseTest
    {
        [Test]
        public void DeleteItemColumnNameCanNotBeNull()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                    db.DeleteItem<SecretTask>(null, 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >=
                        0);
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
        public void DeleteItemColumnNameCanNotBeEmpty()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                    db.DeleteItem<SecretTask>("", 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >=
                        0);
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
        public void DeleteItemColumnValueCanNotBeNull()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                    db.DeleteItem<SecretTask>("Id", null);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column value can't be null.", StringComparison.Ordinal) >=
                        0);
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
        public void DeleteItemUsingIncorrectColumnName()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                    db.DeleteItem<SecretTask>("IT", 2);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("doesn't contain column with name", StringComparison.Ordinal) >=
                        0);
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
        public void DeleteItemUsingEncryptedColumn()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);

                    db.DeleteItem<SecretTask>("SecretToDo", "Hello, world!");
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("You can't use [Encrypted] column as a column in which the columnValue should be deleted.", StringComparison.Ordinal) >=
                        0);
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
        public void DeleteItemUsingColumnName()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);


                    db.DeleteItem<SecretTask>("Price", 24523.123);

                    var elements = db.Table<SecretTask>().ToArray();

                    Assert.IsTrue(elements.Length == tasks.Length - 1);

                    Assert.IsTrue(elements[0].Equal(tasks[0]));
                    Assert.IsTrue(elements[1].Equal(tasks[2]));
                    Assert.IsTrue(elements[2].Equal(tasks[3]));

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
        public void DeleteItemUsingId()
        {
            var tasks = GetTasks();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    foreach (var task in tasks)
                        db.InsertItem(task);


                    db.DeleteItem<SecretTask>("Id",1);

                    var elements = db.Table<SecretTask>().ToArray();

                    Assert.IsTrue(elements.Length == tasks.Length - 1);

                    Assert.IsTrue(elements[0].Equal(tasks[1]));
                    Assert.IsTrue(elements[1].Equal(tasks[2]));
                    Assert.IsTrue(elements[2].Equal(tasks[3]));

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
