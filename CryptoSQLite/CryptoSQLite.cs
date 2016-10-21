using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using CryptoSQLite.CryptoProviders;
using CryptoSQLite.Mapping;
using SQLitePCL.pretty;


namespace CryptoSQLite
{
    public class CryptoException : Exception
    {
        public string Description { get; }

        public CryptoException(string description)
        {
            Description = description;
        }
    }

    /// <summary>
    /// Enumerates the crypto algoritms, that can be used for data protection
    /// </summary>
    public enum CryptoAlgoritms
    {
        /// <summary>
        /// USSR encryption algoritm. It uses the 256 bit encryption key.
        /// </summary>
        Gost28147,
        /// <summary>
        /// USA encryption algoritm. It uses the 256 bit encryption key. 
        /// </summary>
        Aes256
    }

    public interface ICryptoSQLite
    {
        void CreateTable<TTable>() where TTable : class;

        void DeleteTable<TTable>();

        int Insert<TTable>(TTable item) where TTable : class;

        int InsertOrReplace<TTable>(TTable item) where TTable : class;

        int Update<TTable>(TTable item) where TTable : class;

        int Delete<TTable>(TTable item);

        IQueryable<TTable> Table<TTable>() where TTable : new();
    }

    public class CryptoSQLite : IKeysSetter, IDisposable, ICryptoSQLite
    {
        #region Private fields

        private readonly SQLiteDatabaseConnection _connection;

        private readonly IEncryptor _externalEncryptor;

        private readonly ICryptoProvider _internalEncryptor;

        private readonly Dictionary<Type, TableMap> _tables;

        #endregion


        #region Constructors

        public CryptoSQLite(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename);
            _internalEncryptor = new AesExternalCryptoProvider();
            _tables = new Dictionary<Type, TableMap>();
        }

        public CryptoSQLite(string dbFilename, CryptoAlgoritms cryptoAlgoritm)
        {
            _connection = SQLite3.Open(dbFilename);
            switch (cryptoAlgoritm)
            {
                case CryptoAlgoritms.Aes256:
                    _internalEncryptor = new AesExternalCryptoProvider();
                    break;

                case CryptoAlgoritms.Gost28147:
                    _internalEncryptor = new GostExternalCryptoProvider();
                    break;

                default:
                    _internalEncryptor = new AesExternalCryptoProvider();
                    break;
            }
            _tables = new Dictionary<Type, TableMap>();
        }

        public CryptoSQLite(string dbFilename, IEncryptor encryptor)
        {
            _connection = SQLite3.Open(dbFilename);
            _externalEncryptor = encryptor;
            _internalEncryptor = null;
            _tables = new Dictionary<Type, TableMap>();
        }

        #endregion

        
        #region Implementation of IDispose

        public void Dispose()
        {
            _internalEncryptor?.ClearKey();
        }

        #endregion
        

        #region Implementation of IKeySetter

        public void SetKey(byte[] key)
        {
            if (_internalEncryptor == null && _externalEncryptor != null)
                throw new Exception("You are using External Crypto Provider. You can use this function only if you use internal crypto provider.");

            _internalEncryptor?.SetKey(key);
        }

        public void SetSolt(byte[] solt)
        {
            if (_internalEncryptor == null && _externalEncryptor != null)
                throw new Exception("You are using External Crypto Provider. You can use this function only if you use internal crypto provider.");

            _internalEncryptor?.SetSolt(solt);
        }

        public void ClearKey()
        {
            _internalEncryptor?.ClearKey();
        }

        #endregion


        #region Implementation of ICryptoSQLite

        public void CreateTable<TTable>() where TTable : class
        {
            MapTable<TTable>();

            TableMap map;
            _tables.TryGetValue(typeof(TTable), out map);
            var cmd = map?.CmdCreateTable();
            _connection.Execute(cmd);
        }

        public void DeleteTable<TTable>() 
        {
            var type = typeof(TTable);

            var cryptoTableAttributes = type.GetTypeInfo().GetCustomAttributes<CryptoTableAttribute>().ToArray();

            if (!cryptoTableAttributes.Any())
                throw new Exception($"Table {typeof(TTable)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}");

            var tableAttribute = cryptoTableAttributes.First();

            if (string.IsNullOrEmpty(tableAttribute.Name))
                throw new Exception("The name of Table can't be empty");

            _connection.Execute(SqlCmds.CmdDeleteTable(tableAttribute.Name));
        }

        public int Insert<TTable>(TTable item) where TTable : class
        {
            var type = typeof(TTable);

            if (!_tables.ContainsKey(type))
                MapTable<TTable>();

            return 0;
        }

        public int InsertOrReplace<TTable>(TTable item) where TTable : class 
        {
            return 0;
        }

        public int Update<TTable>(TTable item) where TTable : class 
        {
            return 0;
        }

        public int Delete<TTable>(TTable item)
        {
            return 0;
        }

        public IQueryable<TTable> Table<TTable>() where TTable : new()
        {
            return null;
        }

        #endregion
        

        #region Private Functions

        private void MapTable<TTable>()
        {
            if (_tables.ContainsKey(typeof(TTable)))
                return;

            var type = typeof(TTable);

            var cryptoTableAttributes = type.GetTypeInfo().GetCustomAttributes<CryptoTableAttribute>().ToArray();

            if (!cryptoTableAttributes.Any())
                throw new Exception($"Table {typeof(TTable)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}");

            var tableAttribute = cryptoTableAttributes.First();

            if(string.IsNullOrEmpty(tableAttribute.Name))
                throw  new Exception("The name of Table can't be empty");

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            // now we must fint primary key:
            var primaryProperties = properties.Where(prop => prop.IsPrimaryKey()).ToArray();

            if(primaryProperties.Length > 1)    // we can't have two or more columns specified as PrimaryKey
                throw new Exception("Crypto Table can't contain more that one PrimaryKey column. Please specify only one PrimaryKey column.");
            
            if(primaryProperties.Length == 0)   // Table can't be without PrimaryKey column
                throw new Exception("Crypto Table must contain at least one column specified as PrimatyKey.");

            properties.Remove(primaryProperties[0]);    // remove from all properties primary key property 

            var tableMap = new TableMap(tableAttribute.Name, type);

            var columnsMap = new List<ColumnMap> {primaryProperties[0].CreateColumnMap()};

            columnsMap.AddRange(properties.Select(property => property.CreateColumnMap()));

            tableMap.Columns = columnsMap;

            _tables.Add(type, tableMap);
        }

        #endregion
    }




}
