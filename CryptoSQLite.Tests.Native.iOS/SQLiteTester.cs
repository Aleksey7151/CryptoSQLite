using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace CryptoSQLite.Tests.Native.iOS
{
    [CryptoTable("MyTasks")]
    public class Tasks
    {
        [PrimaryKey, AutoIncremental]
        public int Id { get; set; }

        [Encrypted, NotNull]
        public string Task { get; set; }

        [NotNull("Default Description")]
        public string Description { get; set; }

        [Column("PriceForTask")]
        public double? Price { get; set; }

        public bool IsDone { get; set; }

        public int InfoId { get; set; }		// For joining two tables: Tasks and Info.
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

    internal class JoinResult
    {
        public Tasks Tasks { get; set; }

        public Info Infos { get; set; }

        public JoinResult(Tasks tasks, Info info)
        {
            Tasks = tasks;
            Infos = info;
        }
    }
    class SQLiteTester
    {
        public void StartSQLiteTests(string fileName)
        {
            CryptoSQLiteConnection db = null;

            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder

                var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder

                var pathToDatabaseFile = Path.Combine(libraryPath, fileName);

                db = new CryptoSQLiteConnection(pathToDatabaseFile, CryptoAlgoritms.AesWith256BitsKey);

                // SETTING ENCRYPTION KEY FOR ALL TABLES IN DATABASE FILE
                var keyForDatabase = new byte[32];
                db.SetEncryptionKey(keyForDatabase);

                var specificKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2 };
                // SITTING SPECIFIC ENCRYPTION KEY ONLY FOR TABLE 'Infos'. THIS KEY WILL BE USED ONLY IN TABLE 'Infos' FOR ENCRYPTION DATA.

                db.CreateTable<Tasks>();
                db.CreateTable<Info>();

                // All Properties that have [Encrypted] attribute properties will be automatically encrypted or decrypted

                // Here all data in table 'Infos' will be encrypted using 'specificKey'
                db.InsertItem(new Info { SomeInfo = "Some Info 1", SomeValue = 12343.11212 });

                // In table 'Tasks' for encryption data will be used 'keyForDatabase' key, because we don't specify for table 'Tasks' cpecific encryption key
                db.InsertItem(new Tasks { Task = "Task1", Description = "Description_1", Price = 1234.41, IsDone = false });
                db.InsertItem(new Tasks { Task = "Task2", Description = "Description_2", Price = 1307.69, IsDone = true });
                db.InsertItem(new Tasks { Task = "Task3", Description = "Description_1", Price = null, IsDone = true });
                db.InsertItem(new Tasks { Task = "Task4", Description = "Description_2", Price = 1100.99, IsDone = false });
                db.InsertItem(new Tasks { Task = "Task5", Description = "Description_2", Price = 1718.99, IsDone = false });
                db.InsertItem(new Tasks { Task = "Task6", Description = "Description_3", Price = null, IsDone = true });

                // Infos tables. Only for JOINING TABLES example
                db.InsertItem(new Info { SomeInfo = "SomeInfo 1", SomeValue = 1233.991 });
                db.InsertItem(new Info { SomeInfo = "SomeInfo 2", SomeValue = 111.585 });
                db.InsertItem(new Info { SomeInfo = "SomeInfo 3", SomeValue = 2322 });
                db.InsertItem(new Info { SomeInfo = "SomeInfo 4", SomeValue = 0.00021 });
                db.InsertItem(new Info { SomeInfo = "SomeInfo 5", SomeValue = 4.878544 });

                // FIND ELEMENTS IN LINQ MANNER:
                var completedTasks = db.Find<Tasks>(t => t.IsDone);             // SQL Request will be: SELECT * FROM MyTasks WHERE (IsDone = 1)

                var notCompletedTasks = db.Find<Tasks>(t => !t.IsDone);         // SQL Request will be: SELECT * FROM MyTasks WHERE (IsDone = 0)
                var tasksWithoutPrice = db.Find<Tasks>(t => t.Price == null);   // SQL Request will be: SELECT * FROM MyTasks WHERE (Price IS NULL) 
                var tasksWithPrice = db.Find<Tasks>(t => t.Price != null);      // SQL Request will be: SELECT * FROM MyTasks WHERE (Price IS NOT NULL) 
                var completedTasksWithPriceLessThanValue = db.Find<Tasks>(t => t.IsDone && t.Price < 1400); // SQL Request will be: SELECT * FROM MyTasks WHERE ((IsDone = 1) AND (Price < 1400))
                var taskWithExplisitDescription = db.Find<Tasks>(t => t.Description == "Description_3");    // SQL Request will be: SELECT * FROM MyTasks WHERE (Description = 'Description_3')
                var exampleWithThreeRules = db.Find<Tasks>(t => t.IsDone && t.Price > 1000 && t.Price < 1300);  // SQL Request will be: SELECT * FROM MyTasks WHERE (((IsDone = 1) AND (Price > 1000)) AND (Price < 1300))
                
                // WE CAN SET THE "ORDER BY" COLUMN AND CHOSE ORDER TYPE: ASC/DESC
                var withOrderByColumn = db.Find<Tasks>(t => t.Price < 100, t => t.Description /*ORDER BY COLUMN*/, SortOrder.Desc);

                // WE CAN SET THE LIMIT OF RETURNED ELEMENTS
                var limitElements = db.Find<Tasks>(t => t.IsDone, 5 /*LIMIT THE NUMBER OF RETURNED RECORDS*/);

                // WE CAN SPECIFY THE "ORDER BY" COLUMN AND THE LIMIT OF RETURNED RECORDS
                var orderByAndLimitClause = db.Find<Tasks>(task => task.Price < 500, 8/*LIMIT RECORDS*/, task => task.Price /*"ORDER BY" COLUMN*/, SortOrder.Desc /*ORDER BY TYPE*/);

                // WE CAN SPECIFY WHICH COLUMNS SHOULD BE READED FROM DATABASE
                var selectedItem = db.Select<Tasks>(t => t.IsDone /*This is Predicate*/, t => t.Task /*First property*/, t => t.Price /*Second property*/);    // It will get values for only 'Task' and 'Price' properties. SQL Request will be: SELECT Task, Price FROM MyTasks WHERE (IsDone = TRUE) 
                
                // DELETE USING PREDICATES
                db.Delete<Tasks>(t => t.IsDone);    // It will removes from table all completed tasks.

                db.Delete<Tasks>(t => t.Price == null);     // Removes all tasks for which Price is not set

                db.Delete<Tasks>(t => t.Price == null || t.IsDone);

                db.Delete<Tasks>(t => t.Price < 1300 && t.Description == "Description_1");

                // WE CAN CALCULATE NUMBER OF ROWS, THAT SATISFYING TO SPECIFIC CONDITIONS
                db.Count<Tasks>();                          // returns count of records in table "Tasks"
                db.Count<Tasks>(t => t.IsDone == true);     // returns count of completed tasks
                db.Count<Tasks>("PriceForTask");            // returns count of records, that have not null value in column "PriceForTask"
                db.CountDistinct<Tasks>("PriceForTask");    // returns count of records, that have distinct values in column "PriceForTask"

                db.SelectTop<Tasks>(3);     // Returns first three elements from table Tasks

                // WE CAN JOIN UP TO FOUR TABLES:
                var joiningResult = db.Join<Tasks, Info, JoinResult>(
                    t => t.IsDone, // Filter for Tasks table. You can pass NULL, so there won't be any filter.
                    (tasks, info) => tasks.InfoId == info.Id, // Determine columns for joining tables (joining condition)
                    (tasks, info) => new JoinResult(tasks, info));   // Specifying the view of how joined tables will be returned
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                db?.Dispose(); // Here all internal copies of Encryption Key will be removed from memory (ZeroMemory).
            }
        }
    }
}