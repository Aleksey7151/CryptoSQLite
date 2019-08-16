using System;
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
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder
            CryptoSQLiteFactory.Current.Init(libraryPath);

            try
            {
                var connection = CryptoSQLiteFactory.Current.Create("test_db");
                ClearTableContent(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


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