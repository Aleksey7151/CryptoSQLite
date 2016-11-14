using System;
using System.Linq;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class UpdateItemTests : BaseTest
    {
        [Test]
        public void UpdateShortNumbers()
        {
            var item = new ShortNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ShortNumbers>();
                    db.CreateTable<ShortNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ShortNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ShortMaxVal == item.ShortMaxVal && element.ShortMinVal == item.ShortMinVal);

                    element.ShortMaxVal = 21379;
                    element.ShortMinVal = -1234;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ShortNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.ShortMaxVal == updated.ShortMaxVal && element.ShortMinVal == updated.ShortMinVal);
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

        [Test]
        public void UpdateIntNumbers()
        {
            var item = new IntNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntNumbers>();
                    db.CreateTable<IntNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<IntNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.IntMaxVal == item.IntMaxVal && element.IntMinVal == item.IntMinVal);

                    element.IntMaxVal = 144221379;
                    element.IntMinVal = -1231134;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<IntNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.IntMaxVal == updated.IntMaxVal && element.IntMinVal == updated.IntMinVal);
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

        [Test]
        public void UpdateLongNumbers()
        {
            var item = new LongNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<LongNumbers>();
                    db.CreateTable<LongNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<LongNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.LongMaxVal == item.LongMaxVal && element.LongMinVal == item.LongMinVal);

                    element.LongMaxVal = 785144221379;
                    element.LongMinVal = -9621231134;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<LongNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.LongMaxVal == updated.LongMaxVal && element.LongMinVal == updated.LongMinVal);
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

        [Test]
        public void UpdateDoubleNumbers()
        {
            var item = new DoubleNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DoubleNumbers>();
                    db.CreateTable<DoubleNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DoubleNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - item.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - item.DoubleMinVal) < 0.000001);

                    element.DoubleMaxVal = 485421379.23413511;
                    element.DoubleMinVal = -96212314.15145344;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DoubleNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - updated.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - updated.DoubleMinVal) < 0.000001);
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

        [Test]
        public void UpdateFloatNumbers()
        {
            var item = new FloatNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<FloatNumbers>();
                    db.CreateTable<FloatNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<FloatNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - item.FloatMaxVal) < 0.00001 && Math.Abs(element.FloatMinVal - item.FloatMinVal) < 0.00001);

                    element.FloatMaxVal = 422179.23422f;
                    element.FloatMinVal = -962123.12341f;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<FloatNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - updated.FloatMaxVal) < 0.00001 && Math.Abs(element.FloatMinVal - updated.FloatMinVal) < 0.00001);
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

        [Test]
        public void UpdateEncryptedStrings()
        {
            var item = new EncryptedStrings {Str1 = "Hello, world!", Str2 = "Good buy, world!"};
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<EncryptedStrings>();
                    db.CreateTable<EncryptedStrings>();

                    db.InsertItem(item);

                    var element = db.Table<EncryptedStrings>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.Str1 == item.Str1 && element.Str2 == item.Str2);

                    element.Str1 = "Gendalf Gray";
                    element.Str2 = "Gendalf White";

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<EncryptedStrings>().ToArray()[0];
                    Assert.IsNotNull(updated);

                    Assert.IsTrue(element.Str1 == updated.Str1 && element.Str2 == updated.Str2);
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

        [Test]
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
                    Assert.IsNotNull(element);
                    Assert.IsTrue(item.OpenBlob.MemCmp(element.OpenBlob) == 0 && item.ClosedBlob.MemCmp(element.ClosedBlob) == 0);
                    Assert.IsTrue(element.SingleByte == item.SingleByte);

                    // update element values
                    element.OpenBlob.MemSet(0xEE);
                    element.ClosedBlob.MemSet(0xCC);
                    element.SingleByte = 0xAD;

                    db.InsertOrReplaceItem(element);    // update element in database

                    var updated = db.Table<BlobData>().ToArray()[0];    // read updated element from database

                    // checking equals
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(updated.OpenBlob.MemCmp(element.OpenBlob) == 0 && updated.ClosedBlob.MemCmp(element.ClosedBlob) == 0);
                    Assert.IsTrue(element.SingleByte == updated.SingleByte);
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

        [Test]
        public void UpdateShortEncryptedNumbers()
        {
            var item = new ShortEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ShortEncryptedNumbers>();
                    db.CreateTable<ShortEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ShortEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ShortMaxVal == item.ShortMaxVal && element.ShortMinVal == item.ShortMinVal);

                    element.ShortMaxVal = 21379;
                    element.ShortMinVal = -1234;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<ShortEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.ShortMaxVal == updated.ShortMaxVal && element.ShortMinVal == updated.ShortMinVal);
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

        [Test]
        public void UpdateIntEncryptedNumbers()
        {
            var item = new IntEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<IntEncryptedNumbers>();
                    db.CreateTable<IntEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<IntEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.IntMaxVal == item.IntMaxVal && element.IntMinVal == item.IntMinVal);

                    element.IntMaxVal = 144221379;
                    element.IntMinVal = -1231134;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<IntEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.IntMaxVal == updated.IntMaxVal && element.IntMinVal == updated.IntMinVal);
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

        [Test]
        public void UpdateLongEncryptedNumbers()
        {
            var item = new LongEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<LongEncryptedNumbers>();
                    db.CreateTable<LongEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<LongEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.LongMaxVal == item.LongMaxVal && element.LongMinVal == item.LongMinVal);

                    element.LongMaxVal = 785144221379;
                    element.LongMinVal = -9621231134;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<LongEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(element.LongMaxVal == updated.LongMaxVal && element.LongMinVal == updated.LongMinVal);
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

        [Test]
        public void UpdateDoubleEncryptedNumbers()
        {
            var item = new DoubleEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DoubleEncryptedNumbers>();
                    db.CreateTable<DoubleEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<DoubleEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - item.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - item.DoubleMinVal) < 0.000001);

                    element.DoubleMaxVal = 485421379.23413511;
                    element.DoubleMinVal = -96212314.15145344;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<DoubleEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - updated.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - updated.DoubleMinVal) < 0.000001);
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

        [Test]
        public void UpdateFloatEncryptedNumbers()
        {
            var item = new FloatEncryptedNumbers();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<FloatEncryptedNumbers>();
                    db.CreateTable<FloatEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<FloatEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - item.FloatMaxVal) < 0.00001 && Math.Abs(element.FloatMinVal - item.FloatMinVal) < 0.00001);

                    element.FloatMaxVal = 422179.23422f;
                    element.FloatMinVal = -962123.12341f;

                    db.InsertOrReplaceItem(element);

                    var updated = db.Table<FloatEncryptedNumbers>().ToArray()[0];
                    Assert.IsNotNull(updated);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - updated.FloatMaxVal) < 0.00001 && Math.Abs(element.FloatMinVal - updated.FloatMinVal) < 0.00001);
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
