using System;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [CryptoTable("TableIncorrectAppliedForeignKeyAttribute")]
    internal class TableIncorrectAppliedForeignKeyAttribute
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("Tra")]
        public string ForeignKey { get; set; }
    }

    [CryptoTable("Persons")]
    internal class Person
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted, NotNull]
        public string Name { get; set; }

        [Encrypted, NotNull]
        public string Address { get; set; }

        public int Rating { get; set; }
    }

    [CryptoTable("Orders")]
    internal class Order
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Column("OrderIdentify")]
        public int OrderNumber { get; set; }

        [Encrypted]
        public string OrderDescription { get; set; }

        public int PersonRefId { get; set; }    // This column will be FOREIGN KEY for Person.

        [ForeignKey("PersonRefId")]
        public Person Person { get; set; }      // Navigation property to corresponding Person.
    }

    [TestFixture]
    public class ForeignKeyTests : BaseTest
    {
        [Test]
        public void ForeignKeyAttributeNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.FindByValue<AccountsData>(null, 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void ForeignKeyAttributeNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.FindByValue<AccountsData>(null, 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }

        [Test]
        public void ForeignKeyAttributeCanBeAppliedOnlyToIntTypeOrClassThatHasCryptoTableAttribute()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.FindByValue<AccountsData>(null, 123);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Column name can't be null or empty.", StringComparison.Ordinal) >= 0);
                    return;
                }
                catch (Exception ex)
                {
                    Assert.Fail(ex.Message);
                }
                finally
                {
                    db.Dispose();
                }
                Assert.Fail();
            }
        }


    }
}
