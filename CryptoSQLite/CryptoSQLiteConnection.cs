using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite.CryptoProviders;
using CryptoSQLite.Extensions;
using CryptoSQLite.Mapping;
using SQLitePCL.pretty;


namespace CryptoSQLite
{
    /// <summary>
    /// Class represents CryptoSQLite Exception.
    /// </summary>
    public class CryptoSQLiteException : Exception
    {
        /// <summary>
        /// Propable cause of exception.
        /// </summary>
        public string ProbableCause { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message with the reason.</param>
        /// <param name="cause">Probable cause of exception.</param>
        public CryptoSQLiteException(string message, string cause = null) : base(message)
        {
            ProbableCause = cause;
        }
    }

    /// <summary>
    /// Enumerates crypto algorithms, that can be used for data encryption in CryptoSQLite library
    /// </summary>
    public enum CryptoAlgoritms
    {
        /// <summary>
        /// USSR encryption algorithm. It uses the 256 bit encryption key.
        /// <para/>This algorithm is reliable. Many exsoviet union countries use this algorithm for secret data protection.
        /// </summary>
        Gost28147With256BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 256 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith256BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 192 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith192BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 128 bit encryption key. 
        /// <para/>This algorithm is reliable.
        /// </summary>
        AesWith128BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 56 bit encryption key.
        /// <para/>This algorithm is Very Fast, but not reliable.
        /// </summary>
        DesWith56BitsKey,

        /// <summary>
        /// USA encryption algorithm. It uses the 168 bit encryption key.
        /// <para/>This algorithm is Fast and more reliable than DES, but not so fast as DES.
        /// </summary>
        TripleDesWith168BitsKey
    }

    /// <summary>
    /// FTS3 and FTS4 are SQLite virtual table modules that allows users to perform full-text searches.
    /// <para/>The FTS3 and FTS4 extension modules allows users to create special tables with a built-in full-text index (hereafter "FTS tables").
    /// <para/>The full-text index allows the user to efficiently query the database for all rows that contain one or more words (hereafter "tokens"), 
    /// even if the table contains many large documents.
    /// </summary>
    [Flags]
    internal enum FullTextSearchFlags
    {
        /// <summary>
        /// Create ordinary table
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Create virtual table using FTS3.
        /// <para/>Attention, for newer applications, FTS4 is recommended.
        /// </summary>
        FTS3 = 0x0001,

        /// <summary>
        /// Create virtual table using FTS4.
        /// <para/>For newer applications, FTS4 is recommended.
        /// <para/>FTS4 contains query performance optimizations that may significantly improve the performance of full-text queries that contain terms that are very common (present in a large percentage of table rows).
        /// </summary>
        FTS4 = 0x0002
    }

    /// <summary>
    /// Interface of SQLite connection to database file with data encryption
    /// </summary>
    public interface ICryptoSQLiteConnection
    {
        /// <summary>
        /// Sets the encryption key for all tables in database file. That key will be used for encryption data for all tables, that don't have specific encryption key.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Sets the specific encryption key for specific table. This key will be used for encryption data only for specified table: <typeparamref name="TTable"></typeparamref>.
        /// That allows you to set up different encryption keys for different tables.
        /// This feature significantly increases data security.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <typeparam name="TTable">Table for which Encryption Key will be set</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey<TTable>(byte[] key) where TTable : class;

        /// <summary>
        /// Creates a new ordinary table (if it not already exists) in database, that can contain encrypted columns.
        /// <para/>Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing automatically generated column with name: "SoltColumn". 
        /// <para/>SoltColumn is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// <para/>Warning! If you insert element in the table, and then change Properties order in table type (in your class),
        /// you won't be able to decrypt elements. Properties order in table is important thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void CreateTable<TTable>() where TTable : class;

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteTable<TTable>() where TTable : class;

        /// <summary>
        /// Deletes all data inside the table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        void ClearTable<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <returns></returns>
        int Count<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table that satisfying the condition defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="predicate">Condition</param>
        /// <returns></returns>
        int Count<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the number of values (NULL values won't be counted) for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        int Count<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the number of distinct values for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        int CountDistinct<TTable>(string columnName) where TTable : class;

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
        /// <para/>If you insert element with setted PrimaryKey value this element will replace element in database, that has same PrimaryKey value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void InsertOrReplaceItem<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Removes from table <typeparamref name="TTable"/> all elements which column values satisfy conditions defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table in which elements will be removed</typeparam>
        /// <param name="predicate">condition for column values</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        void Delete<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void Delete<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete(
             "This method is deprecated and soon will be removed. Prealse use 'Delete(string columnName, object columnValue)' method instead.",
             false)]
        void DeleteItem<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        IEnumerable<TTable> Table<TTable>() where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> contain value == <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all rows which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        //[Obsolete("This method is deprecated and soon will be removed. Use LINQ Where<T>(Predicate<T> p) method instead.", false)]
        IEnumerable<TTable> FindByValue<TTable>(string columnName, object columnValue) where TTable : class, new();

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Property names whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Select<TTable>(Expression<Predicate<TTable>> predicate, params string[] selectedProperties)
            where TTable : class, new();

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        IEnumerable<TTable> SelectTop<TTable>(int count) where TTable : class, new();
    }

    /// <summary>
    /// Interface of SQLite Async connection to database file with data encryption
    /// </summary>
    public interface ICryptoSQLiteAsyncConnection
    {
        /// <summary>
        /// Sets the encryption key for all tables in database file. That key will be used for encryption data for all tables, that don't have specific encryption key.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Sets the specific encryption key for specific table. This key will be used for encryption data only for specified table: <typeparamref name="TTable"></typeparamref>.
        /// That allows you to set up different encryption keys for different tables.
        /// This feature significantly increases data security.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <typeparam name="TTable">Table for which Encryption Key will be set</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey<TTable>(byte[] key) where TTable : class;

        /// <summary>
        /// Creates a new ordinary table (if it not already exists) in database, that can contain encrypted columns.
        /// <para/>Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing automatically generated column with name: "SoltColumn". 
        /// <para/>SoltColumn is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// <para/>Warning! If you insert element in the table, and then change Properties order in table type (in your class),
        /// you won't be able to decrypt elements. Properties order in table is important thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task CreateTableAsync<TTable>() where TTable : class;

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteTableAsync<TTable>() where TTable : class;

