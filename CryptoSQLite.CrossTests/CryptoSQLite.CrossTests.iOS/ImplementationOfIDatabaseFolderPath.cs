using System;
using System.IO;
using CryptoSQLite.CrossTests.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(ImplementationOfIDatabaseFolderPath))]
namespace CryptoSQLite.CrossTests.iOS
{
    public class ImplementationOfIDatabaseFolderPath : IDatabaseFolderPathGetter
    {
        public string GetDatabaseFolderPath(string fileName)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var libraryPath = Path.Combine(documentsPath, "..", "Library");                    // Library folder
            return Path.Combine(libraryPath, fileName);
        }
    }
}
