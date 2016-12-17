using System;
using System.Linq;
using CryptoSQLite;
using NUnit.Framework;

namespace Tests
{
    [CryptoTable("TableForeignKeyNullName")]
    internal class TableForeignKeyNullName
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(null)]
        public int ForeignKey { get; set; }
    }

    [CryptoTable("TableForeignKeyEmptyName")]
    internal class TableForeignKeyEmptyName
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("")]
        public int ForeignKey { get; set; }
    }

    [CryptoTable("TableForeignKeyNotIntType")]
    internal class TableForeignKeyNotIntType
    {
        [PrimaryKey]
        public int Id { get; set; }


        [ForeignKey("ForeignKey")]
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

        [Encrypted, Column("Description")]
        public string OrderDescription { get; set; }

        [ForeignKey("PersonNavigation")]
        public int PersonRefId { get; set; }        // This column will be FOREIGN KEY for Person. You can add also Column attribute to this property and NotNull Attribute

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
                    db.DeleteTable<TableForeignKeyNullName>();
                    db.CreateTable<TableForeignKeyNullName>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Foreign Key Attribute in property '", StringComparison.Ordinal) >= 0);
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
                    db.DeleteTable<TableForeignKeyEmptyName>();
                    db.CreateTable<TableForeignKeyEmptyName>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Foreign Key Attribute in property '", StringComparison.Ordinal) >= 0);
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
                    db.DeleteTable<TableForeignKeyNotIntType>();
                    db.CreateTable<TableForeignKeyNotIntType>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("ForeignKey attribute can be applied only to 'Int32', 'UInt32' properties, or to property, Type of which has CryptoTable attribute.", StringComparison.Ordinal) >= 0);
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
        public void ReferencedTableLoadedAutomatically()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<Person>();
                    db.CreateTable<Person>();
                    db.DeleteTable<Order>();
                    db.CreateTable<Order>();

                    var person1 = new Person {Address = "Tuhachevskogo", Name = "Safonova Anna", Rating = 132};
                    db.InsertItem(person1);

                    var order1 = new Order {OrderDescription = "Take a rest", OrderNumber = 766446, PersonRefId = 1};
                    db.InsertItem(order1);

                    var elements = db.Find<Order>(o => o.Id == 1).ToArray();

                    Assert.NotNull(elements);
                    Assert.IsTrue(elements.Length == 1);


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
