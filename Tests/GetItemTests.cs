using System;
using System.Linq;
using CryptoSQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Tables;

namespace Tests
{
    [TestClass]
    public class GetItemTests : BaseTest
    {
        [TestMethod]
        public void GetItemColumnNameCanNotBeNull()
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

                    db.GetItem<SecretTask>(null, 123);
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

        [TestMethod]
        public void GetItemColumnNameCanNotBeEmpty()
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

                    db.GetItem<SecretTask>("", 123);
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

        [TestMethod]
        public void GetItemColumnValueCanNotBeNull()
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

                    db.GetItem<SecretTask>("Id", null);
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

        [TestMethod]
        public void GetItemUsingIncorrectColumnName()
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

                    db.GetItem<SecretTask>("IT", 2);
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

        [TestMethod]
        public void GetItemUsingEncryptedColumn()
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

                    db.GetItem<SecretTask>("SecretToDo", "Hello, world!");
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("You can't use [Encrypted] column as a column in which the columnValue should be found.", StringComparison.Ordinal) >=
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

        [TestMethod]
        public void GetItemUsingColumnName()
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


                    var element = db.GetItem<SecretTask>("Id", 2);
                    
                    Assert.IsNotNull(element);
                    Assert.IsTrue(tasks[1].IsTaskEqualTo(element));

                    element = db.GetItem<SecretTask>("IsDone", true);
                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.IsTaskEqualTo(tasks[0]));
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

        [TestMethod]
        public void GetItemUsingId()
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

                    for (var i = 1; i <= tasks.Length; i++)
                    {
                        var element = db.GetItem<SecretTask>(i);
                        Assert.IsNotNull(element);
                        Assert.IsTrue(tasks[i-1].IsTaskEqualTo(element));
                    }
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

        [TestMethod]
        public void GetItemUsingItemInstance()
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

                    var item = new SecretTask {Price = 24523.123};
                    item = db.GetItem(item);

                    Assert.IsNotNull(item);
                    Assert.IsTrue(item.IsTaskEqualTo(tasks[1]));
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

        public void GetItemReturns_Null_IfItemDoesNotExistsInDataBase()
        {
            
        }
       
        [TestMethod]
        public void GetAllItems()
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


                    var elements = db.Table<SecretTask>().ToArray();

                    Assert.IsNotNull(elements);
                    Assert.IsTrue(tasks.Length == elements.Length);
                    for (var i = 0; i < elements.Length; i++)
                        Assert.IsTrue(tasks[i].IsTaskEqualTo(elements[i]));
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
