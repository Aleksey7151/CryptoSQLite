using System;
using System.Linq;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class GetItemTests : BaseTest
    {
        [Test]
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
                        Assert.IsTrue(tasks[i].Equal(elements[i]));
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
