﻿using System.Linq;
using Xunit;

namespace CryptoSQLite.Tests
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

        public bool Equals(Product p)
        {
            return Serial == p.Serial && Description == p.Description && CustomerId == p.CustomerId &&
                   ManufacturerId == p.ManufacturerId && WarehouseId == p.WarehouseId;
        }
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

        public bool Equals(Customer c)
        {
            return ContactName == c.ContactName && Address == c.Address && Rating == c.Rating;
        }
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

        public bool Equals(Manufacturer m)
        {
            return FirstName == m.FirstName && LastName == m.LastName && Salary == m.Salary;
        }
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

        public bool Equals(Warehouse w)
        {
            return Address == w.Address && PhoneNumber == w.PhoneNumber && Manager == w.Manager;
        }
    }

    public class JoinedTables
    {
        public Product Product { get; set; }

        public Customer Customer { get; set; }

        public Manufacturer Manufacturer { get; set; }

        public Warehouse Warehouse { get; set; }
    }

    #endregion

    [Collection("Sequential")]
    public class JoinTests : BaseTest
    {
        [Fact]
        public void JoinTwoTables()
        {

            var warehouse1 = new Warehouse { Address = "Tuchachevskogo 31-30", Manager = "Safonova Anna", PhoneNumber = "7000665" };
            var warehouse2 = new Warehouse { Address = "Ohotskaya", Manager = "Safonov Alexei", PhoneNumber = "7979665" };

            var manufacturer1 = new Manufacturer { FirstName = "Alexei", LastName = "Safonov", Salary = 1800 };
            var manufacturer2 = new Manufacturer { FirstName = "Hanna", LastName = "Safonova", Salary = 250 };
            var manufacturer3 = new Manufacturer { FirstName = "Tatiana", LastName = "Kulikova", Salary = 550 };

            var customer1 = new Customer { Address = "New York", ContactName = "Sidorov Evgeniy", Rating = 100 };
            var customer2 = new Customer { Address = "Moscow", ContactName = "Sidorova Evgenia", Rating = 99 };
            var customer3 = new Customer { Address = "Minsk", ContactName = "Luka", Rating = 2 };

            var product1 = new Product { Serial = 11111, Description = "Toy Pistol", CustomerId = 2, ManufacturerId = 1, WarehouseId = 1 };
            var product2 = new Product { Serial = 22222, Description = "Book of jungle", CustomerId = 4, ManufacturerId = 2, WarehouseId = 1 };
            var product3 = new Product { Serial = 33333, Description = "Bycycle", CustomerId = 1, ManufacturerId = 3, WarehouseId = 2 };
            var product4 = new Product { Serial = 44444, Description = "Train", CustomerId = null, ManufacturerId = 1, WarehouseId = 1 };
            var product5 = new Product { Serial = 55555, Description = "MotoByke", CustomerId = 3, ManufacturerId = 3, WarehouseId = 2 };

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

                    var twoJoinedTablesResult = db.Join<Product, Customer, JoinedTables>(null, (p, c) => p.CustomerId == c.Id, (t1, t2) => new JoinedTables { Product = t1, Customer = t2 });

                    var twoJoinedTables = twoJoinedTablesResult.ToArray();

                    Assert.NotNull(twoJoinedTables);
                    Assert.True(twoJoinedTables.Length == 3);

                    var joined = twoJoinedTables[0];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer == null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product1) && joined.Customer.Equals(customer2));

                    joined = twoJoinedTables[1];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer == null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product3) && joined.Customer.Equals(customer1));

                    joined = twoJoinedTables[2];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer == null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product5) && joined.Customer.Equals(customer3));

                    var threeJoinedTablesResult = db.Join<Product, Customer, Manufacturer, JoinedTables>(null,
                        (product, customer) => product.CustomerId == customer.Id,
                        (product, manufacturer) => product.ManufacturerId == manufacturer.Id,
                        (product, customer, manufacturer) =>
                                new JoinedTables { Customer = customer, Product = product, Manufacturer = manufacturer });

                    var threeJoinedTables = threeJoinedTablesResult.ToArray();

                    Assert.NotNull(threeJoinedTables);
                    Assert.Equal(3, threeJoinedTables.Length);

                    joined = threeJoinedTables[0];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product1) && joined.Customer.Equals(customer2) && joined.Manufacturer.Equals(manufacturer1));

                    joined = threeJoinedTables[1];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product3) && joined.Customer.Equals(customer1) && joined.Manufacturer.Equals(manufacturer3));

                    joined = threeJoinedTables[2];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse == null);
                    Assert.True(joined.Product.Equals(product5) && joined.Customer.Equals(customer3) && joined.Manufacturer.Equals(manufacturer3));

                    var fourJoinedTablesResult = db.Join<Product, Customer, Manufacturer, Warehouse, JoinedTables>(null,
                        (product, customer) => product.CustomerId == customer.Id,
                        (product, manufacturer) => product.ManufacturerId == manufacturer.Id,
                        (product, warehouse) => product.WarehouseId == warehouse.Id,
                        (product, customer, manufacturer, warehouse) =>
                                new JoinedTables { Customer = customer, Product = product, Manufacturer = manufacturer, Warehouse = warehouse });

                    var fourJoinedTables = fourJoinedTablesResult.ToArray();

                    Assert.NotNull(fourJoinedTables);
                    Assert.Equal(3, fourJoinedTables.Length);

                    joined = fourJoinedTables[0];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse != null);
                    Assert.True(joined.Product.Equals(product1) && joined.Customer.Equals(customer2) && joined.Manufacturer.Equals(manufacturer1) && joined.Warehouse.Equals(warehouse1));

                    joined = fourJoinedTables[1];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse != null);
                    Assert.True(joined.Product.Equals(product3) && joined.Customer.Equals(customer1) && joined.Manufacturer.Equals(manufacturer3) && joined.Warehouse.Equals(warehouse2));

                    joined = fourJoinedTables[2];
                    Assert.True(joined.Product != null && joined.Customer != null && joined.Manufacturer != null && joined.Warehouse != null);
                    Assert.True(joined.Product.Equals(product5) && joined.Customer.Equals(customer3) && joined.Manufacturer.Equals(manufacturer3) && joined.Warehouse.Equals(warehouse2));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
