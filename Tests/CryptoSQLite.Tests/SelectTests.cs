using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.Tests
{
    [TestFixture]
    public class SelectTests : BaseTest
    {
        [Test]
        public void ReturnsOnlyValuesForColumnsWhictsPropertyNamesDetermined_V1_UsingMemberAccess()
        {
            var item1 = new IntNumbers
            {
                IntMaxVal = 837498273,
                NullAble2 = 23423423,
                NullAble1 = 23425664,
                IntMinVal = -6245234
            };
            var item2 = new IntNumbers
            {
                IntMaxVal = 837498273,
                NullAble2 = 34534334,
                NullAble1 = 54678567,
                IntMinVal = -8387290
            };
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);

                    var elements =
                        db.Select<IntNumbers>(i => i.IntMaxVal == 837498273, i => i.NullAble1, i => i.IntMinVal).ToArray();

                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 2);

                    Assert.IsTrue(elements[0].NullAble2 == null &&
                                  elements[0].IntMaxVal == 0 &&
                                  elements[0].NullAble1 == item1.NullAble1 &&
                                  elements[0].IntMinVal == item1.IntMinVal);

                    Assert.IsTrue(elements[1].NullAble2 == null &&
                                  elements[1].IntMaxVal == 0 &&
                                  elements[1].NullAble1 == item2.NullAble1 &&
                                  elements[1].IntMinVal == item2.IntMinVal);
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
        public void ReturnsOnlyValuesForColumnsWhictsPropertyNamesDetermined_V2_UsingMemberAccess()
        {
            var item1 = new AccountsData
            {
                Name = "Account1",
                Password = "Password1",
                Age = 23,
                Posts = 42,
                Productivity = 82882.293847,
                Salary = 92384,
                SocialSecureId = 127348923
            };

            var item2 = new AccountsData
            {
                Name = "Account2",
                Password = "Password2",
                Age = 23,
                Posts = 45,
                Productivity = 12111.95905,
                Salary = 1211384,
                SocialSecureId = 593683904
            };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);

                    var elements =
                        db.Select<AccountsData>(i => i.Age == 23, a => a.Password, a => a.Age, a => a.Posts, a => a.Salary).ToArray();

                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 2);

                    Assert.IsTrue(elements[0].Name == null &&
                                  elements[0].Productivity == null &&
                                  elements[0].SocialSecureId == null &&
                                  elements[0].Password == item1.Password &&
                                  elements[0].Age == item1.Age &&
                                  elements[0].Posts == item1.Posts &&
                                  elements[0].Salary == item1.Salary);

                    Assert.IsTrue(elements[1].Name == null &&
                                  elements[1].Productivity == null &&
                                  elements[1].SocialSecureId == null &&
                                  elements[1].Password == item2.Password &&
                                  elements[1].Age == item2.Age &&
                                  elements[1].Posts == item2.Posts &&
                                  elements[1].Salary == item2.Salary);
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
        public void SelectReferencedTableIfNavigationPropertyNamePassedUsingMemberAccess()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();


                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var simpleRef1 = new SimpleReference
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 1 /*Row Doesn't exist in Infos!!!*/
                    };
                    db.InsertItem(simpleRef1);

                    var elements = db.Select<SimpleReference>(s => s.Id == 0, s => s.InfoRefId).ToArray();
                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].SomeData == null && elements[0].Simple.Equal(simple1));
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

        [Test]
        public void NotSelectReferencedTableIfNavigationPropertyNameNotPassedUsingMemberAccess()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();


                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var simpleRef1 = new SimpleReference
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 1 /*Row Doesn't exist in Infos!!!*/
                    };
                    db.InsertItem(simpleRef1);

                    var elements = db.Select<SimpleReference>(s => s.Id == 0, s => s.SomeData).ToArray();
                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].SomeData == "Some Data Descriptionen 1" && elements[0].Simple == null);
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

        /// <summary>
        /// When table contains several encrypted tables, and we want to get only one [Encrypted] column
        /// column must be correctly decrypted. 
        /// </summary>
        [Test]
        public void SelectOnlyOneEncryptedColumnWhenTableContainsSeveralEncryptedColumnsUsingMemberAccess()
        {
            var item = SeveralColumns.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SeveralColumns>();
                    db.CreateTable<SeveralColumns>();

                    db.InsertItem(item);

                    var elements = db.Select<SeveralColumns>(s => s.Id == 1, s => s.Str2).ToArray();
                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].Str2 == item.Str2 && elements[0].Str1 == null && elements[0].Str3 == null && elements[0].Str4 == null);
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

        /// <summary>
        /// When table contains several encrypted tables, and we want to get only one [Encrypted] column
        /// column must be correctly decrypted. 
        /// </summary>
        [Test]
        public void SelectTwoEncryptedColumnWhenTableContainsSeveralEncryptedColumnsUsingMemberAccess()
        {
            var item = SeveralColumns.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SeveralColumns>();
                    db.CreateTable<SeveralColumns>();

                    db.InsertItem(item);

                    var elements = db.Select<SeveralColumns>(s => s.Id == 1, s => s.Str2, s => s.Str4).ToArray();
                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].Str2 == item.Str2 && elements[0].Str1 == null && elements[0].Str3 == null && elements[0].Str4 == item.Str4);
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

        /// <summary>
        /// When table contains several encrypted tables, and we want to get only one [Encrypted] column
        /// column must be correctly decrypted. 
        /// </summary>
        [Test]
        public void SelectThreeEncryptedColumnWhenTableContainsSeveralEncryptedColumnsUsingMemberAccess()
        {
            var item = SeveralColumns.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SeveralColumns>();
                    db.CreateTable<SeveralColumns>();

                    db.InsertItem(item);

                    var elements = db.Select<SeveralColumns>(s => s.Id == 1, s => s.Str2, s => s.Str4, s => s.Str3).ToArray();
                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].Str2 == item.Str2 && elements[0].Str1 == null && elements[0].Str3 == item.Str3 && elements[0].Str4 == item.Str4);
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

        [Test]
        public void SelectTop_Items_From_Table()
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

                    var result = db.SelectTop<IntNumbers>(2).ToArray();

                    Assert.IsTrue(result.Length == 2);

                    Assert.IsTrue(result[0].Equals(item1));
                    Assert.IsTrue(result[1].Equals(item2));
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

    [CryptoTable("SeveralColumns")]
    internal class SeveralColumns
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string Str1 { get; set; }

        [Encrypted]
        public string Str2 { get; set; }

        [Encrypted]
        public string Str3 { get; set; }

        [Encrypted]
        public string Str4 { get; set; }

        public bool Equals(SeveralColumns sc)
        {
            return Str1 == sc.Str1 && Str2 == sc.Str2 && Str3 == sc.Str3 && Str4 == sc.Str4;
        }

        public static SeveralColumns GetDefault()
        {
            return new SeveralColumns
            {
                Str1 = "Several Columns String 1",
                Str2 = "Several Columns String 22",
                Str3 = "Several Columns String 333",
                Str4 = "Several Columns String 4444",
            };
        }
    }
}
