using System;
using System.IO;
using System.Linq;
using CryptoSQLite;
using CryptoSQLite.Tests.Tables;
using Foundation;
using UIKit;

namespace Blank
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);
            Window.RootViewController = new UIViewController();

            // make the window visible
            Window.MakeKeyAndVisible();

            SQLitePCL.Batteries_V2.Init();

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder

            var connection = new CryptoSQLite.CryptoSQLite(Path.Combine(libraryPath, "my_sqlite_db"));

            ClearTableContent(connection);

            return true;
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
                
                connection.ClearTable<AccountsData>();

                table = connection.Table<AccountsData>().ToArray();
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

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}


