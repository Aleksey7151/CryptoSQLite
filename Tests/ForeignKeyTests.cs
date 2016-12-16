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
    internal class OrderV1
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Column("OrderIdentify")]
        public int OrderNumber { get; set; }

        [Encrypted, Column("Description")]
        public string OrderDescription { get; set; }

        public int PersonRefId { get; set; }        // This column will be FOREIGN KEY for Person. You can add also Column attribute to this property and NotNull Attribute

        [ForeignKey("PersonRefId")]    // Important, here we must pass real property name, but property can have Column attribute for specifing it name in table
        public Person PersonNavigation { get; set; }      // Navigation property to corresponding Person.
    }

    [CryptoTable("Orders")]
    internal class OrderV2
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Column("OrderIdentify")]
        public int OrderNumber { get; set; }

        [Encrypted]
        public string OrderDescription { get; set; }

        [ForeignKey("PersonNavigation")]      // Here we specify property name, that will navigate to corresponding Person. You can add also Column attribute to this property and NotNull Attribute
        public int PersonRefId { get; set; }    // This column will be FOREIGN KEY for Person.
        
        public Person PersonNavigation { get; set; }      // Navigation property to corresponding Person.
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
