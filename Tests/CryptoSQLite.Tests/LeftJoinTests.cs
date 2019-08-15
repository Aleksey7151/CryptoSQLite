using System;
using System.Linq;
using NUnit.Framework;

namespace CryptoSQLite.Tests
{
    #region Joining Tables

    [CryptoTable("RightTable")]
    public class RightTable
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public int IntJoinKey { get; set; }

        public string StringJoinKey { get; set; }

        [Encrypted]
        public string SomeData { get; set; }

        [Encrypted]
        public uint SomeInt { get; set; }

        public bool Equals(RightTable ot)
        {
            return IntJoinKey == ot.IntJoinKey && StringJoinKey == ot.StringJoinKey && SomeData == ot.SomeData &&
                   SomeInt == ot.SomeInt;
        }
    }

    [CryptoTable("LeftTable")]
    public class LeftTable
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public int IntJoinKey { get; set; }

        public string StringJoinKey { get; set; }

        [Encrypted]
        public string SomeData { get; set; }

        [Encrypted]
        public int SomeInt { get; set; }

        public bool Equals(LeftTable it)
        {
            return IntJoinKey == it.IntJoinKey &&
                   StringJoinKey == it.StringJoinKey &&
                   SomeData == it.SomeData &&
                   SomeInt == it.SomeInt;
        }
    }

    public class LeftJoinResult
    {
        public RightTable Right { get; set; }

        public LeftTable Left { get; set; }

        public LeftJoinResult(LeftTable left, RightTable right)
        {
            Right = right;
            Left = left;
        }

    }

    #endregion

    [TestFixture]
    public class LeftJoinTests : BaseTest
    {
        [Test]
        public void CanNotUseEncryptedColumnsInJoiningConditions()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<RightTable>();
                    db.CreateTable<LeftTable>();

                    var leftJoin = db.LeftJoin<LeftTable, RightTable, LeftJoinResult>(null, (left, right) => left.SomeData/*Has Encrypted Attribute*/ == right.SomeData,
                                (left, right) => new LeftJoinResult(left, right));
                });
                Assert.That(ex.Message, Contains.Substring("Columns that are used in joining expressions can't be Encrypted"));
            }
        }

        [Test]
        public void CanNotJoinSameTables()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<LeftTable>();

                    var leftJoin = db.LeftJoin<LeftTable, LeftTable, LeftJoinResult>(null, (left, right) => left.SomeData/*Has Encrypted Attribute*/ == right.SomeData,
                                (left, right) => null);
                });
                Assert.That(ex.Message, Contains.Substring("You can't join table with itself."));
            }
        }

        [Test]
        public void LeftJoinTablesUsingInt()
        {

            var rightTable1 = new RightTable { IntJoinKey = 1, SomeData = "Some data for check 1", SomeInt = 12311, StringJoinKey = "Key1" };
            var rightTable2 = new RightTable { IntJoinKey = 2, SomeData = "Some data for check 2", SomeInt = 26622, StringJoinKey = "Key2" };
            var rightTable3 = new RightTable { IntJoinKey = 3, SomeData = "Some data for check 3", SomeInt = 57474, StringJoinKey = "Key3" };


            var leftTable1 = new LeftTable { IntJoinKey = 1, SomeData = "Some Inner data 1 - 1", SomeInt = 45333, StringJoinKey = "Tra-la-la" };
            var leftTable2 = new LeftTable { IntJoinKey = 4, SomeData = "Some Inner data 2 - 1", SomeInt = 54744, StringJoinKey = "Key2" };
            var leftTable3 = new LeftTable { IntJoinKey = 0, SomeData = "Some Inner data 3 - 1", SomeInt = 34533, StringJoinKey = null };

            var leftTable4 = new LeftTable { IntJoinKey = 3, SomeData = "Some Inner data 1 - 2", SomeInt = 1110020, StringJoinKey = "Key1" };
            var leftTable5 = new LeftTable { IntJoinKey = 2, SomeData = "Some Inner data 2 - 2", SomeInt = 2011111, StringJoinKey = "Key3" };

            var leftTable6 = new LeftTable { IntJoinKey = 1, SomeData = "Some Inner data 1 - 3", SomeInt = 9383, StringJoinKey = "Bilbo Beggins" };
            var leftTable7 = new LeftTable { IntJoinKey = 5, SomeData = "Some Inner data 3 - 3", SomeInt = 345, StringJoinKey = "Key2" };

            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<RightTable>();
                     db.DeleteTable<LeftTable>();

                     db.CreateTable<RightTable>();
                     db.CreateTable<LeftTable>();

                     db.InsertItem(rightTable1);
                     db.InsertItem(rightTable2);
                     db.InsertItem(rightTable3);

                     db.InsertItem(leftTable1);
                     db.InsertItem(leftTable2);
                     db.InsertItem(leftTable3);
                     db.InsertItem(leftTable4);
                     db.InsertItem(leftTable5);
                     db.InsertItem(leftTable6);
                     db.InsertItem(leftTable7);


                    var leftJoin =  db.LeftJoin<LeftTable, RightTable, LeftJoinResult>(null, (left, right) => left.IntJoinKey == right.IntJoinKey,
                                (left, right) => new LeftJoinResult(left, right));

                    Assert.IsTrue(leftJoin != null);

                    var result = leftJoin.ToArray();

                    Assert.IsTrue(result.Length == 7);

                    Assert.IsTrue(result[0].Left.Equals(leftTable1));
                    Assert.IsTrue(result[0].Right.Equals(rightTable1));

                    Assert.IsTrue(result[1].Left.Equals(leftTable2));
                    Assert.IsTrue(result[1].Right == null);

                    Assert.IsTrue(result[2].Left.Equals(leftTable3));
                    Assert.IsTrue(result[2].Right == null);

                    Assert.IsTrue(result[3].Left.Equals(leftTable4));
                    Assert.IsTrue(result[3].Right.Equals(rightTable3));

                    Assert.IsTrue(result[4].Left.Equals(leftTable5));
                    Assert.IsTrue(result[4].Right.Equals(rightTable2));

                    Assert.IsTrue(result[5].Left.Equals(leftTable6));
                    Assert.IsTrue(result[5].Right.Equals(rightTable1));

                    Assert.IsTrue(result[6].Left.Equals(leftTable7));
                    Assert.IsTrue(result[6].Right == null);
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
        public void LeftJoinTablesUsingStringKey()
        {

            var rightTable1 = new RightTable { IntJoinKey = 1, SomeData = "Some data for check 1", SomeInt = 12311, StringJoinKey = "Key1" };
            var rightTable2 = new RightTable { IntJoinKey = 2, SomeData = "Some data for check 2", SomeInt = 26622, StringJoinKey = "Key2" };
            var rightTable3 = new RightTable { IntJoinKey = 3, SomeData = "Some data for check 3", SomeInt = 57474, StringJoinKey = "Key3" };


            var leftTable1 = new LeftTable { IntJoinKey = 1, SomeData = "Some Inner data 1 - 1", SomeInt = 45333, StringJoinKey = "Tra-la-la" };
            var leftTable2 = new LeftTable { IntJoinKey = 4, SomeData = "Some Inner data 2 - 1", SomeInt = 54744, StringJoinKey = "Key2" };
            var leftTable3 = new LeftTable { IntJoinKey = 0, SomeData = "Some Inner data 3 - 1", SomeInt = 34533, StringJoinKey = null };

            var leftTable4 = new LeftTable { IntJoinKey = 3, SomeData = "Some Inner data 1 - 2", SomeInt = 1110020, StringJoinKey = "Key1" };
            var leftTable5 = new LeftTable { IntJoinKey = 2, SomeData = "Some Inner data 2 - 2", SomeInt = 2011111, StringJoinKey = "Key3" };

            var leftTable6 = new LeftTable { IntJoinKey = 1, SomeData = "Some Inner data 1 - 3", SomeInt = 9383, StringJoinKey = "Bilbo Beggins" };
            var leftTable7 = new LeftTable { IntJoinKey = 5, SomeData = "Some Inner data 3 - 3", SomeInt = 345, StringJoinKey = "Key2" };

            foreach (var db in GetConnections())
            {
                try
                {
                     db.DeleteTable<RightTable>();
                     db.DeleteTable<LeftTable>();

                     db.CreateTable<RightTable>();
                     db.CreateTable<LeftTable>();

                     db.InsertItem(rightTable1);
                     db.InsertItem(rightTable2);
                     db.InsertItem(rightTable3);

                     db.InsertItem(leftTable1);
                     db.InsertItem(leftTable2);
                     db.InsertItem(leftTable3);
                     db.InsertItem(leftTable4);
                     db.InsertItem(leftTable5);
                     db.InsertItem(leftTable6);
                     db.InsertItem(leftTable7);


                    var leftJoin =  db.LeftJoin<LeftTable, RightTable, LeftJoinResult>(null, (left, right) => left.StringJoinKey == right.StringJoinKey,
                                (left, right) => new LeftJoinResult(left, right));

                    Assert.IsTrue(leftJoin != null);

                    var result = leftJoin.ToArray();

                    Assert.IsTrue(result.Length == 7);

                    Assert.IsTrue(result[0].Left.Equals(leftTable1));
                    Assert.IsTrue(result[0].Right == null);

                    Assert.IsTrue(result[1].Left.Equals(leftTable2));
                    Assert.IsTrue(result[1].Right.Equals(rightTable2));

                    Assert.IsTrue(result[2].Left.Equals(leftTable3));
                    Assert.IsTrue(result[2].Right == null);

                    Assert.IsTrue(result[3].Left.Equals(leftTable4));
                    Assert.IsTrue(result[3].Right.Equals(rightTable1));

                    Assert.IsTrue(result[4].Left.Equals(leftTable5));
                    Assert.IsTrue(result[4].Right.Equals(rightTable3));

                    Assert.IsTrue(result[5].Left.Equals(leftTable6));
                    Assert.IsTrue(result[5].Right == null);

                    Assert.IsTrue(result[6].Left.Equals(leftTable7));
                    Assert.IsTrue(result[6].Right.Equals(rightTable2));
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
