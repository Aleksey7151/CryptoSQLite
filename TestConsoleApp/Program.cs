using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite;

namespace TestConsoleApp
{
    class Program
    {
        [CryptoTable("Jobs")]
        public class Jobs
        {
            [Column("id"), PrimaryKey]
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

        static void Main(string[] args)
        {
            var key = new byte[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32};
            using (var db = new CryptoSQLite.CryptoSQLite("myDb.db3"))
            {
                //db.CreateTable<Jobs>();
                db.SetEncryptionKey(key);
                var item = new Jobs
                {
                    Id = 66,
                    JobName = "Finish CryptoSQLite.",
                    Description = "It is wery difficult Job.",
                    IsCompleted = false
                };

                db.Insert(item);
            }
           
        }
    }
}
