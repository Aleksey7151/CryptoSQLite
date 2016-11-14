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
                try
                {
                    db.CreateTable<TableWithoutCryptoTableAttribute>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf($"Table {typeof(TableWithoutCryptoTableAttribute)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithNullName>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Table name can't be null or empty.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEmptyName>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Table name can't be null or empty.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
            }
        }

        [Test]
        public void ColumnNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithNullColumnName>();
                }
                catch (ArgumentException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column name can't be null.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void ColumnNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEmptyColumnName>();
                }
                catch (ArgumentException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column name can't be empty.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotContainSoltColumn()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithSoltColumnNameFirst>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("This name is reserved for CryptoSQLite needs.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }

                try
                {
                    db.CreateTable<TableWithSoltColumnNameSecond>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("This name is reserved for CryptoSQLite needs.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotBeWithoutPrimaryKeyColumn()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithoutPrimaryKeyColumn>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Crypto table must contain at least one PrimaryKey column.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotContainTwoPrimaryKeyColumns()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithTwoPrimaryKeyColumns>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Crypto Table can't contain more that one PrimaryKey column.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void PrimaryKeyColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedPrimaryKey>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column with PrimaryKey Attribute can't be Encrypted.", StringComparison.Ordinal) >= 0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void AutoIncrementalColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedAutoIncrementalKey>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Column with AutoIncremental Attribute can't be Encrypted.", StringComparison.Ordinal) >= 0);
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
                try
                {
                    db.CreateTable<TableWithTwoEqualColumnNames>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.ProbableCause.IndexOf("Table can't contain two columns with same names.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotContainEncryptedByteColumn()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedByteColumn>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Columns that have Boolean or Byte type can't be Encrypted.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableCanNotContainEncryptedBoolColumn()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedBoolColumn>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Columns that have Boolean or Byte type can't be Encrypted.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TableWithIncompatibleColumnType()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithInCompatibleColumnType>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("contains incompatible type of property.", StringComparison.Ordinal) >=
                        0);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
        }
    }
}
