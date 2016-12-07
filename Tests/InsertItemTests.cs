using System;
using System.Linq;
using CryptoSQLite;
using NUnit.Framework;
using Tests.Tables;

namespace Tests
{
    [TestFixture]
    public class InsertItemTests : BaseTest
    {
        [Test]
        public void CreateCryptoTableInDatabase()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.CreateTable<AccountsData>();
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
        public void InsertElementInTableThatDoNotExistInDatabaseFile()
        {
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();

                    db.InsertItem(new AccountsData());
                }
                catch (CryptoSQLiteException cex)
                {
                    Assert.IsTrue(
                        cex.Message.IndexOf("Database doesn't contain table with name", StringComparison.Ordinal) >=
                        0);
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
        public void InsertShortsNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.Equals(item));
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
        public void InsertEncryptedShortsNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ShortMaxVal == item.ShortMaxVal && element.ShortMinVal == item.ShortMinVal);
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
        public void InsertUnsignedShortsNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.UShortMaxVal == item.UShortMaxVal && element.UShortMinVal == item.UShortMinVal);
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
        public void InsertUnsignedEncryptedShortsNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.UShortMaxVal == item.UShortMaxVal && element.UShortMinVal == item.UShortMinVal);
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
        public void InsertIntNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.IntMaxVal == item.IntMaxVal && element.IntMinVal == item.IntMinVal);
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
        public void InsertIntEncryptedNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.IntMaxVal == item.IntMaxVal && element.IntMinVal == item.IntMinVal);
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
        public void InsertUIntNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.UIntMaxVal == item.UIntMaxVal && element.UIntMinVal == item.UIntMinVal);
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
        public void InsertUIntEncryptedNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.UIntMaxVal == item.UIntMaxVal && element.UIntMinVal == item.UIntMinVal);
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
        public void InsertLongNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.LongMinVal == item.LongMinVal && element.LongMaxVal == item.LongMaxVal);
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
        public void InsertLongEncryptedNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.LongMinVal == item.LongMinVal && element.LongMaxVal == item.LongMaxVal);
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
        public void InsertULongNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ULongMinVal == item.ULongMinVal && element.ULongMaxVal == item.ULongMaxVal);
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
        public void InsertULongEncryptedNumbers()
        {
            var item = ULongEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ULongEncryptedNumbers>();
                    db.CreateTable<ULongEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ULongEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ULongMinVal == item.ULongMinVal && element.ULongMaxVal == item.ULongMaxVal);
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
        public void InsertDateTimeValue()
        {
            var item1 = new DateTimeTable
            {
                Date = DateTime.Now,
                EncryptedDate = DateTime.Now
            };

            var item2 = new DateTimeTable
            {
                Date = DateTime.MinValue,
                EncryptedDate = DateTime.MinValue
            };

            var item3 = new DateTimeTable
            {
                Date = DateTime.MaxValue,
                EncryptedDate = DateTime.MaxValue
            };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<DateTimeTable>();
                    db.CreateTable<DateTimeTable>();

                    db.InsertItem(item1);
                    db.InsertItem(item2);
                    db.InsertItem(item3);

                    var elements = db.Table<DateTimeTable>().ToArray();

                    Assert.IsNotNull(elements);

                    Assert.IsTrue(DateTime.Equals(elements[0].Date, item1.Date) && DateTime.Equals(elements[0].EncryptedDate, item1.EncryptedDate));
                    Assert.IsTrue(DateTime.Equals(elements[1].Date, item2.Date) && DateTime.Equals(elements[1].EncryptedDate, item2.EncryptedDate));
                    Assert.IsTrue(DateTime.Equals(elements[2].Date, item3.Date) && DateTime.Equals(elements[2].EncryptedDate, item3.EncryptedDate));
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
        public void InsertFloatNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - item.FloatMaxVal) < 0.00001  && Math.Abs(element.FloatMinVal - item.FloatMinVal) < 0.00001);
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
        public void InsertFloatEncryptedNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.FloatMaxVal - item.FloatMaxVal) < 0.00001 && Math.Abs(element.FloatMinVal - item.FloatMinVal) < 0.00001);
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
        public void InsertBoolEncryptedValues()
        {
            var item = BoolEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<BoolEncryptedNumbers>();
                    db.CreateTable<BoolEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<BoolEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.B1 == item.B1 && element.B2 == item.B2 && element.B3 == item.B3 && element.B4 == item.B4);
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
        public void InsertByteEncryptedValues()
        {
            var item = ByteEncryptedNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ByteEncryptedNumbers>();
                    db.CreateTable<ByteEncryptedNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ByteEncryptedNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ByteMaxVal == item.ByteMaxVal && element.ByteMinVal == item.ByteMinVal);
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
        public void InsertByteValues()
        {
            var item = ByteNumbers.GetDefault();
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<ByteNumbers>();
                    db.CreateTable<ByteNumbers>();

                    db.InsertItem(item);

                    var element = db.Table<ByteNumbers>().ToArray()[0];

                    Assert.IsNotNull(element);
                    Assert.IsTrue(element.ByteMaxVal == item.ByteMaxVal && element.ByteMinVal == item.ByteMinVal);
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
        public void InsertDoubleNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - item.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - item.DoubleMinVal) < 0.000001);
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
        public void InsertDoubleEncryptedNumbers()
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

                    Assert.IsNotNull(element);
                    Assert.IsTrue(Math.Abs(element.DoubleMaxVal - item.DoubleMaxVal) < 0.000001 && Math.Abs(element.DoubleMinVal - item.DoubleMinVal) < 0.000001);
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
        public void InserdBlobData()
        {
            var item = new BlobData
            {
                OpenBlob = new byte[16],
                ClosedBlob = new byte[32]
            };
            item.OpenBlob.MemSet(0x17);
            item.ClosedBlob.MemSet(0xCB);

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<BlobData>();
                    db.CreateTable<BlobData>();

                    db.InsertItem(item);

                    var element = db.Table<BlobData>().ToArray()[0];

                    Assert.IsNotNull(element);

                    Assert.IsTrue(item.OpenBlob.MemCmp(element.OpenBlob) == 0 && item.ClosedBlob.MemCmp(element.ClosedBlob) == 0);

                    Assert.IsTrue(element.SingleByte == item.SingleByte);
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
        public void InsertNormalElementInCryptoTable()
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = "Frodo Beggins",
                AccountPassword = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            var account2 = new AccountsData
            {
                Id = 66,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = uint.MaxValue,
                AccountName = "Gendalf Gray",
                AccountPassword = "I am master of Anor flame.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(account1);
                    db.InsertItem(account2);

                    var table = db.Table<AccountsData>().ToArray();

                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account1)));
                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account2)));
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
        public void InsertNormalElementInCryptoTableThatHasNullValues()
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = null,
                AccountPassword = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            
            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(account1);

                    var table = db.Table<AccountsData>().ToArray();

                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account1)));
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
        public void InsertNormalElementInCryptoTableThatHasEncryptedColumnsWithNullValues()
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                AccountName = null,
                AccountPassword = null,
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };


            foreach (var db in GetConnections())
            {
                try
                {
                    db.DeleteTable<AccountsData>();
                    db.CreateTable<AccountsData>();

                    db.InsertItem(account1);

                    var table = db.Table<AccountsData>().ToArray();

                    Assert.IsTrue(table.Any(e => e.IsTableEqualsTo(account1)));
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
