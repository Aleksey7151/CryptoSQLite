using System;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{

    #region Join Tables

    [CryptoTable("Products")]
    public class Product
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public uint Serial { get; set; }

        [Encrypted]
        public string Description { get; set; }

        public int? CustomerId { get; set; }     // determines Customer, if Product doesn't already have Customer, so it Equals to NULL.

        // Product always has a Manufacturer, so ManufacturerIs has ForeignKey Constraint.
        [ForeignKey("Manufacturer", false)]     // this table won't be automatically obtained from database, when we get this table, because we set 'autoResolveReference' parameter to false.
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }

        // Product always has a Warehouse, so WarehouseId has ForeignKey Constraint, references to Warehouses table
        [ForeignKey("Warehouse", false)]    // this table won't be automatically obtained from database, when we get this table, because we set 'autoResolveReference' parameter to false.
        public int WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }

    [CryptoTable("Customers")]
    public class Customer
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string ContactName { get; set; }

        [Encrypted]
        public string Address { get; set; }

        public int Rating { get; set; }
    }

    [CryptoTable("Manufacturers")]
    public class Manufacturer
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Encrypted]
        public int Salary { get; set; }
    }

    [CryptoTable("Warehouses")]
    public class Warehouse
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string Address { get; set; }

        [Encrypted]
        public string PhoneNumber { get; set; }

        public string Manager { get; set; }
    }

    #endregion
    [TestFixture]
    public class JoinTests : BaseTest
    {
        [Test]
        public void JoinTwoTables()
        {
            var warehouse1 = new Warehouse{Address = "Tuchachevskogo 31-30",Manager = "Safonova Anna",PhoneNumber = "7000665"};
            var warehouse2 = new Warehouse{Address = "Ohotskaya",Manager = "Safonov Alexei",PhoneNumber = "7979665"};

            var manufacturer1 = new Manufacturer{FirstName = "Alexei",LastName = "Safonov",Salary = 1800};
            var manufacturer2 = new Manufacturer{FirstName = "Hanna",LastName = "Safonova",Salary = 250};
            var manufacturer3 = new Manufacturer{FirstName = "Tatiana",LastName = "Kulikova",Salary = 550};

            var customer1 = new Customer {Address = "New York", ContactName = "Sidorov Evgeniy", Rating = 100};
            var customer2 = new Customer { Address = "Moscow", ContactName = "Sidorova Evgenia", Rating = 99 };
            var customer3 = new Customer { Address = "Minsk", ContactName = "Luka", Rating = 2 };

            var product1 = new Product{Serial = 12355444,Description = "Toy Pistol",CustomerId = 2,ManufacturerId = 1,WarehouseId = 1};
            var product2 = new Product { Serial = 483783, Description = "Book of jungle", CustomerId = null, ManufacturerId = 2, WarehouseId = 1 };
            var product3 = new Product { Serial = 2456434, Description = "Bycycle", CustomerId = 1, ManufacturerId = 3, WarehouseId = 2 };
            var product4 = new Product { Serial = 345333, Description = "Train", CustomerId = null, ManufacturerId = 1, WarehouseId = 1 };
            var product5 = new Product { Serial = 6786678, Description = "MotoByke", CustomerId = 2, ManufacturerId = 3, WarehouseId = 2 };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<Product>();
                    db.DeleteTable<Warehouse>();
                    db.DeleteTable<Manufacturer>();
                    db.DeleteTable<Customer>();


                    db.CreateTable<Warehouse>();
                    db.CreateTable<Manufacturer>();
                    db.CreateTable<Customer>();
                    db.CreateTable<Product>();

                    db.InsertItem(warehouse1);
                    db.InsertItem(warehouse2);

                    db.InsertItem(manufacturer1);
                    db.InsertItem(manufacturer2);
                    db.InsertItem(manufacturer3);

                    db.InsertItem(customer1);
                    db.InsertItem(customer2);
                    db.InsertItem(customer3);

                    db.InsertItem(product1);
                    db.InsertItem(product2);
                    db.InsertItem(product3);
                    db.InsertItem(product4);
                    db.InsertItem(product5);

                    var max = db.Join<Product, Customer>(i => i.Id == 1, (p,c) => p.CustomerId == c.Id, (t1, t2) => new object[] {t1, t2});

                    
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
}
