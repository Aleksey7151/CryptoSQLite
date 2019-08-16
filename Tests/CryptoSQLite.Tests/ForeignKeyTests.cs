using System;
using System.Linq;
using Xunit;

namespace CryptoSQLite.Tests
{
    #region Incorrect tables

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
    #endregion //Incorrect Tables



    #region Simpliest Dependency

    // Simpliest Dependency
    [CryptoTable("SimpleReference")]
    internal class SimpleReference
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Encrypted]
        public string SomeData { get; set; }

        [ForeignKey("Simple"), Column("SIMPLE_ID")]
        public int InfoRefId { get; set; }

        public Simple Simple { get; set; }
    }

    [CryptoTable("SimpleReferenceWithoutAutoResolve")]
    internal class SimpleReferenceWithoutAutoResolve
    {
        [PrimaryKey]
        public int Id { get; set; }

        [Encrypted]
        public string SomeData { get; set; }

        [ForeignKey("Simple", false)]
        public int InfoRefId { get; set; }

        public Simple Simple { get; set; }
    }

    [CryptoTable("Simple")]
    internal class Simple
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string SimpleString { get; set; }

        public int SimpleValue { get; set; }

        public bool Equal(Simple s)
        {
            return SimpleString == s.SimpleString && SimpleValue == s.SimpleValue;
        }
    }

    #endregion



    #region Dependency sequency

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

    #endregion //Dependency sequency



    #region Multiple Referenced Tables

    // Multiple referenced tables
    // Dependency diagram:  
    //                                     / ---> Account ---> Job                     
    //
    //                 ManyRefTables ---> |  ---> Info                          
    //
    //                                     \ ---> Client  ---> BankCount                
    //
    [CryptoTable("Accounts")]
    internal class Account
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string AccountData { get; set; }

        [ForeignKey("Job"), Column("JOB_ID")]
        public int JobRefId { get; set; }

        public Job Job { get; set; }

        public bool Equal(Account ac)
        {
            return AccountData == ac.AccountData && JobRefId == ac.JobRefId;
        }
    }

    [CryptoTable("Jobs")]
    internal class Job
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string JobDescription { get; set; }

        [Encrypted]
        public double Price { get; set; }

        public bool Equal(Job j)
        {
            return JobDescription == j.JobDescription && Math.Abs(Price - j.Price) < 0.000001;
        }
    }

    [CryptoTable("Infos")]
    internal class Info
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public string SomeInfo { get; set; }

        [Encrypted]
        public double SomeValue { get; set; }

        public bool Equal(Info i)
        {
            return SomeInfo == i.SomeInfo && Math.Abs(SomeValue - i.SomeValue) < 0.000001;
        }
    }

    [CryptoTable("Clients")]
    internal class Client
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted, NotNull]
        public string PasswordHash { get; set; }

        [Column("DATE")]
        public DateTime CreationTime { get; set; }

        [ForeignKey("Count"), Column("COUNT_ID")]
        public int BankCountRefId { get; set; }

        public BankCount Count { get; set; }

        public bool Equal(Client c)
        {
            return PasswordHash == c.PasswordHash && CreationTime == c.CreationTime &&
                   BankCountRefId == c.BankCountRefId;
        }
    }

    [CryptoTable("BankCounts")]
    internal class BankCount
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted]
        public ulong CountId { get; set; }

        [Encrypted]
        public double Deposit { get; set; }

        public bool Equal(BankCount b)
        {
            return CountId == b.CountId && Math.Abs(Deposit - b.Deposit) < 0.000001;
        }
    }

    [CryptoTable("ManyRefTables")]
    internal class ManyRefTables
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted, NotNull]
        public string SomeStringData { get; set; }

        [ForeignKey("Account"), Column("ACC_ID")]
        public int RequestRefId { get; set; }

        public Account Account { get; set; }

        [ForeignKey("Info"), Column("INFO_ID")]
        public int InfoRefId { get; set; }

        public Info Info { get; set; }

        [ForeignKey("Client"), Column("CLIENT_ID")]
        public int ClientRefId { get; set; }

        public Client Client { get; set; }

        public bool Equal(ManyRefTables m)
        {
            return SomeStringData == m.SomeStringData && RequestRefId == m.RequestRefId && InfoRefId == m.InfoRefId &&
                   ClientRefId == m.ClientRefId;
        }
    }

    #endregion //Multiple Referenced Tables

    [Collection("Sequential")]
    public class ForeignKeyTests : BaseTest
    {
        [Fact]
        public void ForeignKeyNameCanNotBeNull()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyNullName>();
                    db.CreateTable<TableForeignKeyNullName>();
                });
                Assert.Contains("Foreign Key Attribute in property '", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyNameCanNotBeEmpty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyEmptyName>();
                    db.CreateTable<TableForeignKeyEmptyName>();
                });
                Assert.Contains("Foreign Key Attribute in property '", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyCanBeAppliedOnlyToIntTypes()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyNotIntType>();
                    db.CreateTable<TableForeignKeyNotIntType>();
                });
                Assert.Contains("ForeignKey attribute can be applied only to 'Int32', 'UInt32', 'Int16', 'UInt16' properties, or to property, Type of which has CryptoTable", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyCanNotBeEncrypted()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyCanNotBeEncrypted>();
                    db.CreateTable<TableForeignKeyCanNotBeEncrypted>();
                });
                Assert.Contains("Property can't have ForeignKey and Encrypted attributes simultaneously.", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyCanNotBePrimaryKey()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyAndPrimaryKey>();
                    db.CreateTable<TableForeignKeyAndPrimaryKey>();
                });
                Assert.Contains("Property can't have ForeignKey and PrimaryKey attributes simultaneously.", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyCanNotBeAutoIncrement()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyAndAutoIncrement>();
                    db.CreateTable<TableForeignKeyAndAutoIncrement>();
                });
                Assert.Contains("Property can't have ForeignKey and AutoIncrement attributes simultaneously.", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyCanNotHaveDefaultValue()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyWithDefaultValue>();
                    db.CreateTable<TableForeignKeyWithDefaultValue>();
                });
                Assert.Contains("Property with ForeignKey attribute can't have Default Value.", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyNameDoesNotPointToNavigationProperty()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyWithIncorrectName>();
                    db.CreateTable<TableForeignKeyWithIncorrectName>();
                });
                Assert.Contains("Can't find Navigation Property for '", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyReferencedTableWithoutCryptoTableAttribute()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyReferencedTableWithoutCryptoTableAttr>();
                    db.CreateTable<TableForeignKeyReferencedTableWithoutCryptoTableAttr>();
                });
                Assert.Contains("doesn't have Custom Attribute:", ex.Message);
            }
        }

        [Fact]
        public void ForeignKeyReferencedTableWithoutPrimaryKeyAttribute()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableForeignKeyReferencedTableWithoutPrimaryKey>();
                    db.CreateTable<TableForeignKeyReferencedTableWithoutPrimaryKey>();
                });
                Assert.Contains("doesn't contain property with PrimaryKey Attribute.", ex.Message);
            }
        }

        [Fact]
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

                    var data1 = new Data { Count = 73447, DataDescription = "Some Data Destriptionen 1" };
                    db.InsertItem(data1);
                    var data2 = new Data { Count = 298729, DataDescription = "Some Data Descriptionen 2" };
                    db.InsertItem(data2);

                    var request1 = new Request { RequestDescription = "Some Request Descriptionen 1", Value = 9128748213748273, DataRefId = 2 };
                    db.InsertItem(request1);
                    var request2 = new Request { RequestDescription = "Some Request Descriptionen 2", Value = 2034958127495, DataRefId = 1 };
                    db.InsertItem(request2);

                    var person1 = new Person { Address = "Some Addressen 1", Name = "Some Name 1", Rating = 13223, RequestRefId = 1 };
                    db.InsertItem(person1);
                    var person2 = new Person { Address = "Some Addressen 2", Name = "Some Name 2", Rating = 3029458, RequestRefId = 2 };
                    db.InsertItem(person2);
                    var person3 = new Person { Address = "Some Addressen 3", Name = "Some Name 3", Rating = 234566, RequestRefId = 2 };
                    db.InsertItem(person3);

                    var order1 = new Order { OrderDescription = "Some Order Descriptionen 1", OrderNumber = 766446, PersonRefId = 2 };
                    db.InsertItem(order1);
                    var order2 = new Order { OrderDescription = "Some Order Descriptionen 2", OrderNumber = 2363473, PersonRefId = 3 };
                    db.InsertItem(order2);
                    var order3 = new Order { OrderDescription = "Some Order Descriptionen 3", OrderNumber = 66786786, PersonRefId = 1 };
                    db.InsertItem(order3);

                    var orders = db.Find<Order>(o => o.Id == 1).ToArray();

                    Assert.NotNull(orders);
                    Assert.True(orders.Length == 1);
                    Assert.True(orders[0].Equal(order1));
                    Assert.True(orders[0].PersonNavigation.Equal(person2));
                    Assert.True(orders[0].PersonNavigation.RequestNav.Equal(request2));
                    Assert.True(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data1));

                    orders = null;
                    orders = db.Find<Order>(o => o.Id == 2).ToArray();
                    Assert.NotNull(orders);
                    Assert.True(orders.Length == 1);
                    Assert.True(orders[0].Equal(order2));
                    Assert.True(orders[0].PersonNavigation.Equal(person3));
                    Assert.True(orders[0].PersonNavigation.RequestNav.Equal(request2));
                    Assert.True(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data1));

                    orders = null;
                    orders = db.Find<Order>(o => o.Id == 3).ToArray();
                    Assert.NotNull(orders);
                    Assert.True(orders.Length == 1);
                    Assert.True(orders[0].Equal(order3));
                    Assert.True(orders[0].PersonNavigation.Equal(person1));
                    Assert.True(orders[0].PersonNavigation.RequestNav.Equal(request1));
                    Assert.True(orders[0].PersonNavigation.RequestNav.DataNavigation.Equal(data2));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void MultipleReferencedTables()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    // Dependency diagram:  
                    //                                     / ---> Account ---> Job                      / ---> 2 ---> 1             / ---> 1 ---> 2
                    //
                    //                 ManyRefTables ---> |  ---> Info                          1 ---> |  ---> 1            2 ---> |  ---> 2
                    //
                    //                                     \ ---> Client  ---> BankCount                \ ---> 2 ---> 2             \ ---> 1 ---> 1
                    //
                    db.DeleteTable<ManyRefTables>();
                    db.DeleteTable<Account>();
                    db.DeleteTable<Info>();
                    db.DeleteTable<Client>();
                    db.DeleteTable<Job>();
                    db.DeleteTable<BankCount>();

                    db.CreateTable<Job>();
                    db.CreateTable<BankCount>();
                    db.CreateTable<Account>();
                    db.CreateTable<Client>();
                    db.CreateTable<Info>();
                    db.CreateTable<ManyRefTables>();

                    var job1 = new Job { JobDescription = "Job Descriptionen 1", Price = 8223.25 };
                    db.InsertItem(job1);
                    var job2 = new Job { JobDescription = "Job Descriptionen 2", Price = 1234.25 };
                    db.InsertItem(job2);

                    var acc1 = new Account { AccountData = "Some Account Data 1", JobRefId = 2 };
                    db.InsertItem(acc1);
                    var acc2 = new Account { AccountData = "Some Account Data 2", JobRefId = 1 };
                    db.InsertItem(acc2);

                    var info1 = new Info { SomeInfo = "Some Info Descriptionen 1", SomeValue = -823782.123122 };
                    db.InsertItem(info1);
                    var info2 = new Info { SomeInfo = "Some Info Descriptionen 2", SomeValue = 1234782.234566 };
                    db.InsertItem(info2);

                    var count1 = new BankCount { CountId = 1298374892738422, Deposit = 2938489.86 };
                    db.InsertItem(count1);
                    var count2 = new BankCount { CountId = 2309458934830942, Deposit = 3.99 };
                    db.InsertItem(count2);

                    var client1 = new Client
                    {
                        CreationTime = DateTime.Now,
                        PasswordHash = "JHJIOIEUOIWEYTYTVUDBVJRHFJDHFDHJHFJDHFYYEE",
                        BankCountRefId = 1
                    };
                    db.InsertItem(client1);
                    var client2 = new Client
                    {
                        CreationTime = DateTime.Now,
                        PasswordHash = "KEIU92098293GKJLDJLGJEIEIU92384092293LKGJ",
                        BankCountRefId = 2
                    };
                    db.InsertItem(client2);

                    var manyRefTables1 = new ManyRefTables { SomeStringData = "Some Data Descriptionen 1", RequestRefId = 2, InfoRefId = 1, ClientRefId = 2 };
                    db.InsertItem(manyRefTables1);

                    var manyRefTables2 = new ManyRefTables { SomeStringData = "Some Data Descriptionen 2", RequestRefId = 1, InfoRefId = 2, ClientRefId = 1 };
                    db.InsertItem(manyRefTables2);

                    var manyRefs = db.Find<ManyRefTables>(mrt => mrt.Id == 1).ToArray();

                    Assert.NotNull(manyRefs);
                    Assert.True(manyRefs.Length == 1);
                    Assert.True(manyRefs[0].Equal(manyRefTables1));
                    Assert.True(manyRefs[0].Account.Equal(acc2));
                    Assert.True(manyRefs[0].Account.Job.Equal(job1));
                    Assert.True(manyRefs[0].Info.Equal(info1));
                    Assert.True(manyRefs[0].Client.Equal(client2));
                    Assert.True(manyRefs[0].Client.Count.Equal(count2));

                    manyRefs = null;
                    manyRefs = db.Find<ManyRefTables>(mrt => mrt.Id == 2).ToArray();

                    Assert.NotNull(manyRefs);
                    Assert.True(manyRefs.Length == 1);
                    Assert.True(manyRefs[0].Equal(manyRefTables2));
                    Assert.True(manyRefs[0].Account.Equal(acc1));
                    Assert.True(manyRefs[0].Account.Job.Equal(job2));
                    Assert.True(manyRefs[0].Info.Equal(info2));
                    Assert.True(manyRefs[0].Client.Equal(client1));
                    Assert.True(manyRefs[0].Client.Count.Equal(count1));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void ReferencedRowInTableDoesNotExist()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();

                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var simpleRef1 = new SimpleReference
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 2 /*Row Doesn't exist in Infos!!!*/
                    };

                    db.InsertItem(simpleRef1);
                });
                Assert.Contains("Column with ForeignKey constrait has invalid value or table doesn't exist in database.", ex.ProbableCause);
            }
        }

        [Fact]
        public void ReferencedTableDoesNotExistInDataBase()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    // db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();  // SimpleReference has ForeignKey constrait, referenced to Simple
                });
                Assert.Contains("Database doesn't contain table with name: Simple.", ex.Message);
            }
        }

        [Fact]
        public void DeleteTableWhenItIsForeignKeyDependencyForOtherTables()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();  // SimpleReference has ForeignKey constrait, referenced to Simple

                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var simpleRef1 = new SimpleReference
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 1 /*Row Doesn't exist in Infos!!!*/
                    };
                    db.InsertItem(simpleRef1);

                    db.DeleteTable<Simple>();   //But SimpleReference Has ForeignKey Constrait referenced to Simple!
                });
                Assert.Contains("because other tables referenced on her, using ForeignKey Constraint.", ex.Message);
            }
        }

        [Fact]
        public void ClearTableWhenItIsForeignKeyDependencyForOtherTables()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<SimpleReference>();
                    db.DeleteTable<Simple>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReference>();  // SimpleReference has ForeignKey constrait, referenced to Simple

                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var simpleRef1 = new SimpleReference
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 1 /*Row Doesn't exist in Infos!!!*/
                    };
                    db.InsertItem(simpleRef1);

                    db.ClearTable<Simple>();   //But SimpleReference Has ForeignKey Constrait referenced to Simple!
                });
                Assert.Contains("because other tables referenced on her, using ForeignKey Constraint.", ex.Message);
            }
        }

        [Fact]
        public void AutoResolveReferenceEqualsToFalse()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<SimpleReferenceWithoutAutoResolve>();

                    db.CreateTable<Simple>();
                    db.CreateTable<SimpleReferenceWithoutAutoResolve>();  // SimpleReference has ForeignKey constrait, referenced to Simple

                    var simple1 = new Simple { SimpleString = "Some Simple String 1", SimpleValue = 283423 };
                    db.InsertItem(simple1);

                    var table = db.Table<Simple>().ToArray();

                    var simpleRef1 = new SimpleReferenceWithoutAutoResolve
                    {
                        SomeData = "Some Data Descriptionen 1",
                        InfoRefId = 1
                    };
                    db.InsertItem(simpleRef1);

                    var item = db.Table<SimpleReferenceWithoutAutoResolve>().ToArray();
                    Assert.True(item.Length == 1);
                    Assert.Null(item[0].Simple);
                    db.DeleteTable<SimpleReferenceWithoutAutoResolve>();
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
