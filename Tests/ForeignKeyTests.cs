using System;
using System.Linq;
using CryptoSQLite;
using NUnit.Framework;

namespace Tests
{
    //Incorrect tables:
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


        [ForeignKey("Person")]
        public string ForeignKey { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyCanNotBeEncrypted")]
    internal class TableForeignKeyCanNotBeEncrypted
    {
        [PrimaryKey]
        public int Id { get; set; }


        [ForeignKey("Person"), Encrypted]
        public string ForeignKey { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyAndPrimaryKey")]
    internal class TableForeignKeyAndPrimaryKey
    {
        [PrimaryKey, ForeignKey("Person")]
        public int Id { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyAndAutoIncrement")]
    internal class TableForeignKeyAndAutoIncrement
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("Person"), AutoIncremental]
        public int ForeignKey { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyWithDefaultValue")]
    internal class TableForeignKeyWithDefaultValue
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("Person"), NotNull(7)]
        public int ForeignKey { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyWithIncorrectName")]
    internal class TableForeignKeyWithIncorrectName
    {
        [PrimaryKey]
        public int Id { get; set; }
        
        [ForeignKey("PArson")]
        public int ForeignKey { get; set; }

        public Person Person { get; set; }
    }

    [CryptoTable("TableForeignKeyReferencedTableWithoutCryptoTableAttr")]
    internal class TableForeignKeyReferencedTableWithoutCryptoTableAttr
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("Person")]
        public int ForeignKey { get; set; }

        public TableWithoutCryptoTableAttribute Person { get; set; }
    }

    internal class TableWithoutCryptoTableAttribute
    {
        public int Id { get; set; }
    }

    [CryptoTable("TableForeignKeyReferencedTableWithoutPrimaryKey")]
    internal class TableForeignKeyReferencedTableWithoutPrimaryKey
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey("Person")]
        public int ForeignKey { get; set; }

        public TableWithoutPrimaryKey Person { get; set; }
    }

    [CryptoTable("TableWithoutPrimaryKey")]
    internal class TableWithoutPrimaryKey
    {
        public int Id { get; set; }

        public string SomeData { get; set; }

        public int SomeInt { get; set; }
    }

    // Correct tables:
    // Dependency diagram:  Order ---> Person ---> Request ---> Data
    [CryptoTable("Datas")]
    internal class Data
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string DataDescription { get; set; }

        public int Count { get; set; }

        public bool Equal(Data d)
        {
            return DataDescription == d.DataDescription && Count == d.Count;
        }
    }

    [CryptoTable("Requests")]
    internal class Request
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string RequestDescription { get; set; }

        [Encrypted]
        public ulong Value { get; set; }

        [ForeignKey("DataNavigation"), Column("DAT_ID")]
        public int DataRefId { get; set; }

        public Data DataNavigation { get; set; }

        public bool Equal(Request r)
        {
            return RequestDescription == r.RequestDescription && Value == r.Value && DataRefId == r.DataRefId;
        }
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

        [ForeignKey("RequestNav"), Column("REQ_ID")]
        public int RequestRefId { get; set; }

        public Request RequestNav { get; set; }

        public bool Equal(Person p)
        {
            return Name == p.Name && Address == p.Address && Rating == p.Rating && RequestRefId == p.RequestRefId;
        }
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

        [ForeignKey("PersonNavigation"), Column("PER_ID")]
        public int PersonRefId { get; set; }        // This column will be FOREIGN KEY for Person. You can add also Column attribute to this property and NotNull Attribute

        public Person PersonNavigation { get; set; }      // Navigation property to corresponding Person.

