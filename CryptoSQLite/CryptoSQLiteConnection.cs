using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite.CryptoProviders;
using CryptoSQLite.Mapping;
using SQLitePCL.pretty;


namespace CryptoSQLite
{
    public class CryptoSQLiteException : Exception
    {
        public CryptoSQLiteException(string description) : base(description)
        {

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
        Gost28147With256BitsKey,

        /// <summary>
        /// USA encryption algoritm. It uses the 256 bit encryption key. 
        /// </summary>
        Aes256With256BitsKey
    }

    public interface ICryptoSQLiteConnection
    {
        void SetEncryptionKey(byte[] key);

        void CreateTable<TTable>() where TTable : class;

        void DeleteTable<TTable>();

        void InsertItem<TTable>(TTable item) where TTable : class;

        void InsertOrReplaceItem<TTable>(TTable item) where TTable : class;

        TTable GetItem<TTable, TVal>(string columnName, TVal columnValue) where TTable : new();

        TTable GetItem<TTable>(int id) where TTable : new();

        void DeleteItem<TTable, TVal>(string columnName, TVal columnValue);

        void DeleteItem<TTable>(int id);

        void DeleteItem<TTable>(TTable item);

        IEnumerable<TTable> Table<TTable>() where TTable : new();
    }

    public interface ICryptoSQLiteAsyncConnection
    {
        void SetEncryptionKey(byte[] key);

        Task CreateTableAsync<TTable>() where TTable : class;

        Task DeleteTableAsync<TTable>();

        Task InsertItemAsync<TTable>(TTable item) where TTable : class;

        Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class;

        Task<TTable> GetItemAsync<TTable, TVal>(string columnName, TVal columnValue) where TTable : new();

        Task<TTable> GetItemAsync<TTable>(int id) where TTable : new();

        Task DeleteItemAsync<TTable>(TTable item);

        Task DeleteItemAsync<TTable, TVal>(string columnName, TVal columnValue);

        Task DeleteItemAsync<TTable>(int id);

        Task<IEnumerable<TTable>> TableAsync<TTable>() where TTable : new();
    }

    public class CryptoSQLiteAsyncConnection : ICryptoSQLiteAsyncConnection, IDisposable
    {
        private readonly ICryptoSQLiteConnection _connection;

        public CryptoSQLiteAsyncConnection(string dbFileName)
        {
            _connection = new CryptoSQLiteConnection(dbFileName);   
        }

        public CryptoSQLiteAsyncConnection(string dbFileName, CryptoAlgoritms cryptoAlgoritms)
        {
            _connection = new CryptoSQLiteConnection(dbFileName, cryptoAlgoritms);
        }

        public CryptoSQLiteAsyncConnection(string dbFileName, IEncryptor encryptor)
        {
            _connection = new CryptoSQLiteConnection(dbFileName, encryptor);
        }

        public void SetEncryptionKey(byte[] key)
        {
            _connection.SetEncryptionKey(key);
        }

        public async Task CreateTableAsync<TTable>() where TTable : class
        {
            await Task.Run(() => _connection.CreateTable<TTable>());
        }

        public async Task DeleteTableAsync<TTable>()
        {
            await Task.Run(() => _connection.DeleteTable<TTable>());
        }

        public async Task InsertItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.InsertItem(item));
        }

