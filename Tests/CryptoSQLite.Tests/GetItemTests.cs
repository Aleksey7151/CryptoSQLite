using System.Linq;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    [Collection("Sequential")]
    public class GetItemTests : BaseTest
    {
        [Fact]
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

                    Assert.NotNull(elements);
                    Assert.True(tasks.Length == elements.Length);

                    for (var i = 0; i < elements.Length; i++)
                        Assert.True(tasks[i].Equal(elements[i]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
