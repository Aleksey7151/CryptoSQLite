using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CryptoSQLite;
using NUnit.Framework;


namespace Tests
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

        [Encrypted, NotNull]
        public byte[] NotNullBytes { get; set; }

        [NotNull, Encrypted]
        public double? NotNullDouble { get; set; }

        public bool Equal(TableWithNotNullAttrs o)
        {
            return NotNullInt == o.NotNullInt && NotNullString == o.NotNullString &&
                   NotNullBytes.MemCmp(o.NotNullBytes) == 0 && Math.Abs(NotNullDouble.Value - o.NotNullDouble.Value) < 0.0000001;
        }
    }

    [CryptoTable("TableWithDefaultValue")]
    public class TableWithDefaultValue
    {
        public const int DefaultInt = 7;

        public const string DefaultString = "Hello World";

        public const double DefaultDouble = 7.7;

        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [NotNull(DefaultInt)]
        public int? NotNullInt { get; set; }

        [NotNull(DefaultString)]
        public string NotNullString { get; set; }   // SQL: NotNullString TEXT DEFAULT 'Hello World'

        /*
        [NotNull(new byte[] {1,2,3,4,5,6,7,8})]
        public byte[] NotNullBytes { get; set; }
        */

        [NotNull(DefaultDouble)]
        public double? NotNullDouble { get; set; }
    }

    [TestFixture]
    public class DefaultValueTests : BaseTest
    {
        [Test]
        public void NotNullAttribute_Without_DefaultValue()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs {NotNullInt = 5, NotNullString = "Hello", NotNullBytes = new byte[8], NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                    var elements = db.Find<TableWithNotNullAttrs>(i => i.NotNullInt == 5).ToArray();
                    Assert.IsNotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(item1.Equal(elements[0]));
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

        [Test]
        public void NotNullAttribute_Without_DefaultValueForIntegers()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs {/*NotNullInt = 5,*/ NotNullString = "Hello", NotNullBytes = new byte[8], NotNullDouble = 4578783.23882};
                    db.InsertItem(item1);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.ProbableCause.IndexOf("Causes: 1. Table doesn't exist in database.\n2. Value for NOT NULL column is not set.", StringComparison.Ordinal) >= 0);
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
        public void NotNullAttribute_Without_DefaultValueForText()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs {NotNullInt = 5, /*NotNullString = "Hello",*/ NotNullBytes = new byte[8], NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.ProbableCause.IndexOf("Causes: 1. Table doesn't exist in database.\n2. Value for NOT NULL column is not set.", StringComparison.Ordinal) >= 0);
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
        public void NotNullAttribute_Without_DefaultValueForBlob()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, NotNullString = "Hello", /*NotNullBytes = new byte[8],*/ NotNullDouble = 4578783.23882 };
                    db.InsertItem(item1);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.ProbableCause.IndexOf("Causes: 1. Table doesn't exist in database.\n2. Value for NOT NULL column is not set.", StringComparison.Ordinal) >= 0);
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
        public void NotNullAttribute_Without_DefaultValueForReal()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithNotNullAttrs>();
                    db.CreateTable<TableWithNotNullAttrs>();

                    var item1 = new TableWithNotNullAttrs { NotNullInt = 5, NotNullString = "Hello", NotNullBytes = new byte[8]/*,NotNullDouble = 4578783.23882*/ };
                    db.InsertItem(item1);
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(cex.ProbableCause.IndexOf("Causes: 1. Table doesn't exist in database.\n2. Value for NOT NULL column is not set.", StringComparison.Ordinal) >= 0);
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
        public void NotNullAttribute_With_DefaultValue()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<TableWithDefaultValue>();
                    db.CreateTable<TableWithDefaultValue>();

                    var item1 = new TableWithDefaultValue {NotNullDouble = 1234.1231, NotNullInt = 1234567};
                    db.InsertItem(item1);
                    var elements = db.Table<TableWithDefaultValue>().ToArray();
                    Assert.IsNotNull(elements);
                    Assert.IsTrue(elements.Length == 1);
                    Assert.IsTrue(Math.Abs(elements[0].NotNullDouble.Value - 1234.1231) <
                                  0.000001 &&
                                  elements[0].NotNullInt == 1234567 &&
                                  elements[0].NotNullString == TableWithDefaultValue.DefaultString);
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
