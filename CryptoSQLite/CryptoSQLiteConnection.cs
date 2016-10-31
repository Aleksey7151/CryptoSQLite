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
        public string ProbableCause { get; }

        public CryptoSQLiteException(string message, string cause = null) : base(message)
        {
            ProbableCause = cause;
        }
    }

    /// <summary>
    /// Enumerates the crypto algoritms, that can be used for data protection in CryptoSQLite library
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
        AesWith256BitsKey
    }

    public interface ICryptoSQLiteConnection
    {
        /// <summary>
        /// Sets the encryption key, that will be use in encryption algoritms for data encryption.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Creates a new table in database, that can contain encrypted columns.
        /// 
        /// Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing one more column: "SoltColumn". This column 
        /// is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// 
        /// Warning! If you insert element in the table, and then change Properties order in table type, you won't be able
        /// to decrypt data too. Properties order in table type is importent thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void CreateTable<TTable>() where TTable : class;

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteTable<TTable>();

        /// <summary>
        /// Inserts new element (row) in table.
        /// The table must be already created.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void InsertItem<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void InsertOrReplaceItem<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <typeparam name="TVal">Type of column value.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        TTable GetItem<TTable, TVal>(string columnName, TVal columnValue) where TTable : new();

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        TTable GetItem<TTable>(int id) where TTable : new();

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        TTable GetItem<TTable>(TTable item) where TTable : new();

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <typeparam name="TVal">Type of column value.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable, TVal>(string columnName, TVal columnValue);

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable>(int id);

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable>(TTable item);

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        IEnumerable<TTable> Table<TTable>() where TTable : new();
    }

    public interface ICryptoSQLiteAsyncConnection
    {
        /// <summary>
        /// Sets the encryption key, that will be use in encryption algoritms for data encryption.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Creates a new table in database, that can contain encrypted columns.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task CreateTableAsync<TTable>() where TTable : class;

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteTableAsync<TTable>();

        /// <summary>
        /// Inserts new element (row) in table.
        /// The table must be already created.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task InsertItemAsync<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <typeparam name="TVal">Type of column value.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task<TTable> GetItemAsync<TTable, TVal>(string columnName, TVal columnValue) where TTable : new();

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task<TTable> GetItemAsync<TTable>(int id) where TTable : new();

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task<TTable> GetItemAsync<TTable>(TTable item) where TTable : new();

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable>(TTable item);

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <typeparam name="TVal">Type of column value.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable, TVal>(string columnName, TVal columnValue);

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable>(int id);

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
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
            var table = Task.Run(() => _connection.GetItem<TTable, TVal>(columnName, columnValue));
            return await table;
        }

        public async Task<TTable> GetItemAsync<TTable>(int id) where TTable : new()
        {
            var table = Task.Run(() => _connection.GetItem<TTable>(id));
            return await table;
        }

        public async Task<TTable> GetItemAsync<TTable>(TTable item) where TTable : new()
        {
            var table = Task.Run(() => _connection.GetItem(item));
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
                case CryptoAlgoritms.AesWith256BitsKey:
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

        #endregion


        #region Implementation of IDispose

        public void Dispose()
        {
            _tables.Clear();
            _connection?.Dispose();
        }

        #endregion


        #region Implementation of ICryptoSQLite

        public void SetEncryptionKey(byte[] key)
        {
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

            try
            {
                _connection.Execute(cmd);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, "Apparently table name or names of columns contain forbidden symbols.");
            }
            _tables.Add(tableName, map);
        }

        public void DeleteTable<TTable>()
        {
            var tableName = GetTableName<TTable>();

            try
            {
                _connection.Execute(SqlCmds.CmdDeleteTable(tableName));
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, "Apparently name of table contains forbidden symbols.");
            }
            

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

            return GetRowFromTableUsingColumnName<TTable>(columnName, columnValue, typeof(TVal));
        }

        public TTable GetItem<TTable>(int id) where TTable : new()
        {
            CheckTable<TTable>();

            return GetRowFromTableUsingId<TTable>(id);
        }

        public TTable GetItem<TTable>(TTable item) where TTable : new()
        {
            CheckTable<TTable>();

            var properties = OrmUtils.GetCompatibleProperties<TTable>();

            var notNull = properties.First(p => p.GetValue(item) != null);

            return GetRowFromTableUsingColumnName<TTable>(notNull.GetColumnName(), notNull.GetValue(item),
                notNull.PropertyType);
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
                throw new CryptoSQLiteException(
                    $"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

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

        /// <summary>
        /// Checks if table <typeparamref name="TTable"/> has correct structure of columns.
        /// And adds type of <typeparamref name="TTable"/> to list of registered tables.
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        private void CheckTable<TTable>()
        {
            var tableName = GetTableName<TTable>();
            if (_tables.ContainsKey(tableName)) return;
            var map = MapTable<TTable>(tableName);
            CheckIfTableExistsInDatabase(map.Name, map.Columns);
            _tables.Add(tableName, map);
        }

        /// <summary>
        /// Checks if table with corresponds columns exists in database
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="properties">Set of columns, that table should contain</param>
        private void CheckIfTableExistsInDatabase(string tableName, IEnumerable<PropertyInfo> properties)
        {
            var tableFromFile = new List<SqlColumnInfo>();

            try
            {
                foreach (var row in _connection.Query(SqlCmds.CmdGetTableInfo(tableName)))
                {
                    var colName = row[1].ToString();
                    if (!colName.Equals(SoltColumnName))
                        tableFromFile.Add(new SqlColumnInfo {Name = colName, SqlType = row[2].ToString()});
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    $" Can't get info about table {tableName}. Aparently table doesn't exist in databale.");
            }

            if (tableFromFile.Count == 0)
                throw new CryptoSQLiteException($"Database doesn't contain table with name: {tableName}.");

            var tableFromOrmMapping = OrmUtils.GetColumnsMappingWithSqlTypes(properties.ToList());

            if (!OrmUtils.AreTablesEqual(tableFromFile, tableFromOrmMapping)) // if database doesn't contain TTable
                throw new CryptoSQLiteException(
                    $"SQL Database doesn't contain table with column structure that specified in {tableName}.");
        }

        /// <summary>
        /// Creates map of table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table for mapping</typeparam>
        /// <param name="tableName">Table name for mapping</param>
        /// <returns></returns>
        private static TableMap MapTable<TTable>(string tableName)
        {
            var type = typeof(TTable);

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            if (properties.Any(p => p.GetColumnName() == SoltColumnName))
                throw new CryptoSQLiteException(
                    $"Table can't contain column with name: {SoltColumnName}. This name is reserved for CryptoSQLite needs.");

            if (properties.Count(p => p.IsPrimaryKey()) > 1)
                // we can't have two or more columns specified as PrimaryKey
                throw new CryptoSQLiteException("Crypto Table can't contain more that one PrimaryKey column.");

            if (properties.Any(p => p.IsPrimaryKey() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with PrimaryKey Attribute can't have Encrypted Attribute");

            if (properties.Any(p => p.IsAutoIncremental() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with AutoIncremental Attribute can't have Encrypted Attribute");

            if (
                properties.Any(
                    p => p.IsEncrypted() && (p.PropertyType == typeof(bool) || p.PropertyType == typeof(byte))))
                throw new CryptoSQLiteException("Columns that have Boolean or Byte type can't be Encrypted");

            var tableMap = new TableMap(tableName, type, properties, properties.Any(p => p.IsEncrypted()));

            return tableMap;
        }

        /// <summary>
        /// Inserts row in table
        /// </summary>
        /// <typeparam name="TTable">Type of table</typeparam>
        /// <param name="row">row for insert</param>
        /// <param name="replaceRowIfExisits">flag if need replace existing row</param>
        private void InsertRowInTable<TTable>(TTable row, bool replaceRowIfExisits)
        {
            var columns = new List<string>();
            var values = new List<string>();
            byte[] solt = null;
            ICryptoProvider encryptor = null;

            var columnsToAdd = OrmUtils.GetCompatibleProperties<TTable>().ToList();
            if (columnsToAdd.Find(p => p.IsEncrypted()) != null)
            {
                solt = GetSolt();
                encryptor = GetEncryptor(solt);
            }

            foreach (var col in columnsToAdd)
            {
                if (col.IsAutoIncremental()) // if column is AutoIncremental
                    continue;

                columns.Add(col.GetColumnName());
                var value = col.GetValue(row);
                var type = col.PropertyType;
                var sqlValue = col.IsEncrypted()
                    ? GetEncryptedValue(type, value, encryptor)
                    : OrmUtils.GetSqlView(type, value);
                values.Add(sqlValue);
            }

            if (solt != null)
            {
                columns.Add(SoltColumnName);
                values.Add($"\'{solt.ToSqlString()}\'");
            }

            var name = GetTableName<TTable>();
            if (replaceRowIfExisits)
            {
                var cmd = SqlCmds.CmdInsertOrReplace(name, columns, values);
                try
                {
                    _connection.Execute(cmd);
                }
                catch (Exception ex)
                {
                    throw new CryptoSQLiteException(ex.Message, "Apparently table doesn't exist in database.");
                }
            }
            else
            {
                var cmd = SqlCmds.CmdInsert(name, columns, values);
                try
                {
                    _connection.Execute(cmd);
                }
                catch (Exception ex)
                {
                    throw new CryptoSQLiteException(ex.Message, "Apparently new element is already exists in table");
                }
            }
        }

        private TTable GetRowFromTableUsingColumnName<TTable>(string columnName, object columnValue, Type columnType)
            where TTable : new()
        {
            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToArray();

            var item = new TTable();

            var columnsFromFile = new List<SqlColumnInfo>();

            var cmd = SqlCmds.CmdSelect(GetTableName<TTable>(), columnName, columnValue, columnType);

            try
            {
                var queryable = _connection.Query(cmd);
                foreach (var row in queryable)
                {
                    columnsFromFile.AddRange(
                        row.Select(
                            column => new SqlColumnInfo {Name = column.ColumnInfo.Name, SqlValue = column.ToString()}));
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, "Apparantly column name is invalid or contains forbidden symbols");
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
                throw new CryptoSQLiteException(
                    $"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

            var item = new TTable();

            var columnsFromFile = new List<SqlColumnInfo>();

            var tableName = GetTableName<TTable>();
            var cmd = SqlCmds.CmdSelect(tableName, idProperty.GetColumnName(), id, typeof(int));
            try
            {
                var queryable = _connection.Query(cmd);
                foreach (var row in queryable)
                {
                    columnsFromFile.AddRange(
                        row.Select(
                            column => new SqlColumnInfo {Name = column.ColumnInfo.Name, SqlValue = column.ToString()}));
                    break;
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    $"Apparently column with name \"id\" doesn't exists in table {tableName}.");
            }


            DecryptRow(propertieInfos, columnsFromFile, item);

            return item;
        }

        private void DeleteRowUsingColumnName<TTable>(string columnName, object columnValue, Type columnType)
        {
            var tableName = GetTableName<TTable>();
            var cmd = SqlCmds.CmdDeleteRow(tableName, columnName, columnValue, columnType);

            try
            {
                _connection.Execute(cmd);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, $"Apparently column with name {columnName} doesn't exist in table {tableName}.");
            }

        }

        private IEnumerable<TTable> GetAllTable<TTable>() where TTable : new()
        {
            var table = new List<List<SqlColumnInfo>>();
            var tableName = GetTableName<TTable>();
            var cmd = SqlCmds.CmdSelectAllTable(tableName);

            try
            {
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
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, $"Apparantly table with name {tableName} doesn't exist in database file.");
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

        private static string GetEncryptedValue(Type type, object value, ICryptoProvider encryptor)
        {
            if(encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            if (type == typeof(string))
            {
                var str = value as string;
                if (str == null)
                    throw new CryptoSQLiteException("GetEncryptedValue function. Argument is not compatible with it type");

                var data = Encoding.UTF8.GetBytes(str);
                encryptor.XorGamma(data);
                var encryptedStr = data.ToSqlString();
                return $"\'{encryptedStr}\'";
            }
            if (type == typeof(DateTime))
            {
                var data = BitConverter.GetBytes(((DateTime)value).Ticks);
                encryptor.XorGamma(data);
                var encryptedTicks = BitConverter.ToInt64(data, 0);
                return $"{encryptedTicks}";
            }
            if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
            {
                var data = BitConverter.GetBytes((ulong)value);
                encryptor.XorGamma(data);
                var encryptedVal = BitConverter.ToUInt64(data, 0);
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

            ICryptoProvider encryptor = null;
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

        private static void GetDecryptedValue(PropertyInfo property, object item, string sqlValue, ICryptoProvider encryptor)
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            var type = property.PropertyType;

            if (type == typeof(string))
            {
                var data = sqlValue.ToByteArrayFromSqlString();
                encryptor.XorGamma(data);
                var openString = Encoding.UTF8.GetString(data, 0, data.Length);
                property.SetValue(item, openString);
            }
            else if (type == typeof(DateTime))
            {
                var data = BitConverter.GetBytes(Convert.ToInt64(sqlValue));
                encryptor.XorGamma(data);
                var ticks = BitConverter.ToInt64(data, 0);
                var dateTime = new DateTime(ticks);
                property.SetValue(item, dateTime);
            }
            else if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
            type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
            {
                var value = Convert.ToUInt64(sqlValue);
                var data = BitConverter.GetBytes(value);
                encryptor.XorGamma(data);
                value = BitConverter.ToUInt64(data, 0);
                property.SetValue(item, value);
            }
            else if (type == typeof(byte))
                property.SetValue(item, Convert.ToByte(sqlValue));

            else if (type == typeof(bool))
                property.SetValue(item, Convert.ToBoolean(byte.Parse(sqlValue)));
        }

        private static void GetNotEncryptedValue(PropertyInfo property, object item, string sqlValue)
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

        private static string GetTableName<TTable>()
        {
            var type = typeof(TTable);

            var cryptoTableAttributes = type.GetTypeInfo().GetCustomAttributes<CryptoTableAttribute>().ToArray();

            if (!cryptoTableAttributes.Any())
                throw new CryptoSQLiteException($"Table {typeof(TTable)} doesn't have Custom Attribute: {nameof(CryptoTableAttribute)}");

            var tableAttribute = cryptoTableAttributes.First();

            if (string.IsNullOrEmpty(tableAttribute.TableName))
                throw new CryptoSQLiteException("The name of Table can't be empty");

            return tableAttribute.TableName;
        }

        private ICryptoProvider GetEncryptor(byte[] solt = null)
        {
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
