using System;
using System.Collections.Generic;
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
        Gost28147With256BitsKey,

        /// <summary>
        /// USA encryption algoritm. It uses the 256 bit encryption key. 
        /// </summary>
        Aes256With256BitsKey
    }

    public interface ICryptoSQLite
    {
        void SetEncryptionKey(byte[] key);

        void CreateTable<TTable>() where TTable : class;

        void DeleteTable<TTable>();

        void InsertItem<TTable>(TTable item) where TTable : class;

        void InsertOrReplaceItem<TTable>(TTable item) where TTable : class;

        TTable GetItem<TTable, TVal>(string columnName, TVal columnValue) where TTable : new();

        TTable GetItem<TTable>(int id) where TTable : new();

        int DeleteItem<TTable>(TTable item);

        IQueryable<TTable> Table<TTable>() where TTable : new();
    }

    public class CryptoSQLite : IDisposable, ICryptoSQLite
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

        public CryptoSQLite(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            _internalEncryptor = new AesExternalCryptoProvider();
            _solter = new SoltGenerator();
            _tables = new Dictionary<string, TableMap>();
        }

        public CryptoSQLite(string dbFilename, CryptoAlgoritms cryptoAlgoritm)
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

        public CryptoSQLite(string dbFilename, IEncryptor encryptor)
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
                throw new Exception(
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


        public int DeleteItem<TTable>(TTable item)
        {
            CheckTable<TTable>();

            return 0;
        }

        public IQueryable<TTable> Table<TTable>() where TTable : new()
        {
            CheckTable<TTable>();

            return null;
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
                throw new Exception($"Database doesn't contain table with name: {tableName}");

            var tableFromOrmMapping = OrmUtils.GetColumnsMappingWithSqlTypes(properties.ToList());

            if (!OrmUtils.AreTablesEqual(tableFromFile, tableFromOrmMapping)) // if database doesn't contain TTable
                throw new Exception(
                    $"SQL Database doesn't contain table with column structure that specified in {tableName}");
        }

        private TableMap MapTable<TTable>(string tableName)
        {
            var type = typeof(TTable);

            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            if (properties.Where(p => p.GetColumnName() == SoltColumnName).ToArray().Any())
                throw new Exception(
                    $"Table can't contain column with name: {SoltColumnName}. This name is reserved for CryptoSQLite needs.");

            // now we must fint primary key:
            var primaryProperties = properties.Where(prop => prop.IsPrimaryKey()).ToArray();

            if (primaryProperties.Length > 1) // we can't have two or more columns specified as PrimaryKey
                throw new Exception(
                    "Crypto Table can't contain more that one PrimaryKey column. Please specify only one PrimaryKey column.");

            var hasEncryptedColumns = false;
            var encryptedColumns = properties.Find(prop => prop.IsEncrypted());
            if (encryptedColumns != null)
                hasEncryptedColumns = true;

            var tableMap = new TableMap(tableName, type, properties, hasEncryptedColumns);

            return tableMap;
        }

        private void InsertRowInTable<TTable>(TTable row, bool replaceRowIfExisits)
        {
            var columns = new List<string>();
            var values = new List<string>();

            var solt = GetSolt();

            var columnsToAdd = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            foreach (var col in columnsToAdd)
            {
                columns.Add(col.GetColumnName());
                var sqlValue = ToSqlValue(col, row, solt);
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
            var properties = OrmUtils.GetCompatibleProperties<TTable>().ToList();

            var idProperty = properties.Find(p => p.GetColumnName().ToLower() == "id");
            if (idProperty == null)
                throw new Exception(
                    $"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

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

            DecryptRow(properties, columnsFromFile, item);

            return item;
        }

        private string ToSqlValue<TTable>(PropertyInfo property, TTable row, byte[] solt)
        {
            var type = property.PropertyType;
            var isEncrypted = property.IsEncrypted();
            var value = property.GetValue(row);
            var encryptor = GetEncryptor(solt);

            if (type == typeof(string))
            {
                var str = value as string;
                if (str == null) throw new ArgumentException("Argument is not compatible with it type", nameof(value));

                var forbidden = new[] { '\'', '\"' };

                if (str.IndexOfAny(forbidden, 0) >= 0)
                    throw new Exception("String can't contain symbols like: \' and \".");

                if (!isEncrypted) return $"\'{value}\'";

                var open = Encoding.UTF8.GetBytes(str);
                var encrypted = encryptor.Encrypt(open);
                var encryptedStr = encrypted.ToSqlString();
                return $"\'{encryptedStr}\'";
            }
            if (type == typeof(DateTime))
            {
                var ticks = ((DateTime)value).Ticks;
                if (!isEncrypted) return $"{ticks}";

                var openBytes = BitConverter.GetBytes(ticks);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedTicks = BitConverter.ToInt64(encryptedBytes, 0);
                return $"{encryptedTicks}";
            }
            if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
            {
                if (!isEncrypted) return $"{value}";
                var openVal = (ulong)value;
                var openBytes = BitConverter.GetBytes(openVal);
                var encryptedBytes = encryptor.Encrypt(openBytes);
                var encryptedVal = BitConverter.ToUInt64(encryptedBytes, 0);
                return $"{encryptedVal}";
            }
            if (type == typeof(byte))
            {
                return $"{value}";
            }
            if (type == typeof(bool))
            {
                var byteVal = Convert.ToByte(value);
                return $"{byteVal}";
            }

            throw new Exception($"Type {type} is not compatible with CryptoSQLite");
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
                encryptor = GetEncryptor(solt);     // this solt was using in encryption of columns
            }
            
            foreach (var property in properties)
            {
                var column = columns.Find(c => c.Name == property.GetColumnName());
                if(column == null)
                    throw new Exception($"Can't find appropriate column in database for property: {property.GetColumnName()}");

                var type = property.PropertyType;

                if (type == typeof(string))
                {
                    var str = column.SqlValue;
                    if (property.IsEncrypted() && encryptor != null)
                    {
                        var encrypted = str.ToByteArrayFromSqlString();
                        var open = encryptor.Decrypt(encrypted);
                        str = Encoding.UTF8.GetString(open, 0, open.Length);
                    }
                    property.SetValue(item, str);
                }
                else if (type == typeof(DateTime))
                {
                    var ticks = Convert.ToInt64(column.SqlValue);
                    if (property.IsEncrypted() && encryptor != null)
                    {
                        var ticksBytes = BitConverter.GetBytes(ticks);
                        var openTicksBytes = encryptor.Decrypt(ticksBytes);
                        ticks = BitConverter.ToInt64(openTicksBytes, 0);
                    }
                    var dateTime = new DateTime(ticks);
                    property.SetValue(item, dateTime);

                }
                else if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
                type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
                {
                    var value = Convert.ToUInt64(column.SqlValue);
                    if (property.IsEncrypted() && encryptor != null)
                    {
                        var bytes = BitConverter.GetBytes(value);
                        var openBytes = encryptor.Decrypt(bytes);
                        value = BitConverter.ToUInt64(openBytes, 0);
                    }

                    if (type == typeof(short))
                    {
                        property.SetValue(item, (short)value);
                    }
                    else if (type == typeof(int))
                    {
                        property.SetValue(item, (int)value);
                    }
                    else if (type == typeof(long))
                    {
                        property.SetValue(item, (long)value);
                    }
                    else if (type == typeof(ushort))
                    {
                        property.SetValue(item, (ushort)value);
                    }
                    else if (type == typeof(uint))
                    {
                        property.SetValue(item, (uint)value);
                    }
                    else if (type == typeof(ulong))
                    {
                        property.SetValue(item, value);
                    }
                }
                else if (type == typeof(byte))
                {
                    var value = Convert.ToByte(column.SqlValue);
                    property.SetValue(item, value);
                }
                else if (type == typeof(bool))
                {
                    var value = Convert.ToBoolean(byte.Parse(column.SqlValue));
                    property.SetValue(item, value);
                }
            }
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
