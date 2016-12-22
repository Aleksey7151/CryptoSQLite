using System;
using System.Linq;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class SelectTests : BaseTest
    {
        [Test]
        public void ErrorIfWrongPropertyNamePassed()
        {
            var item1 = new IntNumbers
            {
                IntMaxVal = 837498273,
                NullAble2 = 23423423,
                NullAble1 = 23425664,
                IntMinVal = -6245234
            };

            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);

                    db.Select<IntNumbers>(i => i.IntMaxVal == 1900, "IntMAXVal");

                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("' doesn't contain property with name: '", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ErrorIfNullOrEmptyPropertyNamePassed()
        {
            var item1 = new IntNumbers
            {
                IntMaxVal = 837498273,
                NullAble2 = 23423423,
                NullAble1 = 23425664,
                IntMinVal = -6245234
            };

            using (var db = GetAesConnection())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);

                    db.Select<IntNumbers>(i => i.IntMaxVal == 1900, "");
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Property Name for 'Select' can't be Null or Empty.",
                            StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
            Assert.Fail();
        }

        [Test]
        public void ReturnsValuesForAllColumnsIfPropertyNamesNotDetermined()
        {
            var item1 = new IntNumbers
            {
                IntMaxVal = 837498273,
                NullAble2 = 23423423,
                NullAble1 = 23425664,
                IntMinVal = -6245234
            };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item1);

                    var elements = db.Select<IntNumbers>(i => i.IntMaxVal == 837498273).ToArray();

                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(elements[0].Equals(item1));
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
        public void ReturnsOnlyValuesForColumnsWhictsPropertyNamesDetermined_V1()
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
                        db.Select<IntNumbers>(i => i.IntMaxVal == 837498273, "NullAble1", "IntMinVal").ToArray();

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
        public void ReturnsOnlyValuesForColumnsWhictsPropertyNamesDetermined_V2()
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
                        db.Select<AccountsData>(i => i.Age == 23, "Password", "Age", "Posts", "Salary").ToArray();

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
        public void SelectReferencedTableIfNavigationPropertyNamePassed()
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

                    var elements = db.Select<SimpleReference>(s => s.Id == 0, "InfoRefId").ToArray();
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
        public void NotSelectReferencedTableIfNavigationPropertyNameNotPassed()
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

                    var elements = db.Select<SimpleReference>(s => s.Id == 0, "SomeData").ToArray();
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
    }
}
