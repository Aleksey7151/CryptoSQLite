using System;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class CheckTableStructureTests : BaseTest
    {
        [Test]
        public void TableMustContainCryptoTableAttribute()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithoutCryptoTableAttribute>();
                });
                Assert.That(ex.Message, Contains.Substring($"Table {typeof(TableWithoutCryptoTableAttribute)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}"));
            }
        }

        [Test]
        public void TableNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithNullName>();
                });
                Assert.That(ex.Message, Contains.Substring("Table name can't be null or empty."));
            }
        }

        [Test]
        public void TableNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEmptyName>();
                });
                Assert.That(ex.Message, Contains.Substring("Table name can't be null or empty."));
            }
        }

        [Test]
        public void ColumnNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    db.CreateTable<TableWithNullColumnName>();
                });
                Assert.That(ex.Message, Contains.Substring("Column name can't be null."));
            }
        }

        [Test]
        public void ColumnNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    db.CreateTable<TableWithEmptyColumnName>();
                });
                Assert.That(ex.Message, Contains.Substring("Column name can't be empty."));
            }
        }

        [Test]
        public void TableCanNotContainSoltColumn_v1()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithSoltColumnNameFirst>();
                });
                Assert.That(ex.Message, Contains.Substring("This name is reserved for CryptoSQLite needs."));
            }
        }

        [Test]
        public void TableCanNotContainSoltColumn_v2()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithSoltColumnNameSecond>();
                });
                Assert.That(ex.Message, Contains.Substring("This name is reserved for CryptoSQLite needs."));
            }
        }

        [Test]
        public void TableCanNotBeWithoutPrimaryKeyColumn()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithoutPrimaryKeyColumn>();
                });
                Assert.That(ex.Message, Contains.Substring("Crypto table must contain at least one PrimaryKey column."));
            }
        }

        [Test]
        public void TableCanNotContainTwoPrimaryKeyColumns()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithTwoPrimaryKeyColumns>();
                });
                Assert.That(ex.Message, Contains.Substring("Crypto Table can't contain more that one PrimaryKey column."));
            }
        }

        [Test]
        public void PrimaryKeyColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedPrimaryKey>();
                });
                Assert.That(ex.Message, Contains.Substring("Column with PrimaryKey Attribute can't be Encrypted."));
            }
        }

        [Test]
        public void AutoIncrementalColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedAutoIncrementalKey>();
                });
                Assert.That(ex.Message, Contains.Substring("Column with AutoIncremental Attribute can't be Encrypted."));
            }
        }

        [Test]
        public void EncryptedColumnCanNotHaveDefaultValue()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedDefaultValue>();
                });
                Assert.That(ex.Message, Contains.Substring("Encrypted columns can't have default value, but they can be Not Null"));
            }
        }

        [Test]
        public void EncryptedColumnCanBeNotNull()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedNotNullValue>();
                    db.DeleteTable<TableWithEncryptedNotNullValue>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.Fail(cex.Message);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotContainTwoColumnsWithSameNames()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithTwoEqualColumnNames>();
                });
                Assert.That(ex.ProbableCause, Contains.Substring("Table can't contain two columns with same names."));
            }
        }
    }
}
