using System;
using System.Linq;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [CryptoTable("FullTextSearch3")]
    internal class SearchTable
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Encrypted]
        public string SomeData { get; set; }

        public bool Equals(SearchTable st)
        {
            return SomeData == st.SomeData;
        }
    }
    [TestFixture]
    public class FullTextSearchTests : BaseTest
    {

        [Test]
        public void CreateTableUsingFts3()
        {
            /*
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            */
            var st1 = new SearchTable { Id = 1, SomeData = "Some Data 1"};
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SearchTable>();

                    db.CreateTable<SearchTable>(FullTextSearchFlags.FTS3);

                    db.InsertItem(st1);

                    //db.InsertItem(st2);
                    //db.InsertItem(st3);

                    var table = db.Table<SearchTable>()?.ToList();
                    Assert.NotNull(table);
                    Assert.IsTrue(table.Count == 1);
                    Assert.IsTrue(table[0].Equals(st1));
                    //Assert.IsTrue(table[1].Equal(st2));
                    //Assert.IsTrue(table[2].Equal(st3));
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message);
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
        
        

        /*
        [Test]
        public void CreateTableUsingFts4()
        {

        }

        [Test]
        public void CreateTwoTablesSimulteniasly()
        {
            
        }
        */
    }

}
