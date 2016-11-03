using System;
using System.Linq;
using CryptoSQLite;

namespace TestConsoleApp
{
    class Program
    {
        [CryptoTable("Jobs")]
        public class Jobs
        {
            [Column("id"), PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            [Encrypted]
            public string JobName { get; set; }

            [Encrypted]
            public string Description { get; set; }

            public bool IsCompleted { get; set; }
        }

        [CryptoTable("ULongNumbers")]
        internal class ULongNumbers
        {
            [PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            public ulong ULongMaxVal { get; set; }

            public ulong ULongMinVal { get; set; }
        }

        [CryptoTable("ULongEncryptedNumbers")]
        internal class ULongEncryptedNumbers
        {
            [PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            [Encrypted]
            public ulong ULongMaxVal { get; set; }

            [Encrypted]
            public ulong ULongMinVal { get; set; }
        }

        [CryptoTable("LongNumbers")]
        internal class LongNumbers
        {
            [PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            public long LongMaxVal { get; set; }

            public long LongMinVal { get; set; }
        }

        [CryptoTable("LongEncryptedNumbers")]
        internal class LongEncryptedNumbers
        {
            [PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            [Encrypted]
            public long LongMaxVal { get; set; }

            [Encrypted]
            public long LongMinVal { get; set; }
        }

        [CryptoTable("DoubleNumbers")]
        internal class DoubleNumbers
        {
            [PrimaryKey, AutoIncremental]
            public int Id { get; set; }

            public double DoubleMaxVal { get; set; }

            public double DoubleMinVal { get; set; }
        }

        static void Main()
        {
            var key = new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32
            };

            using (var db = new CryptoSQLiteConnection("aes.db3", CryptoAlgoritms.AesWith256BitsKey))
            {
                try
                {
                    db.SetEncryptionKey(key);

                    db.DeleteTable<ULongNumbers>();
                    db.DeleteTable<ULongEncryptedNumbers>();
                    db.DeleteTable<LongNumbers>();
                    db.DeleteTable<LongEncryptedNumbers>();
                    db.DeleteTable<DoubleNumbers>();

                    db.CreateTable<ULongNumbers>();
                    db.CreateTable<ULongEncryptedNumbers>();
                    db.CreateTable<LongNumbers>();
                    db.CreateTable<LongEncryptedNumbers>();
                    db.CreateTable<DoubleNumbers>();
                    bool i;
                    {
                        var uln = new ULongNumbers
                        {
                            ULongMaxVal = ulong.MaxValue,
                            ULongMinVal = ulong.MinValue
                        };

                        db.InsertItem(uln);
                        var tuln = db.Table<ULongNumbers>().ToArray();
                        i = tuln[0].ULongMaxVal == uln.ULongMaxVal && tuln[0].ULongMinVal == uln.ULongMinVal;
                        Console.WriteLine(i.ToString());
                    }
                    {
                        var ulen = new ULongEncryptedNumbers
                        {
                            ULongMaxVal = ulong.MaxValue,
                            ULongMinVal = ulong.MinValue
                        };
                        db.InsertItem(ulen);
                        var tulen = db.Table<ULongEncryptedNumbers>().ToArray();
                        i = tulen[0].ULongMaxVal == ulen.ULongMaxVal && tulen[0].ULongMinVal == ulen.ULongMinVal;
                        Console.WriteLine(i.ToString());
                    }
                    {
                        var ln = new LongNumbers
                        {
                            LongMaxVal = long.MaxValue,
                            LongMinVal = long.MinValue
                        };
                        db.InsertItem(ln);
                        var tln = db.Table<LongNumbers>().ToArray();
                        i = tln[0].LongMaxVal == ln.LongMaxVal && tln[0].LongMinVal == ln.LongMinVal;
                        Console.WriteLine(i.ToString());
                    }
                    {
                        var len = new LongEncryptedNumbers
                        {
                            LongMaxVal = long.MaxValue,
                            LongMinVal = long.MinValue
                        };
                        db.InsertItem(len);
                        var tlen = db.Table<LongNumbers>().ToArray();
                        i = tlen[0].LongMaxVal == len.LongMaxVal && tlen[0].LongMinVal == len.LongMinVal;
                        Console.WriteLine(i.ToString());
                    }

                    Console.ReadLine();

                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                
            }
           
        }
    }
}
