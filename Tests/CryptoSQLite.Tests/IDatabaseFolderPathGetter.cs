namespace CryptoSQLite.CrossTests
{
    public interface IDatabaseFolderPathGetter
    {
        /// <summary>
        /// Returns path to SQLite database file
        /// </summary>
        /// <param name="fileName">File name (*.db3)</param>
        /// <returns>Path to file</returns>
        string GetDatabaseFolderPath(string fileName);
    }
}