        public async Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.InsertOrReplaceItem(item));
        }

        public async Task<TTable> GetItemAsync<TTable, TVal>(string columnName, TVal columnValue) where TTable : new()
        {
            var table = Task.Run(() => _connection.GetItem<TTable,TVal>(columnName, columnValue));
            return await table;
        }

        public async Task<TTable> GetItemAsync<TTable>(int id) where TTable : new()
        {
            var table = Task.Run(() => _connection.GetItem<TTable>(id));
            return await table;
        }

        public async Task DeleteItemAsync<TTable>(TTable item)
        {
            await Task.Run(() => _connection.DeleteItem(item));
        }

        public async Task DeleteItemAsync<TTable, TVal>(string columnName, TVal columnValue)
        {
            await Task.Run(() => _connection.DeleteItem<TTable, TVal>(columnName, columnValue));
        }

        public async Task DeleteItemAsync<TTable>(int id)
        {
            await Task.Run(() => _connection.DeleteItem<TTable>(id));
        }

        public async Task<IEnumerable<TTable>> TableAsync<TTable>() where TTable : new()
        {
            var table = Task.Run(() => _connection.Table<TTable>());
            return await table;
        }

        public void Dispose()
        {
            (_connection as IDisposable)?.Dispose();
        }
    }

    public class CryptoSQLiteConnection : ICryptoSQLiteConnection, IDisposable
    {

        #region Private fields

        private readonly SQLiteDatabaseConnection _connection;

        private readonly IEncryptor _externalEncryptor;

        private readonly ICryptoProvider _internalEncryptor;

        private readonly ISoltGenerator _solter;

        private readonly Dictionary<string, TableMap> _tables;

        private const string SoltColumnName = "SoltColumn";

        #endregion


        #region Constructors

        public CryptoSQLiteConnection(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            _internalEncryptor = new AesExternalCryptoProvider();
            _solter = new SoltGenerator();
            _tables = new Dictionary<string, TableMap>();
        }

        public CryptoSQLiteConnection(string dbFilename, CryptoAlgoritms cryptoAlgoritm)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            switch (cryptoAlgoritm)
            {
                case CryptoAlgoritms.Aes256With256BitsKey:
                    _internalEncryptor = new AesExternalCryptoProvider();
                    break;

                case CryptoAlgoritms.Gost28147With256BitsKey:
                    _internalEncryptor = new GostExternalCryptoProvider();
                    break;

                default:
                    _internalEncryptor = new AesExternalCryptoProvider();
                    break;
            }
            _solter = new SoltGenerator();
            _tables = new Dictionary<string, TableMap>();
        }

        public CryptoSQLiteConnection(string dbFilename, IEncryptor encryptor)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            _externalEncryptor = encryptor;
            _internalEncryptor = null;
            _tables = new Dictionary<string, TableMap>();
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
                throw new CryptoSQLiteException(
                    "You are using External Crypto Provider. You can use this function only if you use internal crypto provider.");

            if (key.Length != 32)
                throw new ArgumentException("Key length must be 32 bytes");

            _internalEncryptor?.SetKey(key);
        }

        public void CreateTable<TTable>() where TTable : class
        {
            var tableName = GetTableName<TTable>();

            if (_tables.ContainsKey(tableName)) return; // table already created

            var map = MapTable<TTable>(tableName);

            var cmd = map.HasEncryptedColumns ? map.CmdCreateTable(SoltColumnName) : map.CmdCreateTable();

            _connection.Execute(cmd);

            _tables.Add(tableName, map);
        }

        public void DeleteTable<TTable>()
        {
            var tableName = GetTableName<TTable>();

            _connection.Execute(SqlCmds.CmdDeleteTable(tableName));

            if (_tables.ContainsKey(tableName))
                _tables.Remove(tableName);
        }

        public void InsertItem<TTable>(TTable item) where TTable : class
        {
            CheckTable<TTable>();
            InsertRowInTable(item, false);
        }

        public void InsertOrReplaceItem<TTable>(TTable item) where TTable : class
        {
            CheckTable<TTable>();

            InsertRowInTable(item, true);
        }

        public TTable GetItem<TTable, TVal>(string columnName, TVal columnValue) where TTable : new()
        {
            CheckTable<TTable>();

            return GetRowFromTableUsingColumnName<TTable, TVal>(columnName, columnValue);
        }

        public TTable GetItem<TTable>(int id) where TTable : new()
        {
            CheckTable<TTable>();

            return GetRowFromTableUsingId<TTable>(id);
        }

        public void DeleteItem<TTable, TVal>(string columnName, TVal columnValue)
        {
            CheckTable<TTable>();

            DeleteRowUsingColumnName<TTable>(columnName, columnValue, typeof(TVal));
        }

        public void DeleteItem<TTable>(int id)
        {
            CheckTable<TTable>();

            var properties = OrmUtils.GetCompatibleProperties<TTable>();

            var idProperty = properties.First(p => p.GetColumnName().ToLower() == "id");
            if (idProperty == null)
                throw new CryptoSQLiteException($"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

            DeleteRowUsingColumnName<TTable>(idProperty.GetColumnName(), id, typeof(int));
        }


        public void DeleteItem<TTable>(TTable item)
        {
            CheckTable<TTable>();

            var properties = OrmUtils.GetCompatibleProperties<TTable>();
            
            var notNull = properties.First(p => p.GetValue(item) != null);

            DeleteRowUsingColumnName<TTable>(notNull.GetColumnName(), notNull.GetValue(item), notNull.PropertyType);
        }

        public IEnumerable<TTable> Table<TTable>() where TTable : new()
        {
            CheckTable<TTable>();

            return GetAllTable<TTable>();
        }

        #endregion


        #region Private Functions

        private void CheckTable<TTable>()
        {
            var tableName = GetTableName<TTable>();
            if (_tables.ContainsKey(tableName)) return;
            var map = MapTable<TTable>(tableName);
            CheckTableIfTableExistsInDatabase(map.Name, map.Columns);
            _tables.Add(tableName, map);
        }

        private void CheckTableIfTableExistsInDatabase(string tableName, IEnumerable<PropertyInfo> properties)
        {
            var tableFromFile = new List<SqlColumnInfo>();
            foreach (var row in _connection.Query(SqlCmds.CmdGetTableInfo(tableName)))
            {
                var colName = row[1].ToString();
                if (!colName.Equals(SoltColumnName))
                    tableFromFile.Add(new SqlColumnInfo {Name = colName, SqlType = row[2].ToString()});
            }

            if (tableFromFile.Count == 0)
                throw new CryptoSQLiteException($"Database doesn't contain table with name: {tableName}");

            var tableFromOrmMapping = OrmUtils.GetColumnsMappingWithSqlTypes(properties.ToList());

            if (!OrmUtils.AreTablesEqual(tableFromFile, tableFromOrmMapping))       // if database doesn't contain TTable
                throw new CryptoSQLiteException($"SQL Database doesn't contain table with column structure that specified in {tableName}");
        }

        private static TableMap MapTable<TTable>(string tableName)
        {
            var type = typeof(TTable);

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            if (properties.Any(p => p.GetColumnName() == SoltColumnName))
                throw new CryptoSQLiteException(
                    $"Table can't contain column with name: {SoltColumnName}. This name is reserved for CryptoSQLite needs.");

            if (properties.Count(p => p.IsPrimaryKey()) > 1) // we can't have two or more columns specified as PrimaryKey
                throw new CryptoSQLiteException("Crypto Table can't contain more that one PrimaryKey column.");

            if (properties.Any(p=>p.IsPrimaryKey() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with PrimaryKey Attribute can't have Encrypted Attribute");

            if(properties.Any(p => p.IsAutoIncremental() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with AutoIncremental Attribute can't have Encrypted Attribute");
            
            if(properties.Any(p=>p.IsEncrypted() && (p.PropertyType == typeof(bool) || p.PropertyType == typeof(byte))))
                throw new CryptoSQLiteException("Columns that have Boolean or Byte type can't be Encrypted");

            var tableMap = new TableMap(tableName, type, properties, properties.Any(p=>p.IsEncrypted()));

            return tableMap;
        }

        private void InsertRowInTable<TTable>(TTable row, bool replaceRowIfExisits)
        {
            var columns = new List<string>();
            var values = new List<string>();
            byte[] solt = null;
            IEncryptor encryptor = null;

            var columnsToAdd = OrmUtils.GetCompatibleProperties<TTable>().ToList();
            if (columnsToAdd.Find(p => p.IsEncrypted()) != null)
            {
                solt = GetSolt();
                encryptor = GetEncryptor(solt);
            }

            foreach (var col in columnsToAdd)
            {
                if(col.IsAutoIncremental())         // if column is AutoIncremental
                    continue;

                columns.Add(col.GetColumnName());
                var value = col.GetValue(row);
                var type = col.PropertyType;
                var sqlValue = col.IsEncrypted() ? GetEncryptedValue(type, value, encryptor) : OrmUtils.GetSqlView(type, value);
                values.Add(sqlValue);
            }

            if (solt != null)
            {
                columns.Add(SoltColumnName);
                values.Add($"\'{solt.ToSqlString()}\'");
            }

            var name = GetTableName<TTable>();
            var cmd = replaceRowIfExisits
                ? SqlCmds.CmdInsertOrReplace(name, columns, values)
                : SqlCmds.CmdInsert(name, columns, values);

            _connection.Execute(cmd);
        }

        private TTable GetRowFromTableUsingColumnName<TTable, TValue>(string columnName, TValue columnValue) where TTable : new()
        {
            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToArray();
            
            var item = new TTable();

            var columnsFromFile = new List<SqlColumnInfo>();

            var cmd = SqlCmds.CmdSelect(GetTableName<TTable>(), columnName, columnValue, typeof(TValue));
            var queryable = _connection.Query(cmd);
            foreach (var row in queryable)
            {
                foreach (var column in row)
                {
                    columnsFromFile.Add(new SqlColumnInfo {Name = column.ColumnInfo.Name, SqlValue = column.ToString()});
                }
                break;
            }

            DecryptRow(properties, columnsFromFile, item);

            return item;
        }

        private TTable GetRowFromTableUsingId<TTable>(int id) where TTable : new()
        {
            var properties = OrmUtils.GetCompatibleProperties<TTable>();

            var propertieInfos = properties as PropertyInfo[] ?? properties.ToArray();
            var idProperty = propertieInfos.First(p => p.GetColumnName().ToLower() == "id");
            if (idProperty == null)
                throw new CryptoSQLiteException($"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

            var item = new TTable();

            var columnsFromFile = new List<SqlColumnInfo>();

            var cmd = SqlCmds.CmdSelect(GetTableName<TTable>(), idProperty.GetColumnName(), id, typeof(int));
            var queryable = _connection.Query(cmd);
            foreach (var row in queryable)
            {
                foreach (var column in row)
                {
                    columnsFromFile.Add(new SqlColumnInfo { Name = column.ColumnInfo.Name, SqlValue = column.ToString() });
                }
                break;
            }

            DecryptRow(propertieInfos, columnsFromFile, item);

            return item;
        }

        private void DeleteRowUsingColumnName<TTable>(string columnName, object columnValue, Type columnType)
        {
            var cmd = SqlCmds.CmdDeleteRow(GetTableName<TTable>(), columnName, columnValue, columnType);

            _connection.Execute(cmd);
        }

        private IEnumerable<TTable> GetAllTable<TTable>() where TTable : new()
        {
            var table = new List<List<SqlColumnInfo>>();
            var tableName = GetTableName<TTable>();
            var cmd = SqlCmds.CmdSelectAllTable(tableName);
            var queryable = _connection.Query(cmd);
            foreach (var row in queryable)
            {
                var columnsFromFile = new List<SqlColumnInfo>();
                foreach (var column in row)
                {
                    columnsFromFile.Add(new SqlColumnInfo { Name = column.ColumnInfo.Name, SqlValue = column.ToString() });
                }
                table.Add(columnsFromFile);
            }

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();
            var items = new List<TTable>(); 
            foreach (var row in table)
            {
                var item = new TTable();
                DecryptRow(properties, row, item);
                items.Add(item);
            }
            return items;
        }

        private string GetEncryptedValue(Type type, object value, IEncryptor encryptor)
        {
            if(encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            if (type == typeof(string))
            {
                var str = value as string;
                if (str == null)
                    throw new CryptoSQLiteException("GetEncryptedValue function. Argument is not compatible with it type");

                var open = Encoding.UTF8.GetBytes(str);
                var encrypted = encryptor.Encrypt(open);
                var encryptedStr = encrypted.ToSqlString();
                return $"\'{encryptedStr}\'";
            }
            if (type == typeof(DateTime))
            {
                var openBytes = BitConverter.GetBytes(((DateTime)value).Ticks);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedTicks = BitConverter.ToInt64(encryptedBytes, 0);
                return $"{encryptedTicks}";
            }
            if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
            {
                var openBytes = BitConverter.GetBytes((ulong)value);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedVal = BitConverter.ToUInt64(encryptedBytes, 0);
                return $"{encryptedVal}";
            }
            // one byte and bool we will not Encrypt
            if (type == typeof(byte))
                return $"{value}";
            if (type == typeof(bool))
                return $"{Convert.ToByte(value)}";
            
            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite");
        }

        private void DecryptRow<TTable>(IEnumerable<PropertyInfo> propertieInfos, IEnumerable<SqlColumnInfo> columnInfos, TTable item)
        {
            var properties = propertieInfos.ToList();
            var columns = columnInfos.ToList();

            IEncryptor encryptor = null;
            var soltColumn = columns.Find(c => c.Name == SoltColumnName);
            if (soltColumn != null)
            {
                var solt = soltColumn.SqlValue.ToByteArrayFromSqlString();
                encryptor = GetEncryptor(solt);     // this solt we were using in encryption of columns
            }
            
            foreach (var property in properties)
            {
                var column = columns.Find(c => c.Name == property.GetColumnName());
                if(column == null)
                    throw new CryptoSQLiteException($"Can't find appropriate column in database for property: {property.GetColumnName()}");

                if (property.IsEncrypted())
                    GetDecryptedValue(property, item, column.SqlValue, encryptor);
                else GetNotEncryptedValue(property, item, column.SqlValue);
            }
        }

        private void GetDecryptedValue(PropertyInfo property, object item, string sqlValue, IEncryptor encryptor)
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            var type = property.PropertyType;

            if (type == typeof(string))
            {
                var encrypted = sqlValue.ToByteArrayFromSqlString();
                var open = encryptor.Decrypt(encrypted);
                var openString = Encoding.UTF8.GetString(open, 0, open.Length);
                property.SetValue(item, openString);
            }
            else if (type == typeof(DateTime))
            {
                var ticksBytes = BitConverter.GetBytes(Convert.ToInt64(sqlValue));
                var openTicksBytes = encryptor.Decrypt(ticksBytes);
                var ticks = BitConverter.ToInt64(openTicksBytes, 0);
                var dateTime = new DateTime(ticks);
                property.SetValue(item, dateTime);
            }
            else if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
            type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
            {
                var value = Convert.ToUInt64(sqlValue);
                var bytes = BitConverter.GetBytes(value);
                var openBytes = encryptor.Decrypt(bytes);
                value = BitConverter.ToUInt64(openBytes, 0);
                property.SetValue(item, value);
            }
            else if (type == typeof(byte))
                property.SetValue(item, Convert.ToByte(sqlValue));

            else if (type == typeof(bool))
                property.SetValue(item, Convert.ToBoolean(byte.Parse(sqlValue)));
        }

        private void GetNotEncryptedValue(PropertyInfo property, object item, string sqlValue)
        {
            var type = property.PropertyType;

            if (type == typeof(string))
                property.SetValue(item, sqlValue);
            else if (type == typeof(DateTime))
                property.SetValue(item, new DateTime(Convert.ToInt64(sqlValue)));
            else if (type == typeof(short))
                property.SetValue(item, Convert.ToInt16(sqlValue));
            else if(type == typeof(ushort))
                property.SetValue(item, Convert.ToUInt16(sqlValue));
            else if(type == typeof(int))
                property.SetValue(item, Convert.ToInt32(sqlValue));
            else if(type == typeof(uint))
                property.SetValue(item, Convert.ToUInt32(sqlValue));
            else if(type == typeof(long))
                property.SetValue(item, Convert.ToInt64(sqlValue));
            else if(type == typeof(ulong))
                property.SetValue(item, Convert.ToUInt64(sqlValue));
            else if (type == typeof(byte))
                property.SetValue(item, Convert.ToByte(sqlValue));
            else if (type == typeof(bool))
                property.SetValue(item, Convert.ToBoolean(byte.Parse(sqlValue)));
        }

        private string GetTableName<TTable>()
        {
            var type = typeof(TTable);

            var cryptoTableAttributes = type.GetTypeInfo().GetCustomAttributes<CryptoTableAttribute>().ToArray();

            if (!cryptoTableAttributes.Any())
                throw new CryptoSQLiteException($"Table {typeof(TTable)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}");

            var tableAttribute = cryptoTableAttributes.First();

            if (string.IsNullOrEmpty(tableAttribute.Name))
                throw new CryptoSQLiteException("The name of Table can't be empty");

            return tableAttribute.Name;
        }

        private IEncryptor GetEncryptor(byte[] solt = null)
        {
            if (_externalEncryptor != null) return _externalEncryptor;
            
            if(solt != null)
                _internalEncryptor.SetSolt(solt);

            return _internalEncryptor;
        }

        private byte[] GetSolt()
        {
            var solt = _solter.GetSolt();
            return solt;
        }

        #endregion
    }




}
