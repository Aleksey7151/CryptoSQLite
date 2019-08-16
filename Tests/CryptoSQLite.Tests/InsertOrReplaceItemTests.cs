using System;
using System.Linq;
using CryptoSQLite.Tests.Tables;
using Xunit;

namespace CryptoSQLite.Tests
{
    
    public class InsertOrReplaceItemTests : BaseTest
    {
        [Fact]
        public void UpdateShortNumbers()
        {
            var item = ShortNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ShortNumbers>();
                    db.CreateTable<ShortNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ShortNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.ShortMaxVal = 21379;
                    element.ShortMinVal = -1234;
                    element.NullAble2 = null;
                    element.NullAble1 = -7631;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ShortNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateUnsignedShortNumbers()
        {
            var item = UShortNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<UShortNumbers>();
                    db.CreateTable<UShortNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<UShortNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.UShortMaxVal = 21379;
                    element.UShortMinVal = 1234;
                    element.NullAble2 = 32322;
                    element.NullAble1 = null;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<UShortNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateIntNumbers()
        {
            var item = IntNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<IntNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.IntMaxVal = 144221379;
                    element.IntMinVal = -1231134;
                    element.NullAble1 = null;
                    element.NullAble2 = 61268182;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<IntNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateUnsignedIntNumbers()
        {
            var item = UIntNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<UIntNumbers>();
                    db.CreateTable<UIntNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<UIntNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.UIntMaxVal = 144221379;
                    element.UIntMinVal = 1231134;
                    element.NullAble1 = null;
                    element.NullAble2 = 61268182;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<UIntNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateLongNumbers()
        {
            var item = LongNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<LongNumbers>();
                    db.CreateTable<LongNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<LongNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.LongMaxVal = 785144221379;
                    element.LongMinVal = -9621231134;
                    element.NullAble1 = null;
                    element.NullAble2 = -82346292;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<LongNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateUnsignedLongNumbers()
        {
            var item = ULongNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ULongNumbers>();
                    db.CreateTable<ULongNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ULongNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.ULongMaxVal = 7851442321379;
                    element.ULongMinVal = 9621442231134;
                    element.NullAble1 = null;
                    element.NullAble2 = 8234333216292;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ULongNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateDoubleNumbers()
        {
            var item = DoubleNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DoubleNumbers>();
                    db.CreateTable<DoubleNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DoubleNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.DoubleMaxVal = 485421379.23413511;
                    element.DoubleMinVal = -96212314.15145344;
                    element.NullAble1 = null;
                    element.NullAble2 = -27348992.2384723;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DoubleNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateDecimalNumbers()
        {
            var item = DecimalNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DecimalNumbers>();
                    db.CreateTable<DecimalNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DecimalNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.MaxVal = 485425757831379.23413511m;
                    element.MinVal = -962123383783414.15145344m;
                    element.NullAble1 = null;
                    element.NullAble2 = -27348992.2384723m;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DecimalNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateFloatNumbers()
        {
            var item = FloatNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<FloatNumbers>();
                    db.CreateTable<FloatNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<FloatNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.FloatMaxVal = 422179.23422f;
                    element.FloatMinVal = -962123.12341f;
                    element.NullAble1 = null;
                    element.NullAble2 = 121112.29392f;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<FloatNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateEncryptedStrings()
        {
            var item = new EncryptedStrings { Str1 = "Hello, world!", Str2 = "Good buy, world!" };
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<EncryptedStrings>();
                    db.CreateTable<EncryptedStrings>();

                    db.InsertItem(item);

                    var element = db.Table<EncryptedStrings>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Str1 == item.Str1 && element.Str2 == item.Str2);

                    element.Str1 = "Gendalf Gray";
                    element.Str2 = "Gendalf White";

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<EncryptedStrings>().ToArray()[0];
                    Assert.NotNull(updated);

                    Assert.True(element.Str1 == updated.Str1 && element.Str2 == updated.Str2);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateBlobs()
        {
            var item = new BlobData
            {
                OpenBlob = new byte[16],
                ClosedBlob = new byte[32]
            };
            item.OpenBlob.MemSet(0x77);
            item.ClosedBlob.MemSet(0x1A);

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<BlobData>();
                    db.CreateTable<BlobData>();

                    db.InsertItem(item);    // insert element in data base

                    var element = db.Table<BlobData>().ToArray()[0];    // read inserted element from database

                    //checking equals
                    Assert.NotNull(element);
                    Assert.True(item.OpenBlob.MemCmp(element.OpenBlob) == 0 && item.ClosedBlob.MemCmp(element.ClosedBlob) == 0);
                    Assert.True(element.SingleByte == item.SingleByte);

                    // update element values
                    element.OpenBlob.MemSet(0xEE);
                    element.ClosedBlob.MemSet(0xCC);
                    element.SingleByte = 0xAD;

                    db.InsertOrReplaceItem(element);    // update element in database

                    var updated = db.Table<BlobData>().ToArray()[0];    // read updated element from database

                    // checking equals
                    Assert.NotNull(updated);
                    Assert.True(updated.OpenBlob.MemCmp(element.OpenBlob) == 0 && updated.ClosedBlob.MemCmp(element.ClosedBlob) == 0);
                    Assert.True(element.SingleByte == updated.SingleByte);
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateShortEncryptedNumbers()
        {
            var item = ShortEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ShortEncryptedNumbers>();
                    db.CreateTable<ShortEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ShortEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.ShortMaxVal = 21379;
                    element.ShortMinVal = -1234;
                    element.NullAble2 = null;
                    element.NullAble1 = -12311;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ShortEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateUnsignedShortEncryptedNumbers()
        {
            var item = UShortEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<UShortEncryptedNumbers>();
                    db.CreateTable<UShortEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<UShortEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.UShortMaxVal = 21379;
                    element.UShortMinVal = 1234;
                    element.NullAble2 = 23472;
                    element.NullAble1 = null;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<UShortEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateIntEncryptedNumbers()
        {
            var item = IntEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntEncryptedNumbers>();
                    db.CreateTable<IntEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<IntEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.IntMaxVal == item.IntMaxVal && element.IntMinVal == item.IntMinVal);

                    element.IntMaxVal = 144221379;
                    element.IntMinVal = -12331134;
                    element.NullAble1 = null;
                    element.NullAble2 = 210958433;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<IntEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateUsignedIntEncryptedNumbers()
        {
            var item = UIntEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<UIntEncryptedNumbers>();
                    db.CreateTable<UIntEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<UIntEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.UIntMaxVal = 144221379;
                    element.UIntMinVal = 1231134;
                    element.NullAble1 = null;
                    element.NullAble2 = 1872498322;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<UIntEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateLongEncryptedNumbers()
        {
            var item = LongEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<LongEncryptedNumbers>();
                    db.CreateTable<LongEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<LongEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.LongMaxVal = 785144221379;
                    element.LongMinVal = -9621231134;
                    element.NullAble1 = null;
                    element.NullAble2 = -29874287348222;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<LongEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateDoubleEncryptedNumbers()
        {
            var item = DoubleEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DoubleEncryptedNumbers>();
                    db.CreateTable<DoubleEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DoubleEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.DoubleMaxVal = 485421379.23413511;
                    element.DoubleMinVal = -96212314.15145344;
                    element.NullAble1 = null;
                    element.NullAble2 = -23874827.8934298;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DoubleEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateFloatEncryptedNumbers()
        {
            var item = FloatEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<FloatEncryptedNumbers>();
                    db.CreateTable<FloatEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<FloatEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.FloatMaxVal = 422179.23422f;
                    element.FloatMinVal = -962123.12341f;
                    element.NullAble1 = null;
                    element.NullAble2 = -38728.222f;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<FloatEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        [Fact]
        public void UpdateDecimalEncryptedNumbers()
        {
            var item = DecimalEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DecimalEncryptedNumbers>();
                    db.CreateTable<DecimalEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DecimalEncryptedNumbers>().ToArray()[0];

                    Assert.NotNull(element);
                    Assert.True(element.Equals(item));

                    element.MaxVal = 48539333421379.23413511m;
                    element.MinVal = -96212314.1514558555344m;
                    element.NullAble1 = null;
                    element.NullAble2 = -23874827.8943333334298m;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DecimalEncryptedNumbers>().ToArray()[0];
                    Assert.NotNull(updated);
                    Assert.True(element.Equals(updated));
                }
                finally
                {
                    db.Dispose();
                }
            }
        }
    }
}
