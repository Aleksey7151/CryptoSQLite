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

        [CryptoTable("Work")]
        public class Work
        {
            public int ID { get; set; }
        }

        [CryptoTable("Employee")]
        public class Employee
        {
            public int Id { get; set; }

            public string Name { get; set; }
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
                    db.CreateTable<Jobs>();

                    db.SetEncryptionKey(key);
                    
                    
                    var item = new Jobs
                    {
                        Id = 1,
                        JobName = "I",
                        Description = "A",
                        IsCompleted = true
                    };
                    db.InsertOrReplaceItem(item);

                 
                    var table = db.Table<Jobs>().ToList();

                    var i = table[0];
                    
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                
            }
           
        }
    }
}
