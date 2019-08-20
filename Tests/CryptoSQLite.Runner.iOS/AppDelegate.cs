using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CryptoSQLite;
using CryptoSQLite.Tests;
using CryptoSQLite.Tests.Tables;
using Foundation;
using UIKit;
using Xunit;
using Xunit.Runner;
using Xunit.Sdk;

namespace Blank
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : RunnerAppDelegate
    {
        private readonly byte[] _key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17,
            18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder
            CryptoSQLiteFactory.Current.Init(libraryPath);

            /*
            var connection = CryptoSQLiteFactory.Current.Create("MyDbSQLite.db3");
            connection.SetEncryptionKey(_key);

            Strings_Find_Using_Equal_To_Null_Predicate(connection);
            */
            // We need this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            AddTestAssembly(typeof(BaseTest).Assembly);

#if false
			// you can use the default or set your own custom writer (e.g. save to web site and tweet it ;-)
			Writer = new TcpTextWriter ("10.0.1.2", 16384);
			// start running the test suites as soon as the application is loaded
			AutoStart = true;
			// crash the application (to ensure it's ended) and return to springboard
			TerminateAfterExecution = true;
#endif
            return base.FinishedLaunching(app, options);
        }

        private void RunUnitTests()
        {
            var tests = new FindUsingPredicateTests();
            var methods = tests.GetType().GetMethods();
            foreach (var method in methods)
            {
                if (method.GetCustomAttributes(typeof(FactAttribute), false).Length > 0)
                {
                    method.Invoke(tests, null);
                }
            }
        }

        public void Strings_Find_Using_Equal_To_Null_Predicate(ICryptoSQLite connection)
        {
            var st1 = new SecretTask { IsDone = true, Price = 99.99, Description = null, SecretToDo = "Some Secret Task" };
            var st2 = new SecretTask { IsDone = false, Price = 19.99, Description = "Description 1", SecretToDo = "Some Secret Task" };
            var st3 = new SecretTask { IsDone = true, Price = 9.99, Description = "Description 2", SecretToDo = "Some Secret Task" };
            foreach (var db in new List<ICryptoSQLite>{connection})
            {
                try
                {
                    db.DeleteTable<SecretTask>();
                    db.CreateTable<SecretTask>();

                    db.InsertItem(st1);
                    db.InsertItem(st2);
                    db.InsertItem(st3);

                    var result = db.Find<SecretTask>(a => a.Description == null);

                    var table = result.ToArray();
                    Assert.Single(table);
                    Assert.True(st1.Equal(table[0]));
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    db.Dispose();
                }
            }
        }

        public void ClearTableContent(ICryptoSQLite connection)
        {
            var account1 = new AccountsData
            {
                Id = 33,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = 174376512,
                Name = "Frodo Beggins",
                Password = "A_B_R_A_C_A_D_A_B_R_A",
                Age = 27,
                IsAdministrator = false,
                IgnoredString = "Some string that i can't will be ignored in table mapping"
            };

            var account2 = new AccountsData
            {
                Id = 66,    // will be ignored in table mapping, because it's market as autoincremental
                SocialSecureId = uint.MaxValue,
                Name = "Gendalf Gray",
                Password = "I am master of Anor flame.",
                Age = 27,
                IsAdministrator = true,
                IgnoredString = "Some string that'll be ignored in table mapping"
            };

            try
            {
                connection.DeleteTable<AccountsData>();
                connection.CreateTable<AccountsData>();

                connection.InsertItem(account1);
                connection.InsertItem(account2);

                var table = connection.Table<AccountsData>().ToArray();

                Assert.Contains(table, e => e.Equals(account1));
                Assert.Contains(table, e => e.Equals(account2));

                connection.ClearTable<AccountsData>();

                table = connection.Table<AccountsData>().ToArray();
                Assert.True(table != null);
                Assert.True(table.Length == 0);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                connection.Dispose();
            }
        }
    }
}