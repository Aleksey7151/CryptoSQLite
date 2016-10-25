using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Text;
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
        void SetEncryptionKey(byte[] key);

        void CreateTable<TTable>() where TTable : class;

        void DeleteTable<TTable>();

        int Insert<TTable>(TTable item) where TTable : class;

        int InsertOrReplace<TTable>(TTable item) where TTable : class;

        int Update<TTable>(TTable item) where TTable : class;

        int Delete<TTable>(TTable item);

        IQueryable<TTable> Table<TTable>() where TTable : new();
    }

    public class CryptoSQLite : IDisposable, ICryptoSQLite
    {
        #region Private fields

        private readonly SQLiteDatabaseConnection _connection;

        private readonly IEncryptor _externalEncryptor;

        private readonly ICryptoProvider _internalEncryptor;

        private readonly ISoltGenerator _solter;

        private readonly Dictionary<Type, TableMap> _tables;
        
        #endregion


        #region Constructors

        public CryptoSQLite(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            _internalEncryptor = new AesExternalCryptoProvider();
            _solter = new SoltGenerator();
            _tables = new Dictionary<Type, TableMap>();
        }

        public CryptoSQLite(string dbFilename, CryptoAlgoritms cryptoAlgoritm)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
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
            _solter = new SoltGenerator();
            _tables = new Dictionary<Type, TableMap>();
        }

        public CryptoSQLite(string dbFilename, IEncryptor encryptor)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            _externalEncryptor = encryptor;
            _internalEncryptor = null;
            _tables = new Dictionary<Type, TableMap>();
        }

        #endregion

        
        #region Implementation of IDispose

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #endregion



        #region Implementation of ICryptoSQLite

        public void SetEncryptionKey(byte[] key)
        {
            if (_internalEncryptor == null && _externalEncryptor != null)
                throw new Exception("You are using External Crypto Provider. You can use this function only if you use internal crypto provider.");

            if (key.Length != 32)
                throw new ArgumentException("Key length must be 32 bytes");

            _internalEncryptor?.SetKey(key);
        }

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
            var tableName = GetTableName<TTable>();

            _connection.Execute(SqlCmds.CmdDeleteTable(tableName));
        }

        public int Insert<TTable>(TTable item) where TTable : class
        {
            InsertTable(item);

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

        /// <summary>
        /// Finds table in database file and checks table mapping.
        /// This mapping must be the same as mapping <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <returns></returns>
        private bool CheckTableInDatabase<TTable>()
        {
            var tableName = GetTableName<TTable>();
            var tableFromFile = new List<SqlColumnInfo>();
            foreach (var row in _connection.Query(SqlCmds.CmdGetTableInfo(tableName)))
            {
                tableFromFile.Add(new SqlColumnInfo {Name = row[1].ToString(), SqlType = row[2].ToString()});
            }
            
            if (tableFromFile.Count == 0)
                throw new Exception($"Database doesn't contain table with name: {tableName}");

            var tableFromOrmMapping = OrmUtils.GetColumnsMappingWithSqlTypes<TTable>();

            return OrmUtils.AreTablesEqual(tableFromFile, tableFromOrmMapping);
        }

        private void MapTable<TTable>()
        {
            if (_tables.ContainsKey(typeof(TTable)))
                return;

            var tableName = GetTableName<TTable>();

            var type = typeof(TTable);

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            // now we must fint primary key:
            var primaryProperties = properties.Where(prop => prop.IsPrimaryKey()).ToArray();

            if(primaryProperties.Length > 1)    // we can't have two or more columns specified as PrimaryKey
                throw new Exception("Crypto Table can't contain more that one PrimaryKey column. Please specify only one PrimaryKey column.");
            
            var tableMap = new TableMap(tableName, type);

            var columnsMap = new List<ColumnMap>(); 

            columnsMap.AddRange(properties.Select(property => property.CreateColumnMap()));

            columnsMap.Add(OrmUtils.CreateSoltColumn());    // this column will be contain solt for crypto algoritms

            tableMap.Columns = columnsMap;

            _tables.Add(type, tableMap);
        }

        private void InsertTable<TTable>(TTable row)
        {
            var type = typeof(TTable);
            if (!_tables.ContainsKey(type))             // if existance of TTable in database file is already not checked
            {
                if (!CheckTableInDatabase<TTable>())    // if database doesn't contain TTable
                    throw new Exception($"SQL Database doesn't contain table with column structure specified in {typeof(TTable)}");

                //            MapTable<TTable>();
            }

            var columns = new List<string>();
            var values = new List<string>();

            var solt = UpdateSolt();
            var encryptor = GetEncryptor();

            var insertColumns = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            foreach (var col in insertColumns)
            {
                columns.Add(col.GetColumnName());
                var value = col.GetValue(row);
                var sqlValue = OrmUtils.PrepareValueForSql(col.PropertyType, value, col.IsEncrypted(), encryptor);
                values.Add(sqlValue);
            }

            if (solt != null)
            {
                columns.Add(ColumnMap.SoltColumnName);
                values.Add($"{BitConverter.ToUInt64(solt, 0)}");
            }

            var cmd = SqlCmds.CmdInsertOrReplace(GetTableName<TTable>(), columns, values);

            _connection.Execute(cmd);
        }

        private string GetTableName<TTable>()
        {
            var type = typeof(TTable);

            var cryptoTableAttributes = type.GetTypeInfo().GetCustomAttributes<CryptoTableAttribute>().ToArray();

            if (!cryptoTableAttributes.Any())
                throw new Exception($"Table {typeof(TTable)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}");

            var tableAttribute = cryptoTableAttributes.First();

            if (string.IsNullOrEmpty(tableAttribute.Name))
                throw new Exception("The name of Table can't be empty");

            return tableAttribute.Name;
        }

        private IEncryptor GetEncryptor()
        {
            return _externalEncryptor ?? _internalEncryptor;
        }

        private byte[] UpdateSolt()
        {
            if (_internalEncryptor == null)
                return null;

            var solt = _solter.GetSolt();
            _internalEncryptor.SetSolt(solt);
            return solt;
        }


        #endregion
    }




}
