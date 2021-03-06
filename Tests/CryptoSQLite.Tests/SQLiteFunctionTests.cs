﻿using System;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    [Collection("Sequential")]
    public class SQLiteFunctionsTests : BaseTest
    {
        [Fact]
        public void SQLite_COUNT_WithPredicate_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };
            var item5 = new IntNumbers { IntMinVal = 37 };
            var item6 = new IntNumbers { IntMinVal = 54 };

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
                    db.InsertItem(item5);
                    db.InsertItem(item6);

                    var cnt = db.Count<IntNumbers>(i => i.IntMinVal < 40);

                    Assert.True(cnt == 3);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void SQLite_TOTAL_COUNT_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };
            var item5 = new IntNumbers { IntMinVal = 37 };
            var item6 = new IntNumbers { IntMinVal = 54 };

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
                    db.InsertItem(item5);
                    db.InsertItem(item6);

                    var cnt = db.Count<IntNumbers>();

                    Assert.True(cnt == 6);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void SQLite_MAX_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };

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

                    var max = db.Max<IntNumbers>("IntMinVal");

                    Assert.True(Math.Abs(max - 83) < 0.0000001);

                    var maxPrid = db.Max<IntNumbers>("IntMinVal", t => t.Id < 3);

                    Assert.True(Math.Abs(maxPrid - 44) < 0.0000001);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void SQLite_MIN_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };

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

                    var min = db.Min<IntNumbers>("IntMinVal");

                    Assert.True(Math.Abs(min - 7) < 0.0000001);

                    var minPrid = db.Min<IntNumbers>("IntMinVal", t => t.Id > 1 && t.Id < 4);

                    Assert.True(Math.Abs(minPrid - 13) < 0.0000001);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void SQLite_SUM_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };

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

                    var max = db.Sum<IntNumbers>("IntMinVal");

                    Assert.True(Math.Abs(max - 147) < 0.0000001);

                    var maxPrid = db.Sum<IntNumbers>("IntMinVal", t => t.Id > 1 && t.Id < 4);

                    Assert.True(Math.Abs(maxPrid - 96) < 0.0000001);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void SQLite_AVG_Function()
        {
            var item1 = new IntNumbers { IntMinVal = 44 };
            var item2 = new IntNumbers { IntMinVal = 13 };
            var item3 = new IntNumbers { IntMinVal = 83 };
            var item4 = new IntNumbers { IntMinVal = 7 };

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

                    var max = db.Avg<IntNumbers>("IntMinVal");

                    Assert.True(Math.Abs(max - 36.75) < 0.0000001);

                    var maxPrid = db.Avg<IntNumbers>("IntMinVal", t => t.Id > 1 && t.Id < 4);

                    Assert.True(Math.Abs(maxPrid - 48) < 0.0000001);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
