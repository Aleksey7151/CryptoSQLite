using System;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    
    public class CountTests : BaseTest
    {
        [Fact]
        public void CountOfAllRecordsInTable()
        {
            var item1 = IntNumbers.GetDefault();
            var item2 = IntNumbers.GetDefault();
            var item3 = IntNumbers.GetDefault();
            var item4 = IntNumbers.GetDefault();

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.InsertItem(item3);
                    db.InsertItem(item4);

                    var cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 4);

                    db.Delete<IntNumbers>(i => i.Id == 4);

                    cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 3);

                    db.Delete<IntNumbers>(i => i.Id == 3);

                    cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 2);

                    db.Delete<IntNumbers>(i => i.Id == 2);

                    cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 1);

                    db.Delete<IntNumbers>(i => i.Id == 1);

                    cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 0);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void CountOfRecordsInTableForSpecifiedColumn()
        {
            var item1 = IntNumbers.GetDefault();
            item1.NullAble1 = null;
            var item2 = IntNumbers.GetDefault();
            var item3 = IntNumbers.GetDefault();
            item3.NullAble1 = null;
            var item4 = IntNumbers.GetDefault();

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.InsertItem(item3);
                    db.InsertItem(item4);

                    var cnt = db.Count<IntNumbers>("NullAble1");

                    Assert.True(cnt == 2);

                    db.Delete<IntNumbers>(i => i.Id == 4);

                    cnt = db.Count<IntNumbers>("NullAble1");

                    Assert.True(cnt == 1);

                    db.Delete<IntNumbers>(i => i.Id == 3);

                    cnt = db.Count<IntNumbers>("NullAble1");

                    Assert.True(cnt == 1);

                    db.Delete<IntNumbers>(i => i.Id == 2);

                    cnt = db.Count<IntNumbers>("NullAble1");

                    Assert.True(cnt == 0);

                    db.Delete<IntNumbers>(i => i.Id == 1);

                    cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 0);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void CountOfDistinctRecordsInTableForSpecifiedColumn()
        {
            var item1 = IntNumbers.GetDefault();
            var item2 = IntNumbers.GetDefault();
            var item3 = IntNumbers.GetDefault();
            item3.NullAble1 = null;
            var item4 = IntNumbers.GetDefault();

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.InsertItem(item3);
                    db.InsertItem(item4);

                    var cnt = db.CountDistinct<IntNumbers>("NullAble1");

                    Assert.True(cnt == 1);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void CountOfRecordsWithCondition()
        {
            var item1 = IntNumbers.GetDefault();
            item1.IntMaxVal = 9;
            var item2 = IntNumbers.GetDefault();
            item2.IntMaxVal = 7;
            var item3 = IntNumbers.GetDefault();
            item3.IntMaxVal = 13;
            var item4 = IntNumbers.GetDefault();
            item4.IntMaxVal = 7;

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.InsertItem(item3);
                    db.InsertItem(item4);

                    var cnt = db.Count<IntNumbers>(t => t.IntMaxVal < 10);

                    Assert.True(cnt == 3);

                    cnt = db.Count<IntNumbers>(t => t.IntMaxVal > 10);

                    Assert.True(cnt == 1);

                    cnt = db.Count<IntNumbers>(t => t.IntMaxVal == 7);

                    Assert.True(cnt == 2);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
