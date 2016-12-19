using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite.CryptoProviders;
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
    /// Interface of SQLite connection to database file with data encryption
    /// </summary>
    public interface ICryptoSQLiteConnection
    {
        /// <summary>
        /// Sets the encryption key, that will be use in encryption algorithms for data encryption.
        /// <para/>AES key length must be 32 bytes.
        /// <para/>DES key length must be 8 bytes.
        /// <para/>3DES key length must be 24 bytes.
        /// <para/>GOST key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean content 
        /// of <paramref name="key"/> immediately after you finish work with it.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Creates a new table (if it not already exists) in database, that can contain encrypted columns.
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
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        TTable GetItem<TTable>(string columnName, object columnValue) where TTable : class, new();

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        TTable GetItem<TTable>(int id) where TTable : class, new();

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        TTable GetItem<TTable>(TTable item) where TTable : class, new();

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable>(int id) where TTable: class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        void DeleteItem<TTable>(TTable item) where TTable : class;

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
        /// Finds all the elements whose <paramref name="columnName"/>-values lie between
        /// <paramref name="lowerValue"/> and <paramref name="upperValue"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="lowerValue">Lower value (inclusive).</param>
        /// <param name="upperValue">Upper value (inclusive).</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        IEnumerable<TTable> Find<TTable>(string columnName, object lowerValue = null, object upperValue = null) where TTable : class, new();

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


    }

    /// <summary>
    /// Interface of SQLite Async connection to database file with data encryption
    /// </summary>
    public interface ICryptoSQLiteAsyncConnection
    {
        /// <summary>
        /// Sets the encryption key, that will be use in encryption algorithms for data encryption.
        /// <para/>AES key length must be 32 bytes.
        /// <para/>DES key length must be 8 bytes.
        /// <para/>3DES key length must be 24 bytes.
        /// <para/>GOST key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean content 
        /// of <paramref name="key"/> immediately after you finish work with it.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        void SetEncryptionKey(byte[] key);

        /// <summary>
        /// Creates a new table (if it not already exists) in database, that can contain encrypted columns.
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
        Task DeleteTableAsync<TTable>() where TTable: class;

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
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        Task<TTable> GetItemAsync<TTable>(string columnName, object columnValue) where TTable : class, new();

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        Task<TTable> GetItemAsync<TTable>(int id) where TTable : class, new();

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        Task<TTable> GetItemAsync<TTable>(TTable item) where TTable : class, new();

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable>(TTable item) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable>(string columnName, object columnValue) where TTable : class;

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        Task DeleteItemAsync<TTable>(int id) where TTable : class;

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
        /// Finds all the elements whose <paramref name="columnName"/>-values lie between
        /// <paramref name="lowerValue"/> and <paramref name="upperValue"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="lowerValue">Lower value (inclusive).</param>
        /// <param name="upperValue">Upper value (inclusive).</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        Task<IEnumerable<TTable>> FindAsync<TTable>(string columnName, object lowerValue = null, object upperValue = null) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> contain value == <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all rows which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        Task<IEnumerable<TTable>> FindByValueAsync<TTable>(string columnName, object columnValue) where TTable : class, new();



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
        /// Sets the encryption key, that will be use in encryption algorithms for data encryption.
        /// <para/>AES key length must be 32 bytes.
        /// <para/>DES key length must be 8 bytes.
        /// <para/>3DES key length must be 24 bytes.
        /// <para/>GOST key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean content 
        /// of <paramref name="key"/> immediately after you finish work with it.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey(byte[] key)
        {
            _connection.SetEncryptionKey(key);
        }

        /// <summary>
        /// Creates a new table (if it not already exists) in database, that can contain encrypted columns.
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
        /// <para/>If you insert element with setted PrimaryKey value this element will replace element in database, that has same PrimaryKey value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task InsertOrReplaceItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.InsertOrReplaceItem(item));
        }

        /// <summary>
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// If table contain two or more elements with value <paramref name="columnValue"/> only first element will be returned.
        /// In this case use find function.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public async Task<TTable> GetItemAsync<TTable>(string columnName, object columnValue) where TTable : class, new()
        {
            var table = Task.Run(() => _connection.GetItem<TTable>(columnName, columnValue));
            return await table;
        }

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public async Task<TTable> GetItemAsync<TTable>(int id) where TTable : class, new()
        {
            var table = Task.Run(() => _connection.GetItem<TTable>(id));
            return await table;
        }

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public async Task<TTable> GetItemAsync<TTable>(TTable item) where TTable : class, new()
        {
            var table = Task.Run(() => _connection.GetItem(item));
            return await table;
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task DeleteItemAsync<TTable>(TTable item) where TTable : class
        {
            await Task.Run(() => _connection.DeleteItem(item));
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
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task DeleteItemAsync<TTable>(int id) where TTable : class
        {
            await Task.Run(() => _connection.DeleteItem<TTable>(id));
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
        /// Finds all the elements whose <paramref name="columnName"/>-values lie between
        /// <paramref name="lowerValue"/> and <paramref name="upperValue"/>.
        /// <para/>If <paramref name="lowerValue"/>==null, it will find all elements, which <paramref name="columnName"/> values are less or equal <paramref name="upperValue"/>
        /// <para/>If <paramref name="upperValue"/>==null, it will find all elements, which <paramref name="columnName"/> values are greater or equal <paramref name="lowerValue"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="lowerValue">Lower value (inclusive).</param>
        /// <param name="upperValue">Upper value (inclusive).</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CryptoSQLiteException"></exception>
        public async Task<IEnumerable<TTable>> FindAsync<TTable>(string columnName, object lowerValue = null, object upperValue = null)
            where TTable : class, new()
        {
            var elements = await Task.Run(() => _connection.Find<TTable>(columnName, lowerValue, upperValue));
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

        private readonly SQLiteDatabaseConnection _connection;

        private readonly ICryptoProvider _cryptor;

        private readonly ISoltGenerator _solter;

        private readonly CryptoAlgoritms _algorithm;

        private readonly IDictionary<string, IList<ForeignKey>> _checkedTables;

        internal const string SoltColumnName = "SoltColumn";

        #endregion


        #region Constructors

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFilename">Path to SQLite database file.</param>
        public CryptoSQLiteConnection(string dbFilename)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
            
            _cryptor = new AesCryptoProvider();
            _algorithm = CryptoAlgoritms.AesWith256BitsKey;
            _solter = new SoltGenerator();
            _checkedTables = new Dictionary<string, IList<ForeignKey>>();
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
                    _cryptor = new AesCryptoProvider();
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
                    _cryptor = new AesCryptoProvider();
                    break;
            }
            _algorithm = cryptoAlgorithm;
            _solter = new SoltGenerator();
            _checkedTables = new Dictionary<string, IList<ForeignKey>>();
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
            _cryptor.Dispose();     // clear encryption key!
            _checkedTables.Clear();
            _connection.Dispose();
        }

        #endregion


        #region Implementation of ICryptoSQLite

        /// <summary>
        /// Sets the encryption key, that will be use in encryption algorithms for data encryption.
        /// <para/>AES key length must be 32 bytes.
        /// <para/>DES key length must be 8 bytes.
        /// <para/>3DES key length must be 24 bytes.
        /// <para/>GOST key length must be 32 bytes.
        /// <para/>WARNING <paramref name="key"/> is a secret parameter. You must clean content 
        /// of <paramref name="key"/> immediately after you finish work with it.
        /// </summary>
        /// <param name="key">Buffer, that contains encryption key. Length must be 32 bytes.</param>
        /// <exception cref="NullReferenceException"></exception>
        public void SetEncryptionKey(byte[] key)
        {
            if(key == null)
                throw new ArgumentNullException();

            if((_algorithm == CryptoAlgoritms.AesWith256BitsKey || _algorithm == CryptoAlgoritms.Gost28147With256BitsKey) && key.Length != 32)
                throw new ArgumentException("Key length for AES and GOST must be 32 bytes.");

            if(_algorithm == CryptoAlgoritms.DesWith56BitsKey && key.Length < 8)
                throw new ArgumentException("Key length for DES must be at least 8 bytes.");

            if(_algorithm == CryptoAlgoritms.TripleDesWith168BitsKey && key.Length < 24)
                throw new ArgumentException("Key length for 3DES must be at least 24 bytes.");

            _cryptor?.SetKey(key);
        }

        /// <summary>
        /// Creates a new table (if it not already exists) in database, that can contain encrypted columns.
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
            var table = typeof(TTable);

            CheckTable(table, false);   // here we don't check if table exists in database file
            
            var cmd = SqlCmds.CmdCreateTable(table);

            try
            {
                _connection.Execute(cmd);
            }
            catch (Exception ex)
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
                _connection.Execute(SqlCmds.CmdDeleteTable(tableName));     // it doesn't matter if name wrong or correct and it doesn't matter if table name contains symbols like @#$%^
            }
            catch (SQLitePCL.pretty.SQLiteException ex)
            {
                if(ex.ErrorCode == ErrorCode.Constraint && ex.ExtendedErrorCode == ErrorCode.ConstraintForeignKey)
                    throw new CryptoSQLiteException($"Can't remove table {tableName} because other tables referenced on her, using ForeignKey Constrait.");
                throw new CryptoSQLiteException(ex.Message, "Unknown");
            }


            if (_checkedTables.ContainsKey(table.ToString()))
                _checkedTables.Remove(tableName);
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
            CheckTable(typeof(TTable));
            InsertRowInTable(item, false);
        }

        /// <summary>
        /// Inserts or replaces the element if it exists in database.
        /// <para/>If you insert element with setted PrimaryKey value this element will replace element in database, that has same PrimaryKey value.
        /// </summary>
        /// <typeparam name="TTable">Type of table in which the new element will be inserted.</typeparam>
        /// <param name="item">Instance of element to insert.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void InsertOrReplaceItem<TTable>(TTable item) where TTable : class
        {
            CheckTable(typeof(TTable));

            InsertRowInTable(item, true);
        }

        /// <summary>
        /// Gets element from table in database that has column: <paramref name="columnName"/> with value: <paramref name="columnValue"/>.
        /// If table contain two or more elements with value <paramref name="columnValue"/> only first element will be returned.
        /// In this case use find function.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        ///  </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="columnName">column name.</param>
        /// <param name="columnValue">column value.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public TTable GetItem<TTable>(string columnName, object columnValue) where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            return FindUsingColumnValue<TTable>(columnName, columnValue).FirstOrDefault();
        }

        /// <summary>
        /// Gets element from database using element <paramref name="id"/>.
        /// Type <typeparamref name="TTable"/> must contain the column (read/write Property) with any name: "id", "Id", "iD", "ID".
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which element will be getted.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public TTable GetItem<TTable>(int id) where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            var properties = typeof(TTable).GetColumns().ToArray();

            var idProperty = properties.First(p => p.ColumnName().ToLower() == "id");
            if (idProperty == null)
                throw new CryptoSQLiteException(
                    $"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

            return FindUsingColumnValue<TTable>(idProperty.ColumnName(), id).FirstOrDefault();
        }

        /// <summary>
        /// Gets element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// <para/>This method is deprecated. Please use Find[<typeparamref name="TTable"/>](Predicate predicate) method.
        /// </summary>
        /// <typeparam name="TTable">Type of Table from which the element will be taken.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <returns>Instance of element with type <typeparamref name="TTable"/> that will be created using data from table <typeparamref name="TTable"/> in database.</returns>
        /// <exception cref="CryptoSQLiteException"></exception>
        [Obsolete("This method is deprecated. Use Find<TTable> methods methods.", false)]
        public TTable GetItem<TTable>(TTable item) where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            var properties = typeof(TTable).GetColumns().ToArray();

            var notDefaultValue = properties.First(p => !p.IsDefaultValue(p.GetValue(item)));              

            return FindUsingColumnValue<TTable>(notDefaultValue.ColumnName(), notDefaultValue.GetValue(item)).FirstOrDefault();
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="columnName">Column name.</param>
        /// <param name="columnValue">Column value.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void DeleteItem<TTable>(string columnName, object columnValue) where TTable : class
        {
            CheckTable(typeof(TTable));

            DeleteRowUsingColumnName<TTable>(columnName, columnValue);
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database using
        /// identification number of element. Type <typeparamref name="TTable"/> must contain the column (read and write Properties) with any name: "id", "Id", "iD", "ID".
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="id">Identifacation number of element in table.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void DeleteItem<TTable>(int id) where TTable : class
        {
            CheckTable(typeof(TTable));

            var properties = typeof(TTable).GetColumns().ToArray();

            var idProperty = properties.First(p => p.ColumnName().ToLower() == "id");
            if (idProperty == null)
                throw new CryptoSQLiteException(
                    $"Type {typeof(TTable)} of item doesn't contain property with name \"id\" (\"Id\", \"ID\", \"iD\")");

            DeleteRowUsingColumnName<TTable>(idProperty.ColumnName(), id);
        }

        /// <summary>
        /// Removes element from table <typeparamref name="TTable"/> in database.
        /// In instance of type <typeparamref name="TTable"/> at least one Property from all (read and write) Properties must be initialized.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element will be removed.</typeparam>
        /// <param name="item">Instance of element <typeparamref name="TTable"/> that contains at least one initialized Property.</param>
        /// <exception cref="CryptoSQLiteException"></exception>
        public void DeleteItem<TTable>(TTable item) where TTable : class
        {
            CheckTable(typeof(TTable));

            var properties = typeof(TTable).GetColumns().ToArray();

            var notDefaultValue = properties.First(p => !p.IsDefaultValue(p.GetValue(item)));

            DeleteRowUsingColumnName<TTable>(notDefaultValue.ColumnName(), notDefaultValue.GetValue(item));
        }

        /// <summary>
        /// Gets all elements from table <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of table with information about table</typeparam>
        /// <returns>All elements from table <typeparamref name="TTable"/></returns>
        public IEnumerable<TTable> Table<TTable>() where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            return FindInTable<TTable>("");     // all table
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
            CheckTable(typeof(TTable));

            var tableName = typeof(TTable).TableName();

            PropertyInfo[] columns = typeof(TTable).GetColumns().ToArray();

            var predicateTraslator = new PredicateToSql();

            object[] values;

            var cmd = predicateTraslator.WhereToSqlCmd(predicate, tableName, columns, out values);

            var table = ReadRowsFromTable(cmd, values);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(columns, row, item);

                FindReferencedTables(item);     // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }


        /// <summary>
        /// Finds all the elements whose <paramref name="columnName"/>-values are lying between
        /// <paramref name="lowerValue"/> and <paramref name="upperValue"/>.
        /// <para/>If <paramref name="lowerValue"/>==null, it will find all elements, which <paramref name="columnName"/> values are less or equal <paramref name="upperValue"/>
        /// <para/>If <paramref name="upperValue"/>==null, it will find all elements, which <paramref name="columnName"/> values are greater or equal <paramref name="lowerValue"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="lowerValue">Lower value (inclusive).</param>
        /// <param name="upperValue">Upper value (inclusive).</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        public IEnumerable<TTable> Find<TTable>(string columnName, object lowerValue = null, object upperValue = null)
            where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            return FindInTable<TTable>(columnName, lowerValue, upperValue);
        }

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> value equal to <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all elements which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        public IEnumerable<TTable> FindByValue<TTable>(string columnName, object columnValue) where TTable : class, new()
        {
            CheckTable(typeof(TTable));

            return FindUsingColumnValue<TTable>(columnName, columnValue);
        }




        #endregion


        #region Private Functions

        /// <summary>
        /// Checks if table has correct columns structure.
        /// And adds type of <paramref name="tableType"/> to list of registered tables.
        /// </summary>
        private void CheckTable(Type tableType, bool checkExistanceInDatabase = true)
        {
            var tableName = tableType.TableName();

            if (_checkedTables.ContainsKey(tableType.ToString())) return;

            var columns = tableType.GetColumns().ToList();

            CheckAttributes(tableName, columns);

            if(checkExistanceInDatabase)
                CheckIfTableExistsInDatabase(tableName, columns);

            var referencedTables = columns.ForeignKeys(tableType);  // Find all referenced tables

            _checkedTables.Add(tableType.ToString(), referencedTables);

            foreach (var fk in referencedTables)     // Check all referenced tables that this table contains
            {
                if(!_checkedTables.ContainsKey(fk.TypeOfReferencedTable.ToString()))
                    CheckTable(fk.TypeOfReferencedTable);
            }
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

            var tableFromOrmMapping = OrmUtils.GetColumnsMappingWithSqlTypes(properties.ToList());

            if (!OrmUtils.IsTablesEqual(tableFromFile, tableFromOrmMapping)) // if database doesn't contain TTable
                throw new CryptoSQLiteException(
                    $"SQL Database doesn't contain table with column structure that specified in {tableName}.");
        }
        
        /// <summary>
        /// Checks the structure of attributes for columns (properties) in table
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="properties">list of compatible properties</param>
        private static /*TableMap*/void CheckAttributes(string tableName, IList<PropertyInfo> properties)
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

            if (properties.Any(p => p.IsPrimaryKey() && p.IsForeignKey()))
                throw new CryptoSQLiteException("Property can't have ForeignKey and PrimaryKey attributes simultaneously.");

            if (properties.Any(p => p.IsEncrypted() && p.IsForeignKey()))
                throw new CryptoSQLiteException("Property can't have ForeignKey and Encrypted attributes simultaneously.");

            if (properties.Any(p => p.IsAutoIncremental() && p.IsForeignKey()))
                throw new CryptoSQLiteException("Property can't have ForeignKey and AutoIncrement attributes simultaneously.");

            if (properties.Any(p => p.IsForeignKey() && p.DefaultValue() != null))
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
        /// <param name="row">row for insert</param>
        /// <param name="replaceRowIfExisits">flag if need replace existing row</param>
        private void InsertRowInTable<TTable>(TTable row, bool replaceRowIfExisits)
        {
            var columns = new List<string>();
            var values = new List<object>();
            byte[] solt = null;
            ICryptoProvider encryptor = null;

            var columnsToAdd = typeof(TTable).GetColumns().ToList();

            if (columnsToAdd.Find(p => p.IsEncrypted()) != null)
            {
                solt = GetSolt();
                encryptor = GetEncryptor(solt);
            }

            foreach (var col in columnsToAdd)
            {
                if (col.IsAutoIncremental() && !replaceRowIfExisits)
                    continue;       // if column is AutoIncremental and we don't want to replace this row

                var value = col.GetValue(row);  // column value

                if (value == null && col.DefaultValue() != null)    // if column has dafault value, so when column passed without value, we don't use this column in SQL command for insert element 
                    continue;

                if(value == null && col.IsNotNull() && col.DefaultValue() == null)
                    throw new CryptoSQLiteException($"You are trying to pass NULL-value for Column '{col.ColumnName()}', but this column has NotNull atribute and Default Value is not defined.");

                columns.Add(col.ColumnName());
                
                var type = col.PropertyType;

                object sqlValue = null;

                if (value != null)
                {
                    // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                    if (col.IsEncrypted())
                        sqlValue = GetEncryptedValue(type, value, encryptor);
                    else
                        sqlValue = OrmUtils.GetSqlViewFromClr(type, value);
                }

                values.Add(sqlValue);   // NULL will be NULL
            }

            if (solt != null)
            {
                columns.Add(SoltColumnName);
                values.Add(solt);
            }

            var name = typeof(TTable).TableName();
            if (replaceRowIfExisits)
            {
                var cmd = SqlCmds.CmdInsertOrReplace(name, columns);
                try
                {
                    _connection.Execute(cmd, values.ToArray());     // Do not remove '.ToArray()' or you'll get a error 
                }
                catch (Exception ex)
                {
                    throw new CryptoSQLiteException(ex.Message, "Column with ForeignKey constrait has invalid value or table doesn't exist in database.");
                }
            }
            else
            {
                var cmd = SqlCmds.CmdInsert(name, columns);
                try
                {
                    _connection.Execute(cmd, values.ToArray());     // Do not remove '.ToArray()' or you'll get a error 
                }
                catch (Exception ex)
                {
                    throw new CryptoSQLiteException(ex.Message, "Column with ForeignKey constrait has invalid value or table doesn't exist in database.");
                }
            }
        }

        private IList<TTable> FindUsingColumnValue<TTable>(string columnName, object columnValue) where TTable : class, new()
        {
            if(string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var properties = typeof(TTable).GetColumns().ToArray();

            var tableName = typeof(TTable).TableName();

            if (properties.All(p => p.ColumnName() != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if(properties.Any(p=>p.ColumnName()==columnName && p.IsEncrypted()))
                throw new CryptoSQLiteException("You can't use [Encrypted] column as a column in which the columnValue should be found.");

            string cmd;

            if (columnValue != null)
                cmd = SqlCmds.CmdSelect(tableName, columnName);
            else cmd = SqlCmds.CmdFindNullInTable(tableName, columnName);

            var table = ReadRowsFromTable(cmd, columnValue);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(properties, row, item);
                items.Add(item);
            }

            return items;
        }

        private TTable FindFirstUsingColumnValue<TTable>(string columnName, object columnValue) where TTable : class, new()
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var properties = typeof(TTable).GetColumns().ToArray();

            var tableName = typeof(TTable).TableName();

            if (properties.All(p => p.ColumnName() != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (properties.Any(p => p.ColumnName() == columnName && p.IsEncrypted()))
                throw new CryptoSQLiteException("You can't use [Encrypted] column as a column in which the columnValue should be found.");

            string cmd;

            if (columnValue != null)
                cmd = SqlCmds.CmdSelect(tableName, columnName);
            else cmd = SqlCmds.CmdFindNullInTable(tableName, columnName);

            var table = ReadRowsFromTable(cmd, columnValue);

            if (table.Count > 0)
            {
                var item = new TTable();
                ProcessRow(properties, table[0], item);
                return item;
            }

            return null;
        }

        private readonly MethodInfo _methodFindFirstByValue = typeof(CryptoSQLiteConnection).GetRuntimeMethods().FirstOrDefault(mi => mi.Name == nameof(FindFirstUsingColumnValue));     // FindFirstByValue Method

        private readonly MethodInfo _methodFindReferencedTables = typeof(CryptoSQLiteConnection).GetRuntimeMethods().FirstOrDefault(mi => mi.Name == nameof(FindReferencedTables));   // FindReferencedTables Method

        private void FindReferencedTables<TTable>(TTable table) where TTable : class, new()
        {
            if(!_checkedTables.ContainsKey(typeof(TTable).ToString()))
                return;

            var referencedTablesInfos = _checkedTables[typeof(TTable).ToString()];

            foreach (var refTableInfo in referencedTablesInfos)
            {
                var foreignProperty = table.GetType().GetRuntimeProperty(refTableInfo.ForeignKeyPropertyName);      // get property that contain value of foreign key, that referenced to another table

                var primaryKeyValue = foreignProperty.GetValue(table);                                              // get value of foreign key

                var genericFindFirstByValue = _methodFindFirstByValue.MakeGenericMethod(refTableInfo.TypeOfReferencedTable);         // create genetic method to get referenced table instance using foreign key column name and value

                var refTable = genericFindFirstByValue.Invoke(this, new[] { refTableInfo.ReferencedColumnName, primaryKeyValue });   // invoking generic method and getting instance of referenced table

                var navigationProperty = table.GetType().GetRuntimeProperty(refTableInfo.NavigationPropertyName);   // get property in current table that must navigate to referenced table

                navigationProperty.SetValue(table, refTable);                                           // pass reference to referenced table to navigation property

                if (refTable != null)
                {
                    var genericFindReferencedTables = _methodFindReferencedTables.MakeGenericMethod(refTable.GetType());

                    genericFindReferencedTables.Invoke(this, new[] { refTable });     // Recursive call
                }
            }
        }

        private void DeleteRowUsingColumnName<TTable>(string columnName, object columnValue)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            if (columnValue == null)
                throw new CryptoSQLiteException("Column value can't be null.");

            var tableName = typeof(TTable).TableName();

            var properties = typeof(TTable).GetColumns().ToArray();

            if (properties.All(p => p.ColumnName() != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (properties.Any(p => p.ColumnName() == columnName && p.IsEncrypted()))
                throw new CryptoSQLiteException("You can't use [Encrypted] column as a column in which the columnValue should be deleted.");

            var cmd = SqlCmds.CmdDeleteRow(tableName, columnName);

            try
            {
                _connection.Execute(cmd, columnValue);
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    $"Apparently column with name {columnName} doesn't exist in table {tableName}.");
            }
        }
        
        private IEnumerable<TTable> FindInTable<TTable>(string columnName, object lowerValue = null, object upperValue = null) where TTable : new()
        {
            var tableName = typeof(TTable).TableName();

            var properties = typeof(TTable).GetColumns().ToArray();

            if (columnName == null)
                throw new CryptoSQLiteException("Column name can't be null.");

            if (!string.IsNullOrEmpty(columnName))  
            {
                // we come here only if we in Find function
                if (properties.All(p => p.ColumnName() != columnName))
                    throw new CryptoSQLiteException($"Table {tableName} doesn't contain column {columnName}");

                var col = properties.First(p => p.ColumnName() == columnName);

                if (col.IsEncrypted())
                    throw new CryptoSQLiteException($"Column {columnName} has [Encrypted] attribute, so this column is encrypted. Find function can't work with encrypted columns.");

            }

            var cmd = SqlCmds.CmdFindInTable(tableName, columnName, lowerValue, upperValue);

            var table = ReadRowsFromTable(cmd, lowerValue, upperValue);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(properties, row, item);
                items.Add(item);
            }

            return items;
        }

        private List<List<SqlColumnInfo>> ReadRowsFromTable(string cmd, params object[] values)
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

        private void ProcessRow<TTable>(IEnumerable<PropertyInfo> propertieInfos, IEnumerable<SqlColumnInfo> columnInfos, TTable item)
        {
            var properties = propertieInfos.ToList();
            var columns = columnInfos.ToList();

            ICryptoProvider encryptor = null;
            var soltColumn = columns.Find(c => c.Name == SoltColumnName);
            if (soltColumn != null)
            {
                var solt = (byte[])soltColumn.SqlValue;
                encryptor = GetEncryptor(solt); // this solt we were using in encryption of columns
            }

            foreach (var property in properties)
            {
                var column = columns.Find(c => c.Name == property.ColumnName());
                if (column == null)
                    throw new CryptoSQLiteException(
                        $"Can't find appropriate column in database for property: {property.ColumnName()}");

                if (column.SqlValue == null)
                    continue;   // NULL value will stay NULL value

                if (property.IsEncrypted())
                    GetDecryptedValue(property, item, column.SqlValue, encryptor);
                else
                    OrmUtils.GetClrViewFromSql(property, item, column.SqlValue);
            }
        }

        private static object GetEncryptedValue(Type type, object value, ICryptoProvider encryptor)
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            if (type == typeof(string))
            {
                var bytes = Encoding.Unicode.GetBytes((string)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var dateTime = (DateTime) value;
                var bytes = BitConverter.GetBytes(dateTime.ToBinary());
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(ushort) || type == typeof(ushort?))
            {
                var bytes = BitConverter.GetBytes((ushort)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(short) || type == typeof(short?))
            {
                var bytes = BitConverter.GetBytes((short)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(uint) || type == typeof(uint?))
            {
                var bytes = BitConverter.GetBytes((uint)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                var bytes = BitConverter.GetBytes((int)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(ulong) || type == typeof(ulong?))
            {
                var bytes = BitConverter.GetBytes((ulong)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                var bytes = BitConverter.GetBytes((long)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(float) || type == typeof(float?))
            {
                var bytes = BitConverter.GetBytes((float)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                var bytes = BitConverter.GetBytes((double)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(byte[]))
            {
                var bytes = (byte[])value;
                var bytesForEncrypt = new byte[bytes.Length];
                bytesForEncrypt.MemCpy(bytes, bytes.Length);
                encryptor.XorGamma(bytesForEncrypt);
                return bytesForEncrypt;
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                var bytes = BitConverter.GetBytes((bool) value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            if (type == typeof(byte) || type == typeof(byte?))
            {
                var bytes = BitConverter.GetBytes((byte)value);
                encryptor.XorGamma(bytes);
                return bytes;
            }
            
            throw new CryptoSQLiteException($"Type {type} is not compatible with CryptoSQLite");
        }
     
        private static void GetDecryptedValue(PropertyInfo property, object item, object sqlValue, ICryptoProvider encryptor)
        {
            if (encryptor == null)
                throw new CryptoSQLiteException("Internal error. Encryptor should be enitialized.");

            var type = property.PropertyType;

            if (type == typeof(string))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                var openString = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                property.SetValue(item, openString);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                var ticks = BitConverter.ToInt64(bytes, 0);
                var dateTime = DateTime.FromBinary(ticks);
                property.SetValue(item, dateTime);
            }
            else if (type == typeof(short) || type == typeof(short?))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToInt16(bytes, 0));
            }
            else if (type == typeof(ushort) || type == typeof(ushort?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToUInt16(bytes, 0));
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToInt32(bytes, 0));
            }
            else if (type == typeof(uint) || type == typeof(uint?))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToUInt32(bytes, 0));
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToInt64(bytes, 0));
            }
            else if (type == typeof(ulong) || type == typeof(ulong?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToUInt64(bytes, 0));
            }
            else if (type == typeof(float) || type == typeof(float?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToSingle(bytes, 0));
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToDouble(bytes, 0));
            }
            else if (type == typeof(byte[]))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, bytes);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                var bytes = (byte[]) sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, BitConverter.ToBoolean(bytes, 0));
            }
            else if(type == typeof(byte) || type == typeof(byte?))
            {
                var bytes = (byte[])sqlValue;
                encryptor.XorGamma(bytes);
                property.SetValue(item, bytes[0]);
            }
        }

        private ICryptoProvider GetEncryptor(byte[] solt = null)
        {
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
