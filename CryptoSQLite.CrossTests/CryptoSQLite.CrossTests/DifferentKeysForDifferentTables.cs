using System;
using System.Linq;
using CryptoSQLite.CrossTests.Tables;
using NUnit.Framework;

namespace CryptoSQLite.CrossTests
{
    [TestFixture]
    public class DifferentKeysForDifferentTables : BaseTest
    {
        [Test]
        public void DifferentKeysForThreeTablesWithOneDefaultKey()
        {
            var t1 = IntEncryptedNumbers.GetDefault();
            var k1 = new byte[32];      // k1 - key for t1
            k1.MemSet(11);

            var t2 = DoubleEncryptedNumbers.GetDefault();
            var k2 = new byte[32];      // k2 - key for t2
            k2.MemSet(22);

            var t3 = LongEncryptedNumbers.GetDefault();

            var t4 = FloatEncryptedNumbers.GetDefault();

            var k3 = new byte[32];      // k3 - key for all rest tables
            k3.MemSet(33);

            foreach (var db in GetOnlyConnections())
            {
                try
                {
                    db.DeleteTable<IntEncryptedNumbers>();
                    db.CreateTable<IntEncryptedNumbers>();

                    db.DeleteTable<DoubleEncryptedNumbers>();
                    db.CreateTable<DoubleEncryptedNumbers>();

                    db.DeleteTable<LongEncryptedNumbers>();
                    db.CreateTable<LongEncryptedNumbers>();

                    db.DeleteTable<FloatEncryptedNumbers>();
                    db.CreateTable<FloatEncryptedNumbers>();

                    db.SetEncryptionKey<IntEncryptedNumbers>(k1);   // this Key only for table IntEncryptedNumbers
                    db.SetEncryptionKey<DoubleEncryptedNumbers>(k2);         // this Key only for table DoubleNumbers

                    db.SetEncryptionKey(k3);                        // this Key for all the rest tables

                    db.InsertItem(t1);
                    db.InsertItem(t2);
                    db.InsertItem(t3);
                    db.InsertItem(t4);

                    var result = db.Find<IntEncryptedNumbers>(i => i.Id == 1);
                    Assert.NotNull(result);
                    var table = result.ToArray();
                    Assert.IsTrue(table.Length == 1);
                    Assert.IsTrue(table[0].Equals(t1));

                    var result1 = db.Find<DoubleEncryptedNumbers>(i => i.Id == 1);
                    Assert.NotNull(result);
                    var table1 = result1.ToArray();
                    Assert.IsTrue(table1.Length == 1);
                    Assert.IsTrue(table1[0].Equals(t2));

                    var result2 = db.Find<LongEncryptedNumbers>(i => i.Id == 1);
                    Assert.NotNull(result);
                    var table2 = result2.ToArray();
                    Assert.IsTrue(table2.Length == 1);
                    Assert.IsTrue(table2[0].Equals(t3));

                    var result3 = db.Find<FloatEncryptedNumbers>(i => i.Id == 1);
                    Assert.NotNull(result);
                    var table3 = result3.ToArray();
                    Assert.IsTrue(table3.Length == 1);
                    Assert.IsTrue(table3[0].Equals(t4));
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
