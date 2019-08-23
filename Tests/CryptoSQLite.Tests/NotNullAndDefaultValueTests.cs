using System;
using System.Linq;
using Xunit;

namespace CryptoSQLite.Tests
{
    [CryptoTable("TableWithNotNullAttrs")]
    public class TableWithNotNullAttrs
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [NotNull]
        public int? NotNullInt { get; set; }

        [NotNull]
        public string NotNullString { get; set; }

        [NotNull, Encrypted]
        public byte[] NotNullBytes { get; set; }

        [NotNull, Encrypted]
        public double? NotNullDouble { get; set; }

        public bool Equal(TableWithNotNullAttrs o)
        {
            var doubleEquals = false;
            if (NotNullDouble == null && o.NotNullDouble == null)
                doubleEquals = true;
            else if (NotNullDouble != null && o.NotNullDouble != null)
                doubleEquals = Math.Abs(NotNullDouble.Value - o.NotNullDouble.Value) < 0.000001;

            return NotNullInt == o.NotNullInt && NotNullString == o.NotNullString &&
                NotNullBytes.MemCmp(o.NotNullBytes) == 0 && doubleEquals;
        }
    }

    [CryptoTable("TableWithDefaultValue")]
    public class TableWithDefaultValue
    {
        public const int DefaultInt = 71223;

        public const string DefaultString = "Hello World";

        public const double DefaultDouble = 71211.732346;

        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [NotNull(DefaultInt)]
        public int? NotNullInt { get; set; }

        [NotNull(DefaultString)]
        public string NotNullString { get; set; }   // SQL: NotNullString TEXT DEFAULT 'Hello World'

        [NotNull(DefaultDouble)]
        public double? NotNullDouble { get; set; }
    }

    [Collection("Sequential")]
    public class NotNullAndDefaultValueTests : BaseTest
    {

        [Fact]
        public void ErrorWhenPassingNullForNotNullColumn()
        {
            using (var db = GetGostConnection())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    var item = new TableWithNotNullAttrs();     // here we have defined NotNull attributes, but not defined Default Values for them!!
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();
                    db.InsertItem(item);
                });
                Assert.Contains("You are trying to pass NULL-value for Column ", ex.Message);
            }
        }

        [Fact]
        public void NotNullAttribute_Without_DefaultValue_Doing_Nothing()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, NotNullString = "Hello", NotNullBytes = new byte[8], NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                    var elements = db.Find<TableWithNotNullAttrs>(i => i.NotNullInt == 5).ToArray();
                    Assert.NotNull(elements);
                    Assert.True(elements.Length == 1);
                    Assert.True(item1.Equal(elements[0]));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void NotNullAttribute_Without_DefaultValueForIntegers()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();
                    var item1 = new TableWithNotNullAttrs {/*NotNullInt = 5,*/ NotNullString = "Hello", NotNullBytes = new byte[8], NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                });
                Assert.Contains("You are trying to pass NULL-value for Column '", ex.Message);
            }
        }

        [Fact]
        public void NotNullAttribute_Without_DefaultValueForText()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();
                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, /*NotNullString = "Hello",*/ NotNullBytes = new byte[8], NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                });
                Assert.Contains("You are trying to pass NULL-value for Column '", ex.Message);
            }
        }

        [Fact]
        public void NotNullAttribute_Without_DefaultValueForBlob()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();
                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, NotNullString = "Hello", /*NotNullBytes = new byte[8],*/ NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                });
                Assert.Contains("You are trying to pass NULL-value for Column '", ex.Message);
            }
        }

        [Fact]
        public void NotNullAttribute_Without_DefaultValueForReal()
        {
            foreach (var db in GetConnections())
            {
                var ex = Assert.Throws<CryptoSQLiteException>(() =>
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();
                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, NotNullString = "Hello", NotNullBytes = new byte[8]/*,NotNullDouble = 4578783.23882*/ };
                    db.InsertItem(item1);
                });
                Assert.Contains("You are trying to pass NULL-value for Column '", ex.Message);
            }
        }

        [Fact]
        public void String_NotNullAttribute_With_DefaultValue()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithDefaultValue>();
                    db.CreateTable<TableWithDefaultValue>();

                    var item1 = new TableWithDefaultValue { NotNullDouble = 1234.1231, NotNullInt = 1234567 };
                    db.InsertItem(item1);
                    var elements = db.Table<TableWithDefaultValue>().ToArray();
                    Assert.NotNull(elements);
                    Assert.True(elements.Length == 1);
                    Assert.True(Math.Abs(elements[0].NotNullDouble.Value - 1234.1231) <
                                  0.000001 &&
                                  elements[0].NotNullInt == 1234567 &&
                                  elements[0].NotNullString == TableWithDefaultValue.DefaultString);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Int_NotNullAttribute_With_DefaultValue()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithDefaultValue>();
                    db.CreateTable<TableWithDefaultValue>();

                    var item1 = new TableWithDefaultValue { NotNullDouble = 1234.1231, NotNullString = "Frodo" };
                    db.InsertItem(item1);
                    var elements = db.Table<TableWithDefaultValue>().ToArray();
                    Assert.NotNull(elements);
                    Assert.True(elements.Length == 1);
                    Assert.True(Math.Abs(elements[0].NotNullDouble.Value - 1234.1231) <
                                  0.000001 &&
                                  elements[0].NotNullInt == TableWithDefaultValue.DefaultInt &&
                                  elements[0].NotNullString == "Frodo");
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void Double_NotNullAttribute_With_DefaultValue()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithDefaultValue>();
                    db.CreateTable<TableWithDefaultValue>();

                    var item1 = new TableWithDefaultValue { NotNullInt = 123441, NotNullString = "Frodo" };
                    db.InsertItem(item1);
                    var elements = db.Table<TableWithDefaultValue>().ToArray();
                    Assert.NotNull(elements);
                    Assert.True(elements.Length == 1);
                    Assert.True(Math.Abs(elements[0].NotNullDouble.Value - TableWithDefaultValue.DefaultDouble) < 0.000001 &&
                                  elements[0].NotNullInt == 123441 &&
                                  elements[0].NotNullString == "Frodo");
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
