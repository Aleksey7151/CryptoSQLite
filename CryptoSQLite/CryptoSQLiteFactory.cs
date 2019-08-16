using System;
using System.IO;
using System.Threading;

namespace CryptoSQLite
{
    /// <summary>
    /// Represent entry point for working with crypto connection
    /// </summary>
    public class CryptoSQLiteFactory
    {
        private static readonly Lazy<CryptoSQLiteFactory> Implementation =
            new Lazy<CryptoSQLiteFactory>(CreateCryptoSQLiteFactory, LazyThreadSafetyMode.PublicationOnly);

        private bool _initialized;
        private string _dataBaseFolder;

        private CryptoSQLiteFactory()
        {
        }

        /// <summary>
        /// Current instance of <see cref="CryptoSQLiteFactory"/>
        /// </summary>
        public static CryptoSQLiteFactory Current => Implementation.Value;

        /// <summary>
        ///     Creates <see cref="ICryptoSQLite"/> database with specified full file path.
        /// </summary>
        /// <param name="dataBaseFilePath">The data base file path or file name if Init(string) method was used for initialization.</param>
        /// <returns></returns>
        public ICryptoSQLite Create(string dataBaseFilePath)
        {
            if (!_initialized)
                throw new NotImplementedException("You must call \'CryptoSQLiteFactory.Current.Init()\' method in a platform specific project, e.g. iOS or Android.");

            if (string.IsNullOrEmpty(_dataBaseFolder))
            {
                return new CryptoSQLite(dataBaseFilePath);
            }

            return new CryptoSQLite(Path.Combine(_dataBaseFolder, dataBaseFilePath));
        }

        /// <summary>
        ///     Creates <see cref="ICryptoSQLite"/> database with specified full file path.
        /// </summary>
        /// <param name="dataBaseFilePath">The data base file path or file name if Init(string) method was used for initialization.</param>
        /// <param name="cryptoAlgorithm">The crypto algorithm</param>
        /// <returns></returns>
        public ICryptoSQLite Create(string dataBaseFilePath, CryptoAlgorithms cryptoAlgorithm)
        {
            if (!_initialized)
                throw new NotImplementedException("You must call \'CryptoSQLiteFactory.Current.Init()\' method in a platform specific project, e.g. iOS or Android.");

            if (string.IsNullOrEmpty(_dataBaseFolder))
            {
                return new CryptoSQLite(dataBaseFilePath, cryptoAlgorithm);
            }

            return new CryptoSQLite(Path.Combine(_dataBaseFolder, dataBaseFilePath), cryptoAlgorithm);
        }

        /// <summary>
        /// Initializes <see cref="ICryptoSQLite"/> factory. Must be called from platform specific project e.g. Android or iOS.
        /// </summary>
        /// <exception cref="System.NotImplementedException">You must call \'Init\' method in a platform specific project, e.g. iOS or Android.</exception>
        public void Init()
        {
#if NETSTANDARD2_0
            throw new NotImplementedException("You must call \'Init\' method in a platform specific project, e.g. iOS or Android.");
#else
            SQLitePCL.Batteries_V2.Init();
            _initialized = true;
#endif
        }

        /// <summary>
        /// Initializes <see cref="ICryptoSQLite"/> factory. Must be called from platform specific project e.g. Android or iOS.
        /// If you call this method, then you do not need to provide full path to database in 'Create()' method, just file name.
        /// </summary>
        /// <param name="dataBaseFolderPath">Platform specific the data base folder path.</param>
        /// <exception cref="System.NotImplementedException">You must call \'Init\' method in a platform specific project, e.g. iOS or Android.</exception>
        public void Init(string dataBaseFolderPath)
        {
            _dataBaseFolder = dataBaseFolderPath;
#if NETSTANDARD2_0
            throw new NotImplementedException("You must call \'Init\' method in a platform specific project, e.g. iOS or Android.");
#else
            SQLitePCL.Batteries_V2.Init();
            _initialized = true;
#endif
        }

        private static CryptoSQLiteFactory CreateCryptoSQLiteFactory()
        {
            return new CryptoSQLiteFactory();
        }
    }
}
