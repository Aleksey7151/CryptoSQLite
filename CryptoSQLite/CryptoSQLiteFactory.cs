using System;
using System.Threading;

namespace CryptoSQLite
{
    public class CryptoSQLiteFactory
    {
        private static readonly Lazy<CryptoSQLiteFactory> Implementation =
            new Lazy<CryptoSQLiteFactory>(CreateCryptoSQLiteFactory, LazyThreadSafetyMode.PublicationOnly);

        private bool _initialized;

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
        /// <param name="dataBaseFilePath">The data base file path.</param>
        /// <returns></returns>
        public ICryptoSQLite Create(string dataBaseFilePath)
        {
            if (!_initialized)
                throw new NotImplementedException("You must call \'CryptoSQLiteFactory.Current.Init()\' method in a platform specific project, e.g. iOS or Android.");

            return new CryptoSQLite(dataBaseFilePath);
        }

        /// <summary>
        ///     Creates <see cref="ICryptoSQLite"/> database with specified full file path.
        /// </summary>
        /// <param name="dataBaseFilePath">The data base file path.</param>
        /// <param name="cryptoAlgorithm">The crypto algorithm</param>
        /// <returns></returns>
        public ICryptoSQLite Create(string dataBaseFilePath, CryptoAlgorithms cryptoAlgorithm)
        {
            if (!_initialized)
                throw new NotImplementedException("You must call \'CryptoSQLiteFactory.Current.Init()\' method in a platform specific project, e.g. iOS or Android.");

            return new CryptoSQLite(dataBaseFilePath, cryptoAlgorithm);
        }

        public void Init()
        {
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
