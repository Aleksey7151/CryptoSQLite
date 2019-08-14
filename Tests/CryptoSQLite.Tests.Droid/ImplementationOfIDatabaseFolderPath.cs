using System.IO;
using CryptoSQLite.CrossTests.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ImplementationOfIDatabaseFolderPath))]
namespace CryptoSQLite.CrossTests.Droid
{
    
    public class ImplementationOfIDatabaseFolderPath : IDatabaseFolderPathGetter
    {
        private readonly string _path;
        public ImplementationOfIDatabaseFolderPath()
        {
            _path = Android.OS.Environment.ExternalStorageDirectory.Path;
        }
        public string GetDatabaseFolderPath(string fileName)
        {
            return Path.Combine(_path, fileName);
        }
    }
}