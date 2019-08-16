using System;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    
    public class CheckTableStructureTests : BaseTest
    {
        [Fact]
        public void TableMustContainCryptoTableAttribute()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithoutCryptoTableAttribute>();
                });
                Assert.Contains($"Table {typeof(TableWithoutCryptoTableAttribute)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}", ex.Message);
            }
        }

        [Fact]
        public void TableNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithNullName>();
                });
                Assert.Contains("Table name can't be null or empty.", ex.Message);
            }
        }

        [Fact]
        public void TableNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEmptyName>();
                });
                Assert.Contains("Table name can't be null or empty.", ex.Message);
            }
        }

        [Fact]
        public void ColumnNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    db.CreateTable<TableWithNullColumnName>();
                });
                Assert.Contains("Column name can't be null.", ex.Message);
            }
        }

        [Fact]
        public void ColumnNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<ArgumentException>(() =>
                {
                    db.CreateTable<TableWithEmptyColumnName>();
                });
                Assert.Contains("Column name can't be empty.", ex.Message);
            }
        }

        [Fact]
        public void TableCanNotContainSoltColumn_v1()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithSoltColumnNameFirst>();
                });
                Assert.Contains("This name is reserved for CryptoSQLite needs.", ex.Message);
            }
        }

        [Fact]
        public void TableCanNotContainSoltColumn_v2()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithSoltColumnNameSecond>();
                });
                Assert.Contains("This name is reserved for CryptoSQLite needs.", ex.Message);
            }
        }

        [Fact]
        public void TableCanNotBeWithoutPrimaryKeyColumn()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithoutPrimaryKeyColumn>();
                });
                Assert.Contains("Crypto table must contain at least one PrimaryKey column.", ex.Message);
            }
        }

        [Fact]
        public void TableCanNotContainTwoPrimaryKeyColumns()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithTwoPrimaryKeyColumns>();
                });
                Assert.Contains("Crypto Table can't contain more that one PrimaryKey column.", ex.Message);
            }
        }

        [Fact]
        public void PrimaryKeyColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedPrimaryKey>();
                });
                Assert.Contains("Column with PrimaryKey Attribute can't be Encrypted.", ex.Message);
            }
        }

        [Fact]
        public void AutoIncrementalColumnCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedAutoIncrementalKey>();
                });
                Assert.Contains("Column with AutoIncremental Attribute can't be Encrypted.", ex.Message);
            }
        }

        [Fact]
        public void EncryptedColumnCanNotHaveDefaultValue()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithEncryptedDefaultValue>();
                });
                Assert.Contains("Encrypted columns can't have default value, but they can be Not Null", ex.Message);
            }
        }

        [Fact]
        public void EncryptedColumnCanBeNotNull()
        {
            using (var db = GetGostConnection())
            {
                db.CreateTable<TableWithEncryptedNotNullValue>();
                db.DeleteTable<TableWithEncryptedNotNullValue>();
            }
        }

        [Fact]
        public void TableCanNotContainTwoColumnsWithSameNames()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.CreateTable<TableWithTwoEqualColumnNames>();
                });
                Assert.Contains("Table can't contain two columns with same names.", ex.ProbableCause);
            }
        }
    }
}