        public bool Equal(Order o)
        {
            return OrderNumber == o.OrderNumber && OrderDescription == o.OrderDescription &&
                   PersonRefId == o.PersonRefId;
        }
    }

    // Multiple referenced tables

    [CryptoTable("TableWithTwoReferencedTables")]
    internal class TableWithTwoReferencedTables
    {
        
    }
    
    [TestFixture]
    public class ForeignKeyTests : BaseTest
    {
        [Test]
        public void ForeignKeyNameCanNotBeNull()
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
        public void ForeignKeyNameCanNotBeEmpty()
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
        public void ForeignKeyCanBeAppliedOnlyToIntTypes()
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
                    Assert.IsTrue(cex.Message.IndexOf("ForeignKey attribute can be applied only to 'Int32', 'UInt32', 'Int16', 'UInt16' properties, or to property, Type of which has CryptoTabl", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyCanNotBeEncrypted>();
                    db.CreateTable<TableForeignKeyCanNotBeEncrypted>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Property can't have ForeignKey and Encrypted attributes simultaneously.", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyCanNotBePrimaryKey()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyAndPrimaryKey>();
                    db.CreateTable<TableForeignKeyAndPrimaryKey>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Property can't have ForeignKey and PrimaryKey attributes simultaneously.", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyCanNotBeAutoIncrement()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyAndAutoIncrement>();
                    db.CreateTable<TableForeignKeyAndAutoIncrement>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Property can't have ForeignKey and AutoIncrement attributes simultaneously.", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyCanNotHaveDefaultValue()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyWithDefaultValue>();
                    db.CreateTable<TableForeignKeyWithDefaultValue>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Property with ForeignKey attribute can't have Default Value.", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyNameDoesNotPointToNavigationProperty()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyWithIncorrectName>();
                    db.CreateTable<TableForeignKeyWithIncorrectName>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("Can't find Navigation Property for '", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyReferencedTableWithoutCryptoTableAttribute()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyReferencedTableWithoutCryptoTableAttr>();
                    db.CreateTable<TableForeignKeyReferencedTableWithoutCryptoTableAttr>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("doesn't have Custom Attribute:", StringComparison.Ordinal) >= 0);
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
        public void ForeignKeyReferencedTableWithoutPrimaryKeyAttribute()
        {
            using (var db = GetGostConnection())
            {
                try
                {
                    db.DeleteTable<TableForeignKeyReferencedTableWithoutPrimaryKey>();
                    db.CreateTable<TableForeignKeyReferencedTableWithoutPrimaryKey>();
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.Message.IndexOf("doesn't contain property with PrimaryKey Attribute.", StringComparison.Ordinal) >= 0);
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
        public void SequenceOfReferencedTables()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    // Dependency diagram:  Order ---> Person ---> Request ---> Data
                    //                        1   --->   2    --->    2    --->   1
                    //                        2   --->   3    --->    2    --->   1
                    //                        3   --->   1    --->    1    --->   2
                    db.DeleteTable<Order>();
                    db.DeleteTable<Person>();
                    db.DeleteTable<Request>();
                    db.DeleteTable<Data>();

                    db.CreateTable<Data>();
                    db.CreateTable<Request>();
                    db.CreateTable<Person>();
                    db.CreateTable<Order>();

                    var data1 = new Data {Count = 73447, DataDescription = "Some Data Destriptionen 1"};
                    db.InsertItem(data1);
                    var data2 = new Data {Count = 298729, DataDescription = "Some Data Descriptionen 2"};
                    db.InsertItem(data2);

                    var request1 = new Request {RequestDescription = "Some Request Descriptionen 1", Value = 9128748213748273, DataRefId = 2};
                    db.InsertItem(request1);
                    var request2 = new Request { RequestDescription = "Some Request Descriptionen 2", Value = 2034958127495, DataRefId = 1 };
                    db.InsertItem(request2);

                    var person1 = new Person {Address = "Some Addressen 1", Name = "Some Name 1", Rating = 13223, RequestRefId = 1};
                    db.InsertItem(person1);
                    var person2 = new Person { Address = "Some Addressen 2", Name = "Some Name 2", Rating = 3029458, RequestRefId = 2 };
                    db.InsertItem(person2);
                    var person3 = new Person { Address = "Some Addressen 3", Name = "Some Name 3", Rating = 234566, RequestRefId = 2 };
                    db.InsertItem(person3);

                    var order1 = new Order {OrderDescription = "Some Order Descriptionen 1", OrderNumber = 766446, PersonRefId = 2};
                    db.InsertItem(order1);
                    var order2 = new Order { OrderDescription = "Some Order Descriptionen 2", OrderNumber = 2363473, PersonRefId = 3 };
                    db.InsertItem(order2);
                    var order3 = new Order { OrderDescription = "Some Order Descriptionen 3", OrderNumber = 66786786, PersonRefId = 1 };
                    db.InsertItem(order3);

                    var orders = db.Find<Order>(o => o.Id == 1).ToArray();

                    Assert.NotNull(orders);
                    Assert.IsTrue(orders.Length == 1);
                    Assert.IsTrue(orders[0].Equal(order1));
                    Assert.IsTrue(orders[0].PersonNavigation.Equal(person2));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.Equal(request2));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data1));

                    orders = null;
                    orders = db.Find<Order>(o => o.Id == 2).ToArray();
                    Assert.NotNull(orders);
                    Assert.IsTrue(orders.Length == 1);
                    Assert.IsTrue(orders[0].Equal(order2));
                    Assert.IsTrue(orders[0].PersonNavigation.Equal(person3));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.Equal(request2));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data1));

                    orders = null;
                    orders = db.Find<Order>(o => o.Id == 3).ToArray();
                    Assert.NotNull(orders);
                    Assert.IsTrue(orders.Length == 1);
                    Assert.IsTrue(orders[0].Equal(order3));
                    Assert.IsTrue(orders[0].PersonNavigation.Equal(person1));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.Equal(request1));
                    Assert.IsTrue(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data2));
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