        /// <summary>
        /// Deletes all data inside the table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        Task ClearTableAsync<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <returns></returns>
        Task<int> CountAsync<TTable>() where TTable : class;

        /// <summary>
        /// Returns the number of records in table that satisfying the condition defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="predicate">Condition</param>
        /// <returns></returns>
        Task<int> CountAsync<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the number of values (NULL values won't be counted) for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        Task<int> CountAsync<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the number of distinct values for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        Task<int> CountDistinctAsync<TTable>(string columnName) where TTable : class;

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
        /// <para/>If you insert element with setted PrimaryKey value this element will replace element in database, that has same PrimaryKey value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Removes from table <typeparamref name="TTable"/> all elements which column values satisfy conditions defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table in which elements will be removed</typeparam>
        /// <param name="predicate">condition for column values</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        Task DeleteAsync<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteAsync<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete(
             "This method is deprecated and soon will be removed. Prealse use 'DeleteAsync(string columnName, object columnValue)' method instead.",
             false)]
        Task DeleteItemAsync<TTable>(string columnName, object columnValue) where TTable : class;



        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        Task<IEnumerable<TTable>> TableAsync<TTable>() where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> FindAsync<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> contain value == <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all rows which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        Task<IEnumerable<TTable>> FindByValueAsync<TTable>(string columnName, object columnValue)
            where TTable : class, new();

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Property names whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> SelectAsync<TTable>(Expression<Predicate<TTable>> predicate,
            params string[] selectedProperties) where TTable : class, new();

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        Task<IEnumerable<TTable>> SelectTopAsync<TTable>(int count) where TTable : class, new();

    }

    /// <summary>
    /// Represents SQLite async connection to database file
    /// </summary>
    public class CryptoSQLiteAsyncConnection : ICryptoSQLiteAsyncConnection, IDisposable
    {
        private readonly ICryptoSQLiteConnection _connection;

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFileName">Path to SQLite database file</param>
        public CryptoSQLiteAsyncConnection(string dbFileName)
        {
            _connection = new CryptoSQLiteConnection(dbFileName);
        }

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFileName">Path to database file</param>
        /// <param name="cryptoAlgoritms">Type of crypto algorithm that will be used for data encryption</param>
        public CryptoSQLiteAsyncConnection(string dbFileName, CryptoAlgoritms cryptoAlgoritms)
        {
            _connection = new CryptoSQLiteConnection(dbFileName, cryptoAlgoritms);
        }

        /// <summary>
        /// Sets the encryption key for all tables in database file. That key will be used for encryption data for all tables, that don't have specific encryption key.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey(byte[] key)
        {
            _connection.SetEncryptionKey(key);
        }

        /// <summary>
        /// Sets the specific encryption key for specific table. This key will be used for encryption data only for specified table: <typeparamref name="TTable"></typeparamref>.
        /// That allows you to set up different encryption keys for different tables.
        /// This feature significantly increases data security.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <typeparam name="TTable">Table for which Encryption Key will be set</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey<TTable>(byte[] key) where TTable : class
        {
            _connection.SetEncryptionKey<TTable>(key);
        }

        /// <summary>
        /// Creates a new ordinary table (if it not already exists) in database, that can contain encrypted columns.
        /// <para/>Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing automatically generated column with name: "SoltColumn". 
        /// <para/>SoltColumn is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// <para/>Warning! If you insert element in the table, and then change Properties order in table type (in your class),
        /// you won't be able to decrypt elements. Properties order in table is important thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task CreateTableAsync<TTable>() where TTable : class
        {
            await Task.Run(() => _connection.CreateTable<TTable>());
        }

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task DeleteTableAsync<TTable>() where TTable : class
        {
            await Task.Run(() => _connection.DeleteTable<TTable>());
        }

        /// <summary>
        /// Deletes all data inside the table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        public async Task ClearTableAsync<TTable>() where TTable : class
        {
            await Task.Run(() => _connection.CreateTable<TTable>());
        }

        /// <summary>
        /// Returns the number of records in table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <returns></returns>
        public async Task<int> CountAsync<TTable>() where TTable : class
        {
            return await Task.Run(() => _connection.Count<TTable>());
        }

        /// <summary>
        /// Returns the number of records in table that satisfying the condition defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="predicate">Condition</param>
        /// <returns></returns>
        public async Task<int> CountAsync<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class
        {
            return await Task.Run(() => _connection.Count(predicate));
        }

        /// <summary>
        /// Returns the number of values (NULL values won't be counted) for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        public async Task<int> CountAsync<TTable>(string columnName) where TTable : class
        {
            return await Task.Run(() => _connection.Count<TTable>(columnName));
        }

        /// <summary>
        /// Returns the number of distinct values for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        public async Task<int> CountDistinctAsync<TTable>(string columnName) where TTable : class
        {
            return await Task.Run(() => _connection.CountDistinct<TTable>(columnName));
        }

        /// <summary>
        /// Inserts new element (row) in table.
        /// The table must be already created.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task InsertItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.InsertItem(item));
        }

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// <para/>If you insert element with specified PrimaryKey value this element will replace element in database, that has the same value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.InsertOrReplaceItem(item));
        }

        /// <summary>
        /// Removes from table <typeparamref name="TTable"/> all elements which column values satisfy conditions defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table in which elements will be removed</typeparam>
        /// <param name="predicate">condition for column values</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task DeleteAsync<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class
        {
            await Task.Run(() => _connection.Delete(predicate));
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task DeleteAsync<TTable>(string columnName, object columnValue) where TTable : class
        {
            await Task.Run(() => _connection.Delete<TTable>(columnName, columnValue));
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task DeleteItemAsync<TTable>(string columnName, object columnValue) where TTable : class
        {
            await Task.Run(() => _connection.DeleteItem<TTable>(columnName, columnValue));
        }

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        public async Task<IEnumerable<TTable>> TableAsync<TTable>() where TTable : class, new()
        {
            var table = await Task.Run(() => _connection.Table<TTable>());
            return table;
        }

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public async Task<IEnumerable<TTable>> FindAsync<TTable>(Expression<Predicate<TTable>> predicate)
            where TTable : class, new()
        {
            var elements = await Task.Run(() => _connection.Find(predicate));
            return elements;
        }

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> value equal to<paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all elements which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        public async Task<IEnumerable<TTable>> FindByValueAsync<TTable>(string columnName, object columnValue)
            where TTable : class, new()
        {
            var elements = await Task.Run(() => _connection.FindByValue<TTable>(columnName, columnValue));
            return elements;
        }

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Property names whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public async Task<IEnumerable<TTable>> SelectAsync<TTable>(Expression<Predicate<TTable>> predicate,
            params string[] selectedProperties) where TTable : class, new()
        {
            var elements = await Task.Run(() => _connection.Select(predicate, selectedProperties));
            return elements;
        }

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        public async Task<IEnumerable<TTable>> SelectTopAsync<TTable>(int count) where TTable : class, new()
        {
            return await Task.Run(() => _connection.SelectTop<TTable>(count));
        }



        /// <summary>
        /// Closes connection to SQLite database file. And cleans internal copies of encryption keys.
        /// !! WARNING !! You must clean encryption key, that you have set using 'SetEncryptionKey()'
        /// function, on your own. 
        /// </summary>
        public void Dispose()
        {
            (_connection as IDisposable)?.Dispose();
        }
    }


    /// <summary>
    /// Represents a connection to the SQLite database file.
    /// </summary>
    public class CryptoSQLiteConnection : ICryptoSQLiteConnection, IDisposable
    {
        #region Private fields

        /// <summary>
        /// Represents connection to database file
        /// </summary>
        private readonly SQLiteDatabaseConnection _connection;

        /// <summary>
        /// Encrypts/Decrypts data
        /// </summary>
        private readonly ICryptoProvider _cryptor;

        /// <summary>
        /// Generates pseudo random data
        /// </summary>
        private readonly ISoltGenerator _solter;

        /// <summary>
        /// Translates predicate expression to SQL request
        /// </summary>
        private readonly PredicateTranslator _predicateTranslator;

        /// <summary>
        /// Encryption algorithm that is used for data protection
        /// </summary>
        private readonly CryptoAlgoritms _algorithm;

        /// <summary>
        /// Default encryption key, this key is used for all tables which don't have specific encryption key
        /// </summary>
        private byte[] _defaultKey;

        /// <summary>
        /// Solt column name. This column contains sychronization data for encryption algorithm.
        /// </summary>
        internal const string SoltColumnName = "SoltColumn";

        /// <summary>
        /// Contains information about all checked and registered tables
        /// </summary>
        private readonly Dictionary<Type, TableMap> _tables;

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFilename">Path to SQLite database file.</param>
        public CryptoSQLiteConnection(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);

            _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes256);
            _algorithm = CryptoAlgoritms.AesWith256BitsKey;
            _solter = new SoltGenerator();
            _predicateTranslator = new PredicateTranslator();
            _tables = new Dictionary<Type, TableMap>();
        }

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFilename">Path to database file</param>
        /// <param name="cryptoAlgorithm">Type of crypto algorithm that will be used for data encryption</param>
        public CryptoSQLiteConnection(string dbFilename, CryptoAlgoritms cryptoAlgorithm)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);

            switch (cryptoAlgorithm)
            {
                case CryptoAlgoritms.AesWith256BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes256);
                    break;

                case CryptoAlgoritms.AesWith192BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes192);
                    break;

                case CryptoAlgoritms.AesWith128BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes128);
                    break;

                case CryptoAlgoritms.Gost28147With256BitsKey:
                    _cryptor = new GostCryptoProvider();
                    break;

                case CryptoAlgoritms.DesWith56BitsKey:
                    _cryptor = new DesCryptoProvider();
                    break;

                case CryptoAlgoritms.TripleDesWith168BitsKey:
                    _cryptor = new TripleDesCryptoProvider();
                    break;

                default:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes256);
                    break;
            }
            _algorithm = cryptoAlgorithm;
            _solter = new SoltGenerator();
            _predicateTranslator = new PredicateTranslator();
            _tables = new Dictionary<Type, TableMap>();
        }

        #endregion


        #region Implementation of IDispose

        /// <summary>
        /// Closes connection to SQLite database file. And cleans internal copies of encryption keys.
        /// !! WARNING !! You must clean encryption key, that you have set using 'SetEncryptionKey()'
        /// function, on your own. 
        /// </summary>
        public void Dispose()
        {
            _cryptor.Dispose(); // clear encryption key!
            _tables.Clear();
            _connection.Dispose();
        }

        #endregion


        #region OPEN-GENERIC METHODS

        private readonly MethodInfo _methodFindFirstByValue =
            typeof(CryptoSQLiteConnection).GetRuntimeMethods()
                .FirstOrDefault(mi => mi.Name == nameof(FindFirstUsingColumnValue)); // FindFirstByValue Method

        private readonly MethodInfo _methodFindReferencedTables =
            typeof(CryptoSQLiteConnection).GetRuntimeMethods()
                .FirstOrDefault(mi => mi.Name == nameof(FindReferencedTables)); // FindReferencedTables Method

        private readonly MethodInfo _methodCheckTable =
            typeof(CryptoSQLiteConnection).GetRuntimeMethods().FirstOrDefault(mi => mi.Name == nameof(CheckTable));

        #endregion


        #region Implementation of ICryptoSQLite

        /// <summary>
        /// Sets the encryption key for all tables in database file. That key will be used for encryption data for all tables, that don't have specific encryption key.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey(byte[] key)
        {
            CheckKey(key);

            _defaultKey = key;
        }

        /// <summary>
        /// Sets the specific encryption key for specific table. This key will be used for encryption data only for specified table: <typeparamref name="TTable"/>.
        /// That allows you to set up different encryption keys for different tables.
        /// This feature significantly increases data security.
        /// <para/>AesWith256BitsKey key length must be 32 bytes.
        /// <para/>AesWith192BitsKey key length must be 24 bytes.
        /// <para/>AesWith128BitsKey key length must be 16 bytes.
        /// <para/>DesWith56BitsKey key length must be 8 bytes.
        /// <para/>TripleDesWith168BitsKey key length must be 24 bytes.
        /// <para/>Gost28147With256BitsKey key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean (Zero) key buffer 
        /// immediately after you finish your work with database.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key.</param>
        /// <typeparam name="TTable">Table for which Encryption Key will be set</typeparam>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey<TTable>(byte[] key) where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            CheckKey(key);

            tableMap.Key = key;
        }



        /// <summary>
        /// Creates a new ordinary table (if it not already exists) in database, that can contain encrypted columns.
        /// <para/>Warning! If table contains any Properties marked as [Encrypted], so 
        /// this table will be containing automatically generated column with name: "SoltColumn". 
        /// <para/>SoltColumn is used in encryption algoritms. If you change value of this column you
        /// won't be able to decrypt data.
        /// <para/>Warning! If you insert element in the table, and then change Properties order in table type (in your class),
        /// you won't be able to decrypt elements. Properties order in table is important thing.
        /// </summary>
        /// <typeparam name="TTable">Type of table to create in database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void CreateTable<TTable>() where TTable : class
        {
            var tableMap = CheckTable<TTable>(false); // here we don't check if table exists in database file

            var cmd = SqlCmds.CmdCreateTable(tableMap);

            try
            {
                _connection.Execute(cmd);
            }
            catch (Exception)
            {
                throw new CryptoSQLiteException("Apparently table name or names of columns contain forbidden symbols");
            }
        }

        /// <summary>
        /// Deletes the table from database.
        /// </summary>
        /// <typeparam name="TTable">Type of table to delete from database.</typeparam>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void DeleteTable<TTable>() where TTable : class
        {
            var table = typeof(TTable);

            var tableName = table.TableName();

            try
            {
                _connection.Execute(SqlCmds.CmdDeleteTable(tableName));
                // it doesn't matter if name wrong or correct and it doesn't matter if table name contains symbols like @#$%^
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode == ErrorCode.Constraint && ex.ExtendedErrorCode == ErrorCode.ConstraintForeignKey)
                    throw new CryptoSQLiteException(
                        $"Can't remove table {tableName} because other tables referenced on her, using ForeignKey Constrait.");
                throw new CryptoSQLiteException(ex.Message, "Unknown");
            }


            if (_tables.ContainsKey(table))
                _tables.Remove(table);
        }

        /// <summary>
        /// Deletes all data inside the table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        public void ClearTable<TTable>() where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            try
            {
                _connection.Execute(SqlCmds.CmdClearTable(tableName));
                // it doesn't matter if name wrong or correct and it doesn't matter if table name contains symbols like @#$%^
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode == ErrorCode.Constraint && ex.ExtendedErrorCode == ErrorCode.ConstraintForeignKey)
                    throw new CryptoSQLiteException(
                        $"Can't remove table {tableName} because other tables referenced on her, using ForeignKey Constrait.");
                throw new CryptoSQLiteException(ex.Message, "Unknown");
            }
        }

        /// <summary>
        /// Returns the number of records in table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <returns></returns>
        public int Count<TTable>() where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var countCmd = SqlCmds.CmdCount(tableName);

            try
            {
                var queryable = _connection.Query(countCmd);

                foreach (var row in queryable)
                {
                    foreach (var column in row)
                    {
                        return column.ToInt();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                var msg = ex.Message;
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Returns the number of records in table that satisfying the condition defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="predicate">Condition</param>
        /// <returns></returns>
        public int Count<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            object[] values;

            var countCmd = _predicateTranslator.CountToSqlCmd(predicate, tableName, tableMap.Columns.Values, out values);

            try
            {
                var queryable = _connection.Query(countCmd, values);

                foreach (var row in queryable)
                {
                    foreach (var column in row)
                    {
                        return column.ToInt();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                var msg = ex.Message;
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Returns the number of values (NULL values won't be counted) for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        public int Count<TTable>(string columnName) where TTable : class
        {
            if(string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var countCmd = SqlCmds.CmdCount(tableName, columnName);

            try
            {
                var queryable = _connection.Query(countCmd);

                foreach (var row in queryable)
                {
                    foreach (var column in row)
                    {
                        return column.ToInt();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                var msg = ex.Message;
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Returns the number of distinct values for the specified column
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="columnName">Name of specified column</param>
        /// <returns></returns>
        public int CountDistinct<TTable>(string columnName) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var countCmd = SqlCmds.CmdCount(tableName, columnName, true);

            try
            {
                var queryable = _connection.Query(countCmd);

                foreach (var row in queryable)
                {
                    foreach (var column in row)
                    {
                        return column.ToInt();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                var msg = ex.Message;
                throw;
            }
            return 0;
        }

        /// <summary>
        /// Inserts new element (row) in table.
        /// The table must be already created.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void InsertItem<TTable>(TTable item) where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            InsertRowInTable(tableMap, item, false);
        }

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// <para/>If you insert element with specified PrimaryKey value this element will replace element in database, that has the same value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void InsertOrReplaceItem<TTable>(TTable item) where TTable : class
        {
            var tableMap = CheckTable<TTable>();

            InsertRowInTable(tableMap, item, true);
        }

        /// <summary>
        /// Removes from table <typeparamref name="TTable"/> all elements which column values satisfy conditions defined in <paramref name="predicate"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table in which elements will be removed</typeparam>
        /// <param name="predicate">condition for column values</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void Delete<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate can't be null");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            object[] values;

            var cmd = _predicateTranslator.DeleteToSqlCmd(predicate, tableName, tableMap.Columns.Values, out values);

            try
            {
                _connection.Execute(cmd, values);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, $"Apparantly table {tableName} doesn't exist in database.");
            }
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void Delete<TTable>(string columnName, object columnValue) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>(false);

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            if (mappedColumns.All(col => col.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (mappedColumns.Any(col => col.Name == columnName && col.IsEncrypted))
                throw new CryptoSQLiteException(
                    "You can't use [Encrypted] column as a column in which the columnValue should be deleted.");

            var cmd = SqlCmds.CmdDeleteRow(tableName, columnName, columnValue);

            try
            {
                if (columnValue == null)
                    _connection.Execute(cmd);
                else _connection.Execute(cmd, columnValue);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    $"Apparently column with name {columnName} doesn't exist in table {tableName}.");
            }
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete(
             "This method is deprecated and soon will be removed. Prealse use 'Delete(string columnName, object columnValue)' method instead.",
             false)]
        public void DeleteItem<TTable>(string columnName, object columnValue) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>(false);

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            if (mappedColumns.All(col => col.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (mappedColumns.Any(col => col.Name == columnName && col.IsEncrypted))
                throw new CryptoSQLiteException(
                    "You can't use [Encrypted] column as a column in which the columnValue should be deleted.");

            var cmd = SqlCmds.CmdDeleteRow(tableName, columnName, columnValue);

            try
            {
                if (columnValue == null)
                    _connection.Execute(cmd);
                else _connection.Execute(cmd, columnValue);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    $"Apparently column with name {columnName} doesn't exist in table {tableName}.");
            }
        }


        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        public IEnumerable<TTable> Table<TTable>() where TTable : class, new()
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            var cmd = SqlCmds.CmdSelectAllTable(tableName);

            //TODO change signature of a call
            var table = ReadRowsFromDatabase(cmd, new object[] {});

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(mappedColumns, row, item);
                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// 
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CryptoSQLiteException"></exception>
        public IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate) where TTable : class, new()
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate can't be null");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = _predicateTranslator.WhereToSqlCmd(predicate, tableName, mappedColumns, out values);

            var table = ReadRowsFromDatabase(cmd, values);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row, item);

                FindReferencedTables(item); // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> value equal to <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all elements which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        public IEnumerable<TTable> FindByValue<TTable>(string columnName, object columnValue)
            where TTable : class, new()
        {
            var tableMap = CheckTable<TTable>();

            return FindUsingColumnValue<TTable>(tableMap, columnName, columnValue);
        }

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Property names whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public IEnumerable<TTable> Select<TTable>(Expression<Predicate<TTable>> predicate,
            params string[] selectedProperties) where TTable : class, new()
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = _predicateTranslator.WhereToSqlCmd(predicate, tableName, mappedColumns, out values,
                selectedProperties);

            var table = ReadRowsFromDatabase(cmd, values);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row, item);

                FindReferencedTables(item, selectedProperties); // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable"></typeparam>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<TTable> SelectTop<TTable>(int count) where TTable : class, new()
        {
            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            var cmd = SqlCmds.CmdSelectTop(tableName);

            var table = ReadRowsFromDatabase(cmd, new object[]{count});

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row, item);

                FindReferencedTables(item); // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }


        #endregion


        #region Private Functions

        /// <summary>
        /// Checks if table has correct columns structure.
        /// And adds type of <typeparamref name="TTable"/> to list of registered tables.
        /// </summary>
        private TableMap CheckTable<TTable>(bool checkExistanceInDatabase = true) where TTable : class
        {
            var tableType = typeof(TTable);

            if (_tables.ContainsKey(typeof(TTable))) return _tables[tableType];

            var tableName = tableType.TableName();

            var compatibleProperties = tableType.CompatibleProperties().ToList();

            CheckAttributes(tableName, compatibleProperties); // just checks correctness of columns attributes

            if (checkExistanceInDatabase)
                CheckIfTableExistsInDatabase(tableName, compatibleProperties);

            var hasEncryptedColumns = false;

            var columnMaps = new Dictionary<string, ColumnMap>();

            // list of all ForeignKey Constraits, so we can Check structure of all referenced tables
            var foreignKeys = new List<ForeignKey>();

            var columnNumber = 0;
            foreach (var prop in compatibleProperties)
            {
                var columnName = prop.ColumnName();

                var isEncrypted = prop.IsEncrypted();
                if (!hasEncryptedColumns && isEncrypted)
                    hasEncryptedColumns = true;

                ForeignKey foreignKey = null;

                var isForeignKey = prop.ForeignKey() != null;
                if (isForeignKey)
                {
                    foreignKey = prop.ForeignKeyInfo<TTable>();
                    foreignKeys.Add(foreignKey);
                }

                var colMap = new ColumnMap<TTable>(columnName, prop.Name, prop.PropertyType, prop.SqlType(), columnNumber,
                    prop.IsPrimaryKey(),
                    prop.IsAutoIncremental(), isEncrypted, prop.IsNotNull(), prop.DefaultValue(), isForeignKey,
                    foreignKey,
                    prop.ValueSetter<TTable>(), prop.ValueGetter<TTable>());

                columnMaps.Add(columnName, colMap);

                columnNumber += 1;
            }

            var tableMap = new TableMap(tableName, hasEncryptedColumns, columnMaps);

            _tables.Add(tableType, tableMap);

            foreach (var fk in foreignKeys) // Check all referenced tables that this table contains
            {
                if (_tables.ContainsKey(fk.TypeOfReferencedTable)) continue;

                var genericCheckTable = _methodCheckTable.MakeGenericMethod(fk.TypeOfReferencedTable);
                try
                {
                    genericCheckTable.Invoke(this, new object[] { true });
                }
                catch (Exception ex)
                {
                    throw new CryptoSQLiteException(ex.InnerException.Message);
                }
                
            }

            return tableMap;
        }

        

        /// <summary>
        /// Checks key
        /// </summary>
        /// <param name="key">key buffer</param>
        private void CheckKey(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException();

            if ((_algorithm == CryptoAlgoritms.AesWith256BitsKey || _algorithm == CryptoAlgoritms.Gost28147With256BitsKey) && key.Length < 32)
                throw new ArgumentException("Key length for AES with 256 bit key and GOST must be 32 bytes.");

            if ((_algorithm == CryptoAlgoritms.AesWith192BitsKey) && key.Length < 24)
                throw new ArgumentException("Key length for AES with 192 bit key must be 24 bytes.");

            if ((_algorithm == CryptoAlgoritms.AesWith128BitsKey) && key.Length < 16)
                throw new ArgumentException("Key length for AES with 128 bit key must be 16 bytes.");

            if (_algorithm == CryptoAlgoritms.DesWith56BitsKey && key.Length < 8)
                throw new ArgumentException("Key length for DES must be at least 8 bytes.");

            if (_algorithm == CryptoAlgoritms.TripleDesWith168BitsKey && key.Length < 24)
                throw new ArgumentException("Key length for 3DES must be at least 24 bytes.");
        }

        //TODO think about this function. This function takes time and reads data from database file
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

            var tableFromOrmMapping = properties.ToList().GetColumnsMappingWithSqlTypes();

            if (!OrmUtils.IsTablesEqual(tableFromFile, tableFromOrmMapping)) // if database doesn't contain TTable
                throw new CryptoSQLiteException(
                    $"SQL Database doesn't contain table with column structure that specified in {tableName}.");
        }
        
        /// <summary>
        /// Checks the structure of attributes for columns (properties) in table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="properties">list of compatible properties</param>
        private static void CheckAttributes(string tableName, IList<PropertyInfo> properties)
        {
            if (properties.Any(p => p.ColumnName() == SoltColumnName))
                throw new CryptoSQLiteException(
                    $"Table can't contain column with name: {SoltColumnName}. This name is reserved for CryptoSQLite needs.");

            if (properties.Count(p => p.IsPrimaryKey()) < 1)
                throw new CryptoSQLiteException("Crypto table must contain at least one PrimaryKey column.");

            if (properties.Count(p => p.IsPrimaryKey()) > 1)
                throw new CryptoSQLiteException("Crypto Table can't contain more that one PrimaryKey column.");

            if (properties.Any(p => p.IsPrimaryKey() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with PrimaryKey Attribute can't be Encrypted.");

            if (properties.Any(p => p.IsAutoIncremental() && p.IsEncrypted()))
                throw new CryptoSQLiteException("Column with AutoIncremental Attribute can't be Encrypted.");

            if(properties.Any(p => p.IsEncrypted() && p.DefaultValue() != null))
                throw new CryptoSQLiteException("Encrypted columns can't have default value, but they can be Not Null.");

            if (properties.Any(p => p.IsPrimaryKey() && p.ForeignKey() != null))
                throw new CryptoSQLiteException("Property can't have ForeignKey and PrimaryKey attributes simultaneously.");

            if (properties.Any(p => p.IsEncrypted() && p.ForeignKey() != null))
                throw new CryptoSQLiteException("Property can't have ForeignKey and Encrypted attributes simultaneously.");

            if (properties.Any(p => p.IsAutoIncremental() && p.ForeignKey() != null))
                throw new CryptoSQLiteException("Property can't have ForeignKey and AutoIncrement attributes simultaneously.");

            if (properties.Any(p => p.ForeignKey() != null && p.DefaultValue() != null))
                throw new CryptoSQLiteException("Property with ForeignKey attribute can't have Default Value.");

            // find columns with equal names
            for (var i = 0; i < properties.Count; i++)
                for (var j = i + 1; j < properties.Count; j++)
                    if (properties[i].ColumnName() == properties[j].ColumnName())
                        throw new CryptoSQLiteException(
                            $"Table {tableName} contains columns with same names {properties[i].ColumnName()}.",
                            "Table can't contain two columns with same names.");
        }


        /// <summary>
        /// Inserts row in table
        /// </summary>
        /// <typeparam name="TTable">Type of table</typeparam>
        /// <param name="tableMap">Map of table</param>
        /// <param name="row">row for insert</param>
        /// <param name="replaceRowIfExisits">flag if need replace existing row</param>
        private void InsertRowInTable<TTable>(TableMap tableMap, TTable row, bool replaceRowIfExisits)
        {
            var columnNames = new List<string>();

            var columnValues = new List<object>();    

            byte[] solt = null;

            ICryptoProvider encryptor = null;

            var tableName = tableMap.Name;

            if (tableMap.HasEncryptedColumns)
            {
                solt = GetSolt();
                encryptor = GetEncryptor(typeof(TTable), solt);
            }

            var columns = tableMap.Columns;

            foreach (var column in columns)
            {
                if(column.Value.IsAutoIncremental && !replaceRowIfExisits)
                    continue;   // if column is AutoIncremental and we don't want to replace this row

                var value = ((IValues<TTable>)column.Value).ValueGetter(row);      // Here we get value without reflection!!! We use here Expressions

                if(value == null && column.Value.DefaultValue != null)
                    continue;   // if column has dafault value, so when column passed without value, we don't use this column in SQL command for insert element 

                if (value == null && column.Value.IsNotNull && column.Value.DefaultValue == null)
                    throw new CryptoSQLiteException($"You are trying to pass NULL-value for Column '{column.Value.Name}', but this column has NotNull atribute and Default Value is not defined.");

                columnNames.Add(column.Key);

                var clrType = column.Value.ClrType;

                object sqlValue = null;

                if (value != null)
                {
                    sqlValue = column.Value.IsEncrypted ? GetEncryptedValueForSql(clrType, value, column.Value.ColumnNumber, encryptor) : GetOpenValueForSql(clrType, value);
                }

                columnValues.Add(sqlValue);   // NULL will be NULL
            }

            if (solt != null)
            {
                columnNames.Add(SoltColumnName);
                columnValues.Add(solt);
            }

            var cmd = replaceRowIfExisits ? SqlCmds.CmdInsertOrReplace(tableName, columnNames) : SqlCmds.CmdInsert(tableName, columnNames);
            try
            {
                _connection.Execute(cmd, columnValues.ToArray());     // Do not remove '.ToArray()' or you'll get a error 
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message, "Column with ForeignKey constrait has invalid value or table doesn't exist in database.");
            }
        }

        private IList<TTable> FindUsingColumnValue<TTable>(TableMap tableMap, string columnName, object columnValue) where TTable : class, new()
        {
            if(string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var mappedColumns = tableMap.Columns.Values;

            var tableName = tableMap.Name;

            if (mappedColumns.All(mc => mc.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if(mappedColumns.Any(mc => mc.Name == columnName && mc.IsEncrypted))
                throw new CryptoSQLiteException("You can't use [Encrypted] column as a column in which the columnValue should be found.");

            var cmd = SqlCmds.CmdSelect(tableName, columnName, columnValue);

            var table = ReadRowsFromDatabase(cmd, new []{columnValue});

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(mappedColumns, row, item);
                items.Add(item);
            }

            return items;
        }

        private TTable FindFirstUsingColumnValue<TTable>(TableMap tableMap, string columnName, object columnValue) where TTable : class, new()
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");
            
            var mappedColumns = tableMap.Columns.Values.ToList();

            var tableName = tableMap.Name;

            if (mappedColumns.All(p => p.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (mappedColumns.Any(p => p.Name == columnName && p.IsEncrypted))
                throw new CryptoSQLiteException("You can't use [Encrypted] column as a column in which the columnValue should be found.");

            var cmd = SqlCmds.CmdSelect(tableName, columnName, columnValue);

            var table = ReadRowsFromDatabase(cmd, new[]{columnValue});

            if (table.Count <= 0) return null;

            var item = new TTable();
            ProcessRow(mappedColumns, table[0], item);
            return item;
        }



        private void FindReferencedTables<TTable>(TTable table, string[] selectedColumns = null) where TTable : class, new()
        {
            var tableType = typeof(TTable);
            if(!_tables.ContainsKey(tableType))
                return;

            var referencedTablesInfos = _tables[tableType].Columns.ForeignKeys();

            if (selectedColumns != null)
                referencedTablesInfos = referencedTablesInfos.Where(fk => selectedColumns.Contains(fk.ForeignKeyPropertyName)).ToList();
            
            if(referencedTablesInfos == null)
                return;

            foreach (var refTableInfo in referencedTablesInfos)
            {
                var foreignProperty = table.GetType().GetRuntimeProperty(refTableInfo.ForeignKeyPropertyName);      // get property that contain value of foreign key, that referenced to another table

                var primaryKeyValue = foreignProperty.GetValue(table);                                              // get value of foreign key

                var genericFindFirstByValue = _methodFindFirstByValue.MakeGenericMethod(refTableInfo.TypeOfReferencedTable);         // create genetic method to get referenced table instance using foreign key column name and value

                // FindFirstByValue has three arguments: refTableMap, refColumnName, refColumnValue
                var refTable = genericFindFirstByValue.Invoke(this, new[] { _tables[refTableInfo.TypeOfReferencedTable], refTableInfo.ReferencedColumnName, primaryKeyValue });   // invoking generic method and getting instance of referenced table

                var navigationProperty = table.GetType().GetRuntimeProperty(refTableInfo.NavigationPropertyName);   // get property in current table that must navigate to referenced table

                navigationProperty.SetValue(table, refTable);                                           // pass reference to referenced table to navigation property

                if (refTable == null) continue;

                var genericFindReferencedTables = _methodFindReferencedTables.MakeGenericMethod(refTable.GetType());

                genericFindReferencedTables.Invoke(this, new[] { refTable, null });     // Recursive call
            }
        }

        private List<List<SqlColumnInfo>> ReadRowsFromDatabase(string cmd, IEnumerable<object> values)
        {
            var table = new List<List<SqlColumnInfo>>();
            try
            {
                var notNullValues = values.Where(v => v != null).ToArray();

                var queryable = notNullValues.Length == 0 ? _connection.Query(cmd) : _connection.Query(cmd, notNullValues);

                foreach (var row in queryable)
                {
                    var columnsFromFile = new List<SqlColumnInfo>();
                    foreach (var column in row)
                    {
                        var tmp = new SqlColumnInfo {Name = column.ColumnInfo.Name};

                        if (column.SQLiteType != SQLiteType.Null)   // if we get NULL type, then NULL will stay NULL
                        {
                            switch (column.ColumnInfo.DeclaredType)
                            {
                                case "BLOB":
                                    tmp.SqlValue = column.ToBlob();
                                    break;
                                case "REAL":
                                    if (column.SQLiteType == SQLiteType.Text)   // for default double values
                                    {
                                        var str = column.ToString();
                                        double val;
                                        tmp.SqlValue = double.TryParse(str, out val) ? val : column.ToDouble();
                                    }
                                    else
                                        tmp.SqlValue = column.ToDouble();
                                    break;
                                case "INTEGER":
                                    tmp.SqlValue = column.ToInt64();
                                    break;
                                case "TEXT":
                                    tmp.SqlValue = column.ToString();
                                    break;
                                case "NULL":
                                    tmp.SqlValue = null;
                                    break;
                                default:
                                    throw new CryptoSQLiteException("Type is not compatible with SQLite database");
                            }
                        }
                        columnsFromFile.Add(tmp);   // NULL will be NULL.
                    }
                    table.Add(columnsFromFile);
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message);
            }
            return table;
        }

        private void ProcessRow<TTable>(IEnumerable<ColumnMap> mappedColumns, IList<SqlColumnInfo> databaseColumns, TTable item) where TTable : class 
        {
            ICryptoProvider encryptor = null;
            var soltColumn = databaseColumns.FirstOrDefault(c => c.Name == SoltColumnName);
            if (soltColumn != null)
            {
                var solt = (byte[])soltColumn.SqlValue;
                encryptor = GetEncryptor(typeof(TTable), solt); // this solt we were using in encryption of columns
            }

            foreach (var columnMap in mappedColumns)
            {
                var column = databaseColumns.FirstOrDefault(c => c.Name == columnMap.Name);

                if (column?.SqlValue == null)   // NULL value will stay NULL value
                    continue;

                if (columnMap.IsEncrypted)
                    SetDecryptedValueForClr(columnMap, item, column.SqlValue, encryptor);
                else
                    SetOpenValueForClr(columnMap, item, column.SqlValue);
            }
        }

        /// <summary>
        /// Returns encrypted value that must be stored in database
        /// </summary>
        /// <param name="type">Initial type of value</param>
        /// <param name="value">value</param>
        /// <param name="columnNumber">number of column</param>
        /// <param name="encryptor">ecryptor</param>
        /// <returns></returns>
        private static object GetEncryptedValueForSql(Type type, object value, int columnNumber, ICryptoProvider encryptor)
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            if (type == typeof(string))
            {
                var bytes = Encoding.Unicode.GetBytes((string)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var dateTime = (DateTime) value;
                var bytes = BitConverter.GetBytes(dateTime.ToBinary());
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(ushort) || type == typeof(ushort?))
            {
                var bytes = BitConverter.GetBytes((ushort)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                var bytes = BitConverter.GetBytes((short)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(uint) || type == typeof(uint?))
            {
                var bytes = BitConverter.GetBytes((uint)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                var bytes = BitConverter.GetBytes((int)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(ulong) || type == typeof(ulong?))
            {
                var bytes = BitConverter.GetBytes((ulong)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                var bytes = BitConverter.GetBytes((long)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                var bytes = BitConverter.GetBytes((float)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                var bytes = BitConverter.GetBytes((double)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                var bytes = ((decimal) value).GetBytes();
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(byte[]))
            {
                var bytes = (byte[])value;
                var bytesForEncrypt = new byte[bytes.Length];
                bytesForEncrypt.MemCpy(bytes, bytes.Length);
                encryptor.XorGamma(bytesForEncrypt, columnNumber);
                return bytesForEncrypt;
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                var bytes = BitConverter.GetBytes((bool) value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                var bytes = BitConverter.GetBytes((byte)value);
                encryptor.XorGamma(bytes, columnNumber);
                return bytes;
            }
            
            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite");
        }
     
        private static void SetDecryptedValueForClr<TTable>(ColumnMap columnMap, TTable item, object sqlValue, ICryptoProvider encryptor) where TTable : class 
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            var type = columnMap.ClrType;

            var setter = ((ColumnMap<TTable>) columnMap).ValueSetter;

            var columnNumber = columnMap.ColumnNumber;

            var bytes = (byte[])sqlValue;   // all encrypted properties stored in database file as BLOB type

            if (type == typeof(string))
            {
                encryptor.XorGamma(bytes, columnNumber);
                var openString = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                setter(item, openString);       // Here is no Reflection now! we use Expressions
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                var ticks = BitConverter.ToInt64(bytes, 0);
                var dateTime = DateTime.FromBinary(ticks);
                setter(item, dateTime);
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToInt16(bytes, 0));
            }
            else if (type == typeof(ushort) || type == typeof(ushort?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToUInt16(bytes, 0));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToInt32(bytes, 0));
            }
            else if (type == typeof(uint) || type == typeof(uint?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToUInt32(bytes, 0));
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToInt64(bytes, 0));
            }
            else if (type == typeof(ulong) || type == typeof(ulong?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToUInt64(bytes, 0));
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToSingle(bytes, 0));
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToDouble(bytes, 0));
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, bytes.ToDecimal());
            }
            else if (type == typeof(byte[]))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, bytes);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, BitConverter.ToBoolean(bytes, 0));
            }
            else if(type == typeof(byte) || type == typeof(byte?))
            {
                encryptor.XorGamma(bytes, columnNumber);
                setter(item, bytes[0]);
            }
        }

        private static void SetOpenValueForClr<TTable>(ColumnMap columnMap, TTable item, object sqlValue)
        {
            var type = columnMap.ClrType;

            var setter = ((ColumnMap<TTable>) columnMap).ValueSetter;

            if (type == typeof(string))
                setter(item, sqlValue);     // There is no reflection here! Only compiled expressions
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var bytes = (byte[])sqlValue;
                var ticks = BitConverter.ToInt64(bytes, 0);
                var date = DateTime.FromBinary(ticks);
                setter(item, date);
            }
            else if (type == typeof(short) || type == typeof(short?))
                setter(item, Convert.ToInt16(sqlValue));
            else if (type == typeof(ushort) || type == typeof(ushort?))
                setter(item, Convert.ToUInt16(sqlValue));
            else if (type == typeof(int) || type == typeof(int?))
                setter(item, Convert.ToInt32(sqlValue));
            else if (type == typeof(uint) || type == typeof(uint?))
                setter(item, Convert.ToUInt32(sqlValue));
            else if (type == typeof(long) || type == typeof(long?))
            {
                var value = BitConverter.ToInt64((byte[])sqlValue, 0);
                setter(item, value);
            }
            else if (type == typeof(ulong) || type == typeof(ulong?))
            {
                var value = BitConverter.ToUInt64((byte[])sqlValue, 0);
                setter(item, value);
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                setter(item, ((byte[]) sqlValue).ToDecimal());
            }
            else if (type == typeof(byte) || type == typeof(byte?))
                setter(item, Convert.ToByte(sqlValue));
            else if (type == typeof(bool) || type == typeof(bool?))
                setter(item, Convert.ToBoolean(sqlValue));
            else if (type == typeof(double) || type == typeof(double?))
                setter(item, Convert.ToDouble(sqlValue));
            else if (type == typeof(float) || type == typeof(float?))
                setter(item, Convert.ToSingle(sqlValue));
            else if (type == typeof(byte[]))
                setter(item, (byte[])sqlValue);
        }

        private static object GetOpenValueForSql(Type type, object value)
        {
            if (type == typeof(int) || type == typeof(short) || type == typeof(double) || type == typeof(byte) ||
                type == typeof(uint) || type == typeof(ushort) || type == typeof(float))
                return value;

            if (type == typeof(int?) || type == typeof(short?) || type == typeof(double?) || type == typeof(byte?) ||
                type == typeof(uint?) || type == typeof(ushort?) || type == typeof(float?))
                return value;

            if (type == typeof(string) || type == typeof(byte[]))       // reference types
                return value;

            if (type == typeof(long) || type == typeof(long?))
                return BitConverter.GetBytes((long)value);

            if (type == typeof(ulong) || type == typeof(ulong?))
                return BitConverter.GetBytes((ulong)value);

            if (type == typeof(decimal) || type == typeof(decimal?))
                return ((decimal) value).GetBytes();

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var date = (DateTime)value;
                var ticks = date.ToBinary();
                return BitConverter.GetBytes(ticks);
            }

            if (type == typeof(bool) || type == typeof(bool?))
                return Convert.ToInt32(value);

            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite.");
        }


        private ICryptoProvider GetEncryptor(Type tableType, byte[] solt = null)
        {
            if (_tables.ContainsKey(tableType) && _tables[tableType].Key != null)
                _cryptor.SetKey(_tables[tableType].Key);
            else
            {
                if(_defaultKey == null)
                    throw new CryptoSQLiteException("Encryption key has not been installed.");

                _cryptor.SetKey(_defaultKey);
            }
            if(solt != null)
                _cryptor.SetSolt(solt);

            return _cryptor;
        }

        private byte[] GetSolt()
        {
            var solt = _solter.GetSolt();
            return solt;
        }

        #endregion
    }
}
