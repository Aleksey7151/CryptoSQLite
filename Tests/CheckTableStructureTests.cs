﻿using System;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }
            Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void TableCanNotContainSoltColumn_v1()
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void TableCanNotContainSoltColumn_v2()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithSoltColumnNameSecond>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("This name is reserved for CryptoSQLite needs.", StringComparison.Ordinal) >=
                        0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void EncryptedColumnCanNotHaveDefaultValue()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.CreateTable<TableWithEncryptedDefaultValue>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Encrypted columns can't have default value, but they can be Not Null", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
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
                try
                {
                    db.CreateTable<TableWithTwoEqualColumnNames>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.ProbableCause.IndexOf("Table can't contain two columns with same names.", StringComparison.Ordinal) >=
                        0);
                    return;
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
                Assert.Fail();
            }
        }
    }
}
