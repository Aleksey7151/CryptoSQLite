using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CryptoSQLite.CryptoProviders;
using CryptoSQLite.Expressions;
using CryptoSQLite.Extensions;
using CryptoSQLite.Mapping;
using SQLitePCL.pretty;


namespace CryptoSQLite
{
    /// <summary>
    /// Class represents CryptoSQLite Exception.
    /// </summary>
    public sealed class CryptoSQLiteException : Exception
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
    /// Sort orders
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// Sorts the records in ascending order. This is default order.
        /// </summary>
        Asc,
        /// <summary>
        /// Sorts the records in a descending order
        /// </summary>
        Desc
    }

    /// <summary>
    /// Enumerates crypto algorithms, that can be used for data encryption in CryptoSQLite library
    /// </summary>
    public enum CryptoAlgorithms
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
        /// Updates the row(s) in table <typeparamref name="TTable"/> with values from <paramref name="item"/>.
        /// <para/>LIMITATIONS: This function updates all columns in a row with values from <paramref name="item"/>. We can't specify names of columns which values we want to update. That's because some columns can be encrypted.
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="item">Element with new values for updating.</param>
        /// <param name="predicate">Condition which determines row(s) that must be updated.</param>
        void Update<TTable>(TTable item, Expression<Predicate<TTable>> predicate) where TTable : class;

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
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table whose <paramref name="columnName"/> contain value == <paramref name="columnValue"/>
        /// <para/>If <paramref name="columnValue"/> == null, it will find all rows which <paramref name="columnName"/> value is null.
        /// </summary>
        /// <typeparam name="TTable">Type of Table in which the element should be finded.</typeparam>
        /// <param name="columnName">Column name in table which values will be used in finding elements.</param>
        /// <param name="columnValue">Value for find</param>
        /// <returns>All elements from table that are satisfying conditions.</returns>
        IEnumerable<TTable> FindByValue<TTable>(string columnName, object columnValue) where TTable : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningOnCondition"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Left table type</typeparam>
        /// <typeparam name="TTable2">Right table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join's result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningOnCondition">The relationship between the two tables for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines view of returning result.</param>
        /// <returns>Results of Jioning request</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningOnCondition,
            Func<TTable1, TTable2, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the first table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the second table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Func<TTable1, TTable2, TTable3, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TTable4">Third joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the First table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the Second table for joining them.</param>
        /// <param name="joiningConditionWithTable4">The relationship between Main table and the Third table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TTable4, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Expression<Func<TTable1, TTable4, bool>> joiningConditionWithTable4,
            Func<TTable1, TTable2, TTable3, TTable4, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new()
            where TTable4 : class, new();

        /// <summary>
        /// The LeftJoin function returns all rows from the left table <typeparamref name="TLeftTable"/>, with the matching rows in the right table <typeparamref name="TRightTable"/>. The result is NULL in the right side when there is no match
        /// </summary>
        /// <typeparam name="TLeftTable">Type of the left table</typeparam>
        /// <typeparam name="TRightTable">Type of the right table</typeparam>
        /// <typeparam name="TJoinResult">Type of joining result</typeparam>
        /// <param name="whereCondition">Where clause for left table</param>
        /// <param name="keysSelectorExpression">The relationship between Left table and the Right table for joining them.</param>
        /// <param name="joiningResult">Delegate that determines the view of the joining result.</param>
        /// <returns></returns>
        IEnumerable<TJoinResult> LeftJoin<TLeftTable, TRightTable, TJoinResult>(
            Expression<Predicate<TLeftTable>> whereCondition,
            Expression<Func<TLeftTable, TRightTable, bool>> keysSelectorExpression,
            Func<TLeftTable, TRightTable, TJoinResult> joiningResult)
            where TLeftTable : class, new()
            where TRightTable : class, new();


            /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Properties whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        IEnumerable<TTable> Select<TTable>(Expression<Predicate<TTable>> predicate,
            params Expression<Func<TTable, object>>[] selectedProperties)
            where TTable : class, new();

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        IEnumerable<TTable> SelectTop<TTable>(int count) where TTable : class, new();

        #region SQLiteFunctions

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        double Max<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Max value.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        double Max<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        double Min<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Min value.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        double Min<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        double Sum<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Summary value.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        double Sum<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        double Avg<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining average value.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        double Avg<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        #endregion  //SQLiteFunctions
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
        /// Updates the row(s) in table <typeparamref name="TTable"/> with values from <paramref name="item"/>.
        /// <para/>LIMITATIONS: This function updates all columns in a row with values from <paramref name="item"/>. We can't specify names of columns which values we want to update. That's because some columns can be encrypted.
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="item">Element with new values for updating.</param>
        /// <param name="predicate">Condition which determines row(s) that must be updated.</param>
        Task UpdateAsync<TTable>(TTable item, Expression<Predicate<TTable>> predicate) where TTable : class;

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
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> FindAsync<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> FindAsync<TTable>(Expression<Predicate<TTable>> predicate, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> FindAsync<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber, Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new();

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
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningOnCondition"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Left table type</typeparam>
        /// <typeparam name="TTable2">Right table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningOnCondition">The relationship between the two tables for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines view of returning result.</param>
        /// <returns>Results of Jioning request</returns>
        Task<IEnumerable<TJoinResult>> JoinAsync<TTable1, TTable2, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningOnCondition,
            Func<TTable1, TTable2, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the first table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the second table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        Task<IEnumerable<TJoinResult>> JoinAsync<TTable1, TTable2, TTable3, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Func<TTable1, TTable2, TTable3, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new();

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TTable4">Third joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the First table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the Second table for joining them.</param>
        /// <param name="joiningConditionWithTable4">The relationship between Main table and the Third table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        Task<IEnumerable<TJoinResult>> JoinAsync<TTable1, TTable2, TTable3, TTable4, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Expression<Func<TTable1, TTable4, bool>> joiningConditionWithTable4,
            Func<TTable1, TTable2, TTable3, TTable4, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new()
            where TTable4 : class, new();

        /// <summary>
        /// The LeftJoin function returns all rows from the left table <typeparamref name="TLeftTable"/>, with the matching rows in the right table <typeparamref name="TRightTable"/>. The result is NULL in the right side when there is no match
        /// </summary>
        /// <typeparam name="TLeftTable">Type of the left table</typeparam>
        /// <typeparam name="TRightTable">Type of the right table</typeparam>
        /// <typeparam name="TJoinResult">Type of joining result</typeparam>
        /// <param name="whereCondition">Where clause for left table</param>
        /// <param name="keysSelectorExpression">The relationship between Left table and the Right table for joining them.</param>
        /// <param name="joiningResult">Delegate that determines the view of the joining result.</param>
        /// <returns></returns>
        Task<IEnumerable<TJoinResult>> LeftJoinAsync<TLeftTable, TRightTable, TJoinResult>(
            Expression<Predicate<TLeftTable>> whereCondition,
            Expression<Func<TLeftTable, TRightTable, bool>> keysSelectorExpression,
            Func<TLeftTable, TRightTable, TJoinResult> joiningResult)
            where TLeftTable : class, new()
            where TRightTable : class, new();

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Properties whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        Task<IEnumerable<TTable>> SelectAsync<TTable>(Expression<Predicate<TTable>> predicate,
            params Expression<Func<TTable, object>>[] selectedProperties)
            where TTable : class, new();

        /// <summary>
        /// Returns the first '<paramref name="count"/>' records from the table: <typeparamref name="TTable"/>
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="count">Count of first elements, that will be returned.</param>
        /// <returns></returns>
        Task<IEnumerable<TTable>> SelectTopAsync<TTable>(int count) where TTable : class, new();

        #region SQLiteFunctions

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        Task<double> MaxAsync<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Max value.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        Task<double> MaxAsync<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        Task<double> MinAsync<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Min value.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        Task<double> MinAsync<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        Task<double> SumAsync<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Summary value.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        Task<double> SumAsync<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        Task<double> AvgAsync<TTable>(string columnName) where TTable : class;

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining average value.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        Task<double> AvgAsync<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class;

        #endregion  //SQLiteFunctions

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
        /// Translates Join-ON Rule between two tables
        /// </summary>
        private readonly JoinOnTranslator _joinOnTranslator;

        /// <summary>
        /// Encryption algorithm that is used for data protection
        /// </summary>
        private readonly CryptoAlgorithms _algorithm;

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

        private CryptoSQLiteConnection(CryptoAlgorithms cryptoAlgorithm)
        {
            switch (cryptoAlgorithm)
            {
                case CryptoAlgorithms.AesWith256BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes256);
                    break;

                case CryptoAlgorithms.AesWith192BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes192);
                    break;

                case CryptoAlgorithms.AesWith128BitsKey:
                    _cryptor = new AesCryptoProvider(Aes.AesKeyType.Aes128);
                    break;

                case CryptoAlgorithms.Gost28147With256BitsKey:
                    _cryptor = new GostCryptoProvider();
                    break;

                case CryptoAlgorithms.DesWith56BitsKey:
                    _cryptor = new DesCryptoProvider();
                    break;

                case CryptoAlgorithms.TripleDesWith168BitsKey:
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
            _joinOnTranslator = new JoinOnTranslator();
        }
        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFilename">Path to SQLite database file.</param>
        public CryptoSQLiteConnection(string dbFilename) : this(CryptoAlgorithms.AesWith256BitsKey)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
        }

        /// <summary>
        /// Constructor. Creates connection to SQLite datebase file with data encryption.
        /// </summary>
        /// <param name="dbFilename">Path to database file</param>
        /// <param name="cryptoAlgorithm">Type of crypto algorithm that will be used for data encryption</param>
        public CryptoSQLiteConnection(string dbFilename, CryptoAlgorithms cryptoAlgorithm) : this(cryptoAlgorithm)
        {
            _connection = SQLite3.Open(dbFilename, ConnectionFlags.ReadWrite | ConnectionFlags.Create, null);
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

            var cmd = SqlCommands.CmdCreateTable(tableMap);

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
                _connection.Execute(SqlCommands.CmdDeleteTable(tableName));
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
                _connection.Execute(SqlCommands.CmdClearTable(tableName));
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

            var countCmd = SqlCommands.CmdCount(tableName);

            var queryable = _connection.Query(countCmd);

            foreach (var row in queryable)
            {
                foreach (var column in row)
                {
                    return column.ToInt();
                }
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

            var countCmd = SqlCommands.CmdCountForPredicate(tableName) +
                           _predicateTranslator.TranslateToSqlStatement(predicate, tableName, tableMap.Columns.Values,
                               out values);

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
                var cex = new CryptoSQLiteException($"Please check if table {tableName} exists in database.");
                
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
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var countCmd = SqlCommands.CmdCount(tableName, columnName);

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

            var countCmd = SqlCommands.CmdCount(tableName, columnName, true);

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
        /// Updates the row(s) in table <typeparamref name="TTable"/> with values from <paramref name="item"/>.
        /// <para/>LIMITATIONS: This function updates all columns in a row with values from <paramref name="item"/>. We can't specify names of columns which values we want to update. That's because some columns can be encrypted.
        /// </summary>
        /// <typeparam name="TTable">Type of table.</typeparam>
        /// <param name="item">Element with new values for updating.</param>
        /// <param name="predicate">Condition which determines row(s) that must be updated.</param>
        public void Update<TTable>(TTable item, Expression<Predicate<TTable>> predicate) where TTable : class
        {
            if(predicate == null)
                throw new CryptoSQLiteException("Predicate can't be null");

            var tableMap = CheckTable<TTable>();

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
                if (column.Value.IsAutoIncremental)
                    continue; // if column is AutoIncremental so we don't want to replace this row

                var value = ((IValues<TTable>)column.Value).ValueGetter(item);
                // Here we get value without reflection!!! We use here Expressions

                if (value == null && column.Value.DefaultValue != null)
                    continue;
                // if column has dafault value, so when column passed without value, we don't use this column in SQL command for insert element 

                if (value == null && column.Value.IsNotNull && column.Value.DefaultValue == null)
                    throw new CryptoSQLiteException(
                        $"You are trying to pass NULL-value for Column '{column.Value.Name}', but this column has NotNull atribute and Default Value is not defined.");

                columnNames.Add(column.Key);

                var clrType = column.Value.ClrType;

                object sqlValue = null;

                if (value != null)
                {
                    sqlValue = column.Value.IsEncrypted
                        ? GetEncryptedValueForSql(clrType, value, column.Value.ColumnNumber, encryptor)
                        : GetOpenValueForSql(clrType, value);
                }

                columnValues.Add(sqlValue); // NULL will be NULL
            }

            if (solt != null)
            {
                columnNames.Add(SoltColumnName);
                columnValues.Add(solt);
            }

            object[] values;
            var wherePredicate = _predicateTranslator.TranslateToSqlStatement(predicate, tableName, tableMap.Columns.Values, out values);

            var cmd = SqlCommands.CmdUpdate(tableName, columnNames, wherePredicate);

            columnValues.AddRange(values);

            try
            {
                _connection.Execute(cmd, columnValues.ToArray());
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    "Column with ForeignKey constrait has invalid value or table doesn't exist in database.");
            }
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

            var cmd = SqlCommands.CmdDeleteForPredicate(tableName) +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, tableMap.Columns.Values,
                          out values);

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

            var cmd = SqlCommands.CmdDeleteRow(tableName, columnName, columnValue);

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

            var cmd = SqlCommands.CmdSelectAllTable(tableName);

            //TODO change signature of a call
            var table = ReadRowsFromDatabase(cmd, new object[] {}, tableMap);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(mappedColumns, row[tableName], item);
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
            return FindRecords(predicate, null);
        }

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber)
            where TTable : class, new()
        {
            return FindRecords(predicate, limitNumber);
        }

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, Expression<Func<TTable, object>> orderByColumnSelector,
            SortOrder sortOrder = SortOrder.Asc) where TTable : class, new()
        {
            return FindRecords(predicate, null, orderByColumnSelector, sortOrder);
        }

        /// <summary>
        /// Finds all elements in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="limitNumber">Defines the number of records to return</param>
        /// <param name="orderByColumnSelector">Expression that defines ORDER BY column</param>
        /// <param name="sortOrder">Sort order type. Ascending order is default.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public IEnumerable<TTable> Find<TTable>(Expression<Predicate<TTable>> predicate, int limitNumber,
            Expression<Func<TTable, object>> orderByColumnSelector, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new()
        {
            return FindRecords(predicate, limitNumber, orderByColumnSelector, sortOrder);
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
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Left table type</typeparam>
        /// <typeparam name="TTable2">Right table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between the two tables for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        public IEnumerable<TJoinResult> Join<TTable1, TTable2, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Func<TTable1, TTable2, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
        {
            var tableMap1 = CheckTable<TTable1>();
            var tableMap2 = CheckTable<TTable2>();

            if (tableMap1.Type == tableMap2.Type)
                throw new CryptoSQLiteException("You can't join table with itself.");
            
            var onJoin = _joinOnTranslator.Translate(joiningConditionWithTable2, tableMap1, tableMap2);

            object[] values = {};

            var where = whereCondition != null
                ? _predicateTranslator.TranslateToSqlStatement(whereCondition, tableMap1.Name, tableMap1.Columns.Values, out values, true)
                : null;

            var cmd = SqlCommands.CmdJoinTwoTables(tableMap1, tableMap2, onJoin, where);
            var toRet = new List<TJoinResult>();
            try
            {
                var tables = ReadRowsFromDatabaseForTwoTables(cmd, values, tableMap1, tableMap2);

                foreach (var row in tables)
                {
                    // First Table
                    var cols1 = row[tableMap1.Name];
                    var tableInst1 = new TTable1();
                    ProcessRow(tableMap1.Columns.Values, cols1, tableInst1);

                    // Second Table
                    var cols2 = row[tableMap2.Name];
                    var tableInst2 = new TTable2();
                    ProcessRow(tableMap2.Columns.Values, cols2, tableInst2);

                    toRet.Add(joiningResultView(tableInst1, tableInst2));
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message);
            }

            return toRet;
        }

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TJoinResult">Type of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the First table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the Second table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        public IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3, 
            Func<TTable1, TTable2, TTable3, TJoinResult> joiningResultView)
            where TTable1 : class, new()
            where TTable2 : class, new()
            where TTable3 : class, new()
        {
            var tableMap1 = CheckTable<TTable1>();
            var tableMap2 = CheckTable<TTable2>();
            var tableMap3 = CheckTable<TTable3>();

            if (tableMap1.Type == tableMap2.Type || tableMap1.Type == tableMap3.Type || tableMap2.Type == tableMap3.Type)
                throw new CryptoSQLiteException("You can't join table with itself.");

            var onJoin12 = _joinOnTranslator.Translate(joiningConditionWithTable2, tableMap1, tableMap2);

            var onJoin13 = _joinOnTranslator.Translate(joiningConditionWithTable3, tableMap1, tableMap3);

            object[] values = { };

            var where = whereCondition != null
                ? _predicateTranslator.TranslateToSqlStatement(whereCondition, tableMap1.Name, tableMap1.Columns.Values, out values, true)
                : null;

            var cmd = SqlCommands.CmdJoinThreeTables(tableMap1, tableMap2, tableMap3, onJoin12, onJoin13, where);
            var toRet = new List<TJoinResult>();
            try
            {
                var tables = ReadRowsFromDatabaseForThreeTables(cmd, values, tableMap1, tableMap2, tableMap3);

                foreach (var row in tables)
                {
                    // First Table
                    var cols1 = row[tableMap1.Name];
                    var tableInst1 = new TTable1();
                    ProcessRow(tableMap1.Columns.Values, cols1, tableInst1);

                    // Second Table
                    var cols2 = row[tableMap2.Name];
                    var tableInst2 = new TTable2();
                    ProcessRow(tableMap2.Columns.Values, cols2, tableInst2);

                    // Third Table
                    var cols3 = row[tableMap3.Name];
                    var tableInst3 = new TTable3();
                    ProcessRow(tableMap3.Columns.Values, cols3, tableInst3);

                    toRet.Add(joiningResultView(tableInst1, tableInst2, tableInst3));
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message);
            }

            return toRet;
        }

        /// <summary>
        /// Returns all rows from both tables as long as there is a match defined by '<paramref name="joiningConditionWithTable2"/>' between the columns in both tables.
        /// </summary>
        /// <typeparam name="TTable1">Main table type</typeparam>
        /// <typeparam name="TTable2">First joined table type</typeparam>
        /// <typeparam name="TTable3">Second joined table type</typeparam>
        /// <typeparam name="TTable4">Third joined table type</typeparam>
        /// <typeparam name="TJoinResult">TYpe of join result</typeparam>
        /// <param name="whereCondition">Where clause</param>
        /// <param name="joiningConditionWithTable2">The relationship between Main table and the First table for joining them.</param>
        /// <param name="joiningConditionWithTable3">The relationship between Main table and the Second table for joining them.</param>
        /// <param name="joiningConditionWithTable4">The relationship between Main table and the Third table for joining them.</param>
        /// <param name="joiningResultView">Delegate that determines the view of returning result.</param>
        /// <returns>Joined tables</returns>
        public IEnumerable<TJoinResult> Join<TTable1, TTable2, TTable3, TTable4, TJoinResult>(Expression<Predicate<TTable1>> whereCondition,
            Expression<Func<TTable1, TTable2, bool>> joiningConditionWithTable2,
            Expression<Func<TTable1, TTable3, bool>> joiningConditionWithTable3,
            Expression<Func<TTable1, TTable4, bool>> joiningConditionWithTable4,
            Func<TTable1, TTable2, TTable3, TTable4, TJoinResult> joiningResultView)
            where TTable1 : class, new() 
            where TTable2 : class, new() 
            where TTable3 : class, new() 
            where TTable4 : class, new()
        {
            var tableMap1 = CheckTable<TTable1>();
            var tableMap2 = CheckTable<TTable2>();
            var tableMap3 = CheckTable<TTable3>();
            var tableMap4 = CheckTable<TTable4>();

            if (tableMap1.Type == tableMap2.Type || tableMap1.Type == tableMap3.Type || tableMap2.Type == tableMap3.Type ||
                tableMap1.Type == tableMap4.Type || tableMap2.Type == tableMap4.Type || tableMap3.Type == tableMap4.Type)
                throw new CryptoSQLiteException("You can't join table with itself.");

            var onJoin12 = _joinOnTranslator.Translate(joiningConditionWithTable2, tableMap1, tableMap2);

            var onJoin13 = _joinOnTranslator.Translate(joiningConditionWithTable3, tableMap1, tableMap3);

            var onJoin14 = _joinOnTranslator.Translate(joiningConditionWithTable4, tableMap1, tableMap4);



            object[] values = { };

            var where = whereCondition != null
                ? _predicateTranslator.TranslateToSqlStatement(whereCondition, tableMap1.Name, tableMap1.Columns.Values, out values, true)
                : null;

            var cmd = SqlCommands.CmdJoinFourTables(tableMap1, tableMap2, tableMap3, tableMap4, onJoin12, onJoin13, onJoin14, where);
            var toRet = new List<TJoinResult>();
            try
            {
                var tables = ReadRowsFromDatabaseForFourTables(cmd, values, tableMap1, tableMap2, tableMap3, tableMap4);

                foreach (var row in tables)
                {
                    // First Table
                    var cols1 = row[tableMap1.Name];
                    var tableInst1 = new TTable1();
                    ProcessRow(tableMap1.Columns.Values, cols1, tableInst1);

                    // Second Table
                    var cols2 = row[tableMap2.Name];
                    var tableInst2 = new TTable2();
                    ProcessRow(tableMap2.Columns.Values, cols2, tableInst2);

                    // Third Table
                    var cols3 = row[tableMap3.Name];
                    var tableInst3 = new TTable3();
                    ProcessRow(tableMap3.Columns.Values, cols3, tableInst3);

                    // Fourth Table
                    var cols4 = row[tableMap4.Name];
                    var tableInst4 = new TTable4();
                    ProcessRow(tableMap4.Columns.Values, cols4, tableInst4);

                    toRet.Add(joiningResultView(tableInst1, tableInst2, tableInst3, tableInst4));
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message);
            }

            return toRet;
        }

        /// <summary>
        /// The LeftJoin function returns all rows from the left table <typeparamref name="TLeftTable"/>, with the matching rows in the right table <typeparamref name="TRightTable"/>. The result is NULL in the right side when there is no match
        /// </summary>
        /// <typeparam name="TLeftTable">Type of the left table</typeparam>
        /// <typeparam name="TRightTable">Type of the right table</typeparam>
        /// <typeparam name="TJoinResult">Type of joining result</typeparam>
        /// <param name="whereCondition">Where clause for left table</param>
        /// <param name="keysSelectorExpression">The relationship between Left table and the Right table for joining them.</param>
        /// <param name="joiningResult">Delegate that determines the view of the joining result.</param>
        /// <returns></returns>
        public IEnumerable<TJoinResult> LeftJoin<TLeftTable, TRightTable, TJoinResult>(
            Expression<Predicate<TLeftTable>> whereCondition,
            Expression<Func<TLeftTable, TRightTable, bool>> keysSelectorExpression,
            Func<TLeftTable, TRightTable, TJoinResult> joiningResult)
            where TLeftTable : class, new()
            where TRightTable : class, new()
        {
            var tableLeft = CheckTable<TLeftTable>();
            var tableRight = CheckTable<TRightTable>();

            if (tableLeft.Type == tableRight.Type)
                throw new CryptoSQLiteException("You can't join table with itself.");

            var onJoin = _joinOnTranslator.Translate(keysSelectorExpression, tableLeft, tableRight);

            object[] values = { };

            var where = whereCondition != null
                ? _predicateTranslator.TranslateToSqlStatement(whereCondition, tableLeft.Name, tableLeft.Columns.Values, out values, true)
                : null;

            var cmd = SqlCommands.CmdLeftJoinTwoTables(tableLeft, tableRight, onJoin, where);
            var toRet = new List<TJoinResult>();
            try
            {
                var tables = ReadRowsFromDatabaseForTwoTables(cmd, values, tableLeft, tableRight);

                foreach (var row in tables)
                {
                    // Left Table
                    var cols1 = row[tableLeft.Name];
                    var tableInst1 = new TLeftTable();
                    ProcessRow(tableLeft.Columns.Values, cols1, tableInst1);

                    // Right Table
                    var cols2 = row[tableRight.Name];

                    if (cols2.All(ci => ci.SqlType == null && ci.SqlValue == null))
                    {
                        toRet.Add(joiningResult(tableInst1, null));
                    }
                    else
                    {
                        var tableInst2 = new TRightTable();

                        ProcessRow(tableRight.Columns.Values, cols2, tableInst2);

                        toRet.Add(joiningResult(tableInst1, tableInst2));
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message);
            }

            return toRet;
        }

        /// <summary>
        /// Finds all elements, but not all columns, in table <typeparamref name="TTable"/> which satisfy the condition defined in <paramref name="predicate"/>
        /// <para/>Warning: Properties with type 'UInt64?', 'Int64?', 'DateTime?', 'Byte[]'
        /// <para/>can be used only in Equal To Null or Not Equal To Null Predicate Statements: PropertyValue == null or PropertyValue != null. In any other Predicate statements they can't be used.
        /// <para/>Warning: Properties with type 'UInt64', 'Int64', 'DateTime' can't be used in Predicate statements, because they stored in SQL database file as BLOB data. This is done to protect against data loss.
        /// </summary>
        /// <typeparam name="TTable">Type of Table (element) in which items will be searched.</typeparam>
        /// <param name="predicate">Predicate that contains condition for finding elements</param>
        /// <param name="selectedProperties">Properties whose values will be obtained from database.</param>
        /// <returns>All elements in Table <typeparamref name="TTable"/> that are satisfy condition defined in <paramref name="predicate"/></returns>
        public IEnumerable<TTable> Select<TTable>(Expression<Predicate<TTable>> predicate, params Expression<Func<TTable, object>>[] selectedProperties)
            where TTable : class, new()
        {
            if (selectedProperties == null || selectedProperties.Length == 0)
                throw new CryptoSQLiteException("You must specify at least one property that will be obtained");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;
            
            IList<string> columnNames = new List<string>();
            IList<string> propertyNames = new List<string>();
            var hasEncrypted = false;

            foreach (var expression in selectedProperties)
            {
                bool isEncrypted;
                string propertyName;

                var columnName = AccessMemberTranslator.GetColumnName(expression, tableName, mappedColumns, out isEncrypted, out propertyName);

                if (isEncrypted)
                {
                    // we must read SoltColumn from database only if onle of selected properties has Encrypted attribute
                    hasEncrypted = true;
                }
                columnNames.Add(columnName);
                propertyNames.Add(propertyName);
            }

            if (hasEncrypted)
                columnNames.Add(SoltColumnName);

            var joinedColumnNames = string.Join(", ", columnNames);

            string cmdForPredicate = $"SELECT {joinedColumnNames} FROM {tableName} WHERE ";
            
            object[] values;

            var cmd = cmdForPredicate +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            var table = ReadRowsFromDatabase(cmd, values, tableMap);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row[tableName], item);

                FindReferencedTables(item, propertyNames.ToArray()); // here we get all referenced tables if they exist

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

            var cmd = SqlCommands.CmdSelectTop(tableName);

            var table = ReadRowsFromDatabase(cmd, new object[] {count}, tableMap);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row[tableName], item);

                FindReferencedTables(item); // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }

        #region SQLiteFunctions

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        public double Max<TTable>(string columnName) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var cmd = SqlCommands.CmdMax(tableName, columnName);

            return SQLiteMathFunction(cmd, null);
        }

        /// <summary>
        /// Returns the largest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Max value.</param>
        /// <returns>Largest value of selected column: <paramref name="columnName"/>.</returns>
        public double Max<TTable>(string columnName, Expression<Predicate<TTable>> predicate)
            where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            if (predicate == null)
                throw new CryptoSQLiteException("Predicate can't be null.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = SqlCommands.CmdMaxForPredicate(tableName, columnName) +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            return SQLiteMathFunction(cmd, values);
        }

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        public double Min<TTable>(string columnName) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var cmd = SqlCommands.CmdMin(tableName, columnName);

            return SQLiteMathFunction(cmd, null);
        }

        /// <summary>
        /// Returns the smallest value of the selected column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Min value.</param>
        /// <returns>Smallest value of selected column: <paramref name="columnName"/>.</returns>
        public double Min<TTable>(string columnName, Expression<Predicate<TTable>> predicate)
            where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            if (predicate == null)
                throw new CryptoSQLiteException("Predicate can't be null.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = SqlCommands.CmdMinForPredicate(tableName, columnName) +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            return SQLiteMathFunction(cmd, values);
        }

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        public double Sum<TTable>(string columnName) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var cmd = SqlCommands.CmdSum(tableName, columnName);

            return SQLiteMathFunction(cmd, null);
        }

        /// <summary>
        /// Returns the total sum of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Summary value.</param>
        /// <returns>Total sum of all values in selected column: <paramref name="columnName"/>.</returns>
        public double Sum<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            if (predicate == null)
                throw new CryptoSQLiteException("Predicate can't be null.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = SqlCommands.CmdSumForPredicate(tableName, columnName) +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            return SQLiteMathFunction(cmd, values);
        }

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        public double Avg<TTable>(string columnName) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var cmd = SqlCommands.CmdAvg(tableName, columnName);

            return SQLiteMathFunction(cmd, null);
        }

        /// <summary>
        /// Returns the average value of all values in the selected numeric column: <paramref name="columnName"/>, that are satisfying condition defined in <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="TTable">Type of Table.</typeparam>
        /// <param name="columnName">Selected column name.</param>
        /// <param name="predicate">Addition condition for determining Average value.</param>
        /// <returns>Average value of all values in selected column: <paramref name="columnName"/>.</returns>
        public double Avg<TTable>(string columnName, Expression<Predicate<TTable>> predicate) where TTable : class
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            if (predicate == null)
                throw new CryptoSQLiteException("Predicate can't be null.");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            if (!tableMap.Columns.Keys.Contains(columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var cmd = SqlCommands.CmdAvgForPredicate(tableName, columnName) +
                      _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            return SQLiteMathFunction(cmd, values);
        }

        #endregion  //SQLiteFunctions

        #endregion//CryptoSQLite interface


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

                var colMap = new ColumnMap<TTable>(columnName, prop.Name, prop.PropertyType, prop.SqlType(),
                    columnNumber,
                    prop.IsPrimaryKey(),
                    prop.IsAutoIncremental(), isEncrypted, prop.IsNotNull(), prop.DefaultValue(), isForeignKey,
                    foreignKey,
                    prop.ValueSetter<TTable>(), prop.ValueGetter<TTable>());

                columnMaps.Add(columnName, colMap);

                columnNumber += 1;
            }

            var tableMap = new TableMap(tableName, tableType, hasEncryptedColumns, columnMaps);

            _tables.Add(tableType, tableMap);

            foreach (var fk in foreignKeys) // Check all referenced tables that this table contains
            {
                if (_tables.ContainsKey(fk.TypeOfReferencedTable)) continue;

                var genericCheckTable = _methodCheckTable.MakeGenericMethod(fk.TypeOfReferencedTable);
                try
                {
                    genericCheckTable.Invoke(this, new object[] {true});
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
                throw new ArgumentNullException(nameof(key));

            if ((_algorithm == CryptoAlgorithms.AesWith256BitsKey ||
                 _algorithm == CryptoAlgorithms.Gost28147With256BitsKey) && key.Length < 32)
                throw new ArgumentException("Key length for AES with 256 bit key and GOST must be 32 bytes.");

            if ((_algorithm == CryptoAlgorithms.AesWith192BitsKey) && key.Length < 24)
                throw new ArgumentException("Key length for AES with 192 bit key must be 24 bytes.");

            if ((_algorithm == CryptoAlgorithms.AesWith128BitsKey) && key.Length < 16)
                throw new ArgumentException("Key length for AES with 128 bit key must be 16 bytes.");

            if (_algorithm == CryptoAlgorithms.DesWith56BitsKey && key.Length < 8)
                throw new ArgumentException("Key length for DES must be at least 8 bytes.");

            if (_algorithm == CryptoAlgorithms.TripleDesWith168BitsKey && key.Length < 24)
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
                foreach (var row in _connection.Query(SqlCommands.CmdGetTableInfo(tableName)))
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

            if (properties.Any(p => p.IsEncrypted() && p.DefaultValue() != null))
                throw new CryptoSQLiteException("Encrypted columns can't have default value, but they can be Not Null.");

            if (properties.Any(p => p.IsPrimaryKey() && p.ForeignKey() != null))
                throw new CryptoSQLiteException(
                    "Property can't have ForeignKey and PrimaryKey attributes simultaneously.");

            if (properties.Any(p => p.IsEncrypted() && p.ForeignKey() != null))
                throw new CryptoSQLiteException(
                    "Property can't have ForeignKey and Encrypted attributes simultaneously.");

            if (properties.Any(p => p.IsAutoIncremental() && p.ForeignKey() != null))
                throw new CryptoSQLiteException(
                    "Property can't have ForeignKey and AutoIncrement attributes simultaneously.");

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
                if (column.Value.IsAutoIncremental && !replaceRowIfExisits)
                    continue; // if column is AutoIncremental and we don't want to replace this row

                var value = ((IValues<TTable>) column.Value).ValueGetter(row);
                // Here we get value without reflection!!! We use here Expressions

                if (value == null && column.Value.DefaultValue != null)
                    continue;
                // if column has dafault value, so when column passed without value, we don't use this column in SQL command for insert element 

                if (value == null && column.Value.IsNotNull && column.Value.DefaultValue == null)
                    throw new CryptoSQLiteException($"You are trying to pass NULL-value for Column '{column.Value.Name}', but this column has NotNull atribute and Default Value is not defined.");

                columnNames.Add(column.Key);

                var clrType = column.Value.ClrType;

                object sqlValue = null;

                if (value != null)
                {
                    sqlValue = column.Value.IsEncrypted
                        ? GetEncryptedValueForSql(clrType, value, column.Value.ColumnNumber, encryptor)
                        : GetOpenValueForSql(clrType, value);
                }

                columnValues.Add(sqlValue); // NULL will be NULL
            }

            if (solt != null)
            {
                columnNames.Add(SoltColumnName);
                columnValues.Add(solt);
            }

            var cmd = replaceRowIfExisits
                ? SqlCommands.CmdInsertOrReplace(tableName, columnNames)
                : SqlCommands.CmdInsert(tableName, columnNames);
            try
            {
                _connection.Execute(cmd, columnValues.ToArray()); // Do not remove '.ToArray()' or you'll get a error 
            }
            catch (Exception ex)
            {
                throw new CryptoSQLiteException(ex.Message,
                    "Column with ForeignKey constrait has invalid value or table doesn't exist in database.");
            }
        }

        private IEnumerable<TTable> FindRecords<TTable>(Expression<Predicate<TTable>> predicate, int? limitNumber, Expression<Func<TTable, object>> orderByColumnSelector = null, SortOrder sortOrder = SortOrder.Asc) where TTable : class, new()
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate can't be null");

            var tableMap = CheckTable<TTable>();

            var tableName = tableMap.Name;

            var mappedColumns = tableMap.Columns.Values;

            object[] values;

            var strPredicate = _predicateTranslator.TranslateToSqlStatement(predicate, tableName, mappedColumns, out values);

            bool isOrderByEncrypted = false;
            string orderByPropertyName;
            var orderColumnName = orderByColumnSelector != null
                ? AccessMemberTranslator.GetColumnName(orderByColumnSelector, tableName, tableMap.Columns.Values,
                    out isOrderByEncrypted, out orderByPropertyName)
                : null;

            if(isOrderByEncrypted)
                throw new CryptoSQLiteException("Order By column can't be encrypted.");

            var cmd = SqlCommands.CmdSelectForPredicate(tableMap, strPredicate, orderColumnName, sortOrder, limitNumber);

            var table = ReadRowsFromDatabase(cmd, values, tableMap);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();

                ProcessRow(mappedColumns, row[tableName], item);

                FindReferencedTables(item); // here we get all referenced tables if they exist

                items.Add(item);
            }

            return items;
        }

        private IList<TTable> FindUsingColumnValue<TTable>(TableMap tableMap, string columnName, object columnValue)
            where TTable : class, new()
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var mappedColumns = tableMap.Columns.Values;

            var tableName = tableMap.Name;

            if (mappedColumns.All(mc => mc.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (mappedColumns.Any(mc => mc.Name == columnName && mc.IsEncrypted))
                throw new CryptoSQLiteException(
                    "You can't use [Encrypted] column as a column in which the columnValue should be found.");

            var cmd = SqlCommands.CmdSelect(tableName, columnName, columnValue);

            var table = ReadRowsFromDatabase(cmd, new[] {columnValue}, tableMap);

            var items = new List<TTable>();

            foreach (var row in table)
            {
                var item = new TTable();
                ProcessRow(mappedColumns, row[tableName], item);
                items.Add(item);
            }

            return items;
        }

        private TTable FindFirstUsingColumnValue<TTable>(TableMap tableMap, string columnName, object columnValue)
            where TTable : class, new()
        {
            if (string.IsNullOrEmpty(columnName))
                throw new CryptoSQLiteException("Column name can't be null or empty.");

            var mappedColumns = tableMap.Columns.Values.ToList();

            var tableName = tableMap.Name;

            if (mappedColumns.All(p => p.Name != columnName))
                throw new CryptoSQLiteException($"Table {tableName} doesn't contain column with name {columnName}.");

            if (mappedColumns.Any(p => p.Name == columnName && p.IsEncrypted))
                throw new CryptoSQLiteException(
                    "You can't use [Encrypted] column as a column in which the columnValue should be found.");

            var cmd = SqlCommands.CmdSelect(tableName, columnName, columnValue);

            var table = ReadRowsFromDatabase(cmd, new[] {columnValue}, tableMap);

            if (table.Count <= 0) return null;

            var item = new TTable();
            ProcessRow(mappedColumns, table[0][tableName], item);
            return item;
        }

        private double SQLiteMathFunction(string command, object[] values)
        {
            try
            {
                var queryable = values != null ? _connection.Query(command, values) : _connection.Query(command);
                foreach (var row in queryable)
                {
                    foreach (var column in row)
                    {
                        return column.ToDouble();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new CryptoSQLiteException(ex.Message, "");
            }
            return 0;
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
                if(!refTableInfo.IsAutoResolve)     // we obtain only those tables, that have autoResolveReference setted to TRUE 
                    continue;   

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

        private List<Dictionary<string, List<SqlColumnInfo>>> ReadRowsFromDatabase(string cmd, IEnumerable<object> values, TableMap tableMap1)
        {
            var table = new List<Dictionary<string,List<SqlColumnInfo>>>();
            try
            {
                var notNullValues = values.Where(v => v != null).ToArray();

                var queryable = notNullValues.Length == 0 ? _connection.Query(cmd) : _connection.Query(cmd, notNullValues);

                foreach (var row in queryable)
                {
                    var columnsFromFile = new Dictionary<string, List<SqlColumnInfo>>();
                    foreach (var column in row)
                    {
                        var tableName = string.IsNullOrEmpty(column.ColumnInfo.TableName) ? tableMap1.Name : column.ColumnInfo.TableName;
                        if (!columnsFromFile.ContainsKey(tableName))
                        {
                            columnsFromFile.Add(tableName, new List<SqlColumnInfo>());    // если для очередной таблицы еще не создан список со столбцами, то создаем его.
                        }
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
                        columnsFromFile[tableName].Add(tmp);   // NULL will be NULL.
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

        private IEnumerable<Dictionary<string, List<SqlColumnInfo>>> ReadRowsFromDatabaseForTwoTables(string cmd, IEnumerable<object> values, TableMap tableMap1, TableMap tableMap2)
        {
            var table = new List<Dictionary<string, List<SqlColumnInfo>>>();
            var countOfColumnsInFirstTable = tableMap1.HasEncryptedColumns ? tableMap1.Columns.Count + 1 : tableMap1.Columns.Count;
            try
            {
                var notNullValues = values.Where(v => v != null).ToArray();

                var queryable = notNullValues.Length == 0 ? _connection.Query(cmd) : _connection.Query(cmd, notNullValues);

                foreach (var row in queryable)
                {
                    var columnsFromFile = new Dictionary<string, List<SqlColumnInfo>>();
                    var colNumber = 0;
                    foreach (var column in row)
                    {
                        var tableName = colNumber < countOfColumnsInFirstTable ? tableMap1.Name : tableMap2.Name;

                        if (!columnsFromFile.ContainsKey(tableName))
                        {
                            columnsFromFile.Add(tableName, new List<SqlColumnInfo>());    // если для очередной таблицы еще не создан список со столбцами, то создаем его.
                        }
                        var tmp = new SqlColumnInfo { Name = column.ColumnInfo.Name };

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
                        columnsFromFile[tableName].Add(tmp);   // NULL will be NULL.
                        colNumber++;
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

        private IEnumerable<Dictionary<string, List<SqlColumnInfo>>> ReadRowsFromDatabaseForThreeTables(string cmd, IEnumerable<object> values, TableMap tableMap1, TableMap tableMap2, TableMap tableMap3)
        {
            var table = new List<Dictionary<string, List<SqlColumnInfo>>>();
            var countOfColumnsInFirstTable = tableMap1.HasEncryptedColumns ? tableMap1.Columns.Count + 1 : tableMap1.Columns.Count;
            var countOfColumnsInSecondTable = tableMap2.HasEncryptedColumns ? tableMap2.Columns.Count + 1 : tableMap2.Columns.Count;
            try
            {
                var notNullValues = values.Where(v => v != null).ToArray();

                var queryable = notNullValues.Length == 0 ? _connection.Query(cmd) : _connection.Query(cmd, notNullValues);

                foreach (var row in queryable)
                {
                    var columnsFromFile = new Dictionary<string, List<SqlColumnInfo>>();
                    var colNumber = 0;
                    foreach (var column in row)
                    {
                        string tableName = string.Empty;// = colNumber < countOfColumnsInFirstTable ? tableMap1.Name : tableMap2.Name;

                        if (colNumber < countOfColumnsInFirstTable)
                        {
                            tableName = tableMap1.Name;
                        }
                        else if(countOfColumnsInFirstTable <= colNumber && colNumber < countOfColumnsInFirstTable + countOfColumnsInSecondTable)
                        {
                            tableName = tableMap2.Name;
                        }
                        else if (countOfColumnsInFirstTable + countOfColumnsInSecondTable <= colNumber)
                        {
                            tableName = tableMap3.Name;
                        }

                        if (!columnsFromFile.ContainsKey(tableName))
                        {
                            columnsFromFile.Add(tableName, new List<SqlColumnInfo>());    // если для очередной таблицы еще не создан список со столбцами, то создаем его.
                        }
                        var tmp = new SqlColumnInfo { Name = column.ColumnInfo.Name };

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
                        columnsFromFile[tableName].Add(tmp);   // NULL will be NULL.
                        colNumber++;
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

        private IEnumerable<Dictionary<string, List<SqlColumnInfo>>> ReadRowsFromDatabaseForFourTables(string cmd, IEnumerable<object> values, TableMap tableMap1, TableMap tableMap2, TableMap tableMap3, TableMap tableMap4)
        {
            var table = new List<Dictionary<string, List<SqlColumnInfo>>>();

            var countOfColumnsInFirstTable = tableMap1.HasEncryptedColumns ? tableMap1.Columns.Count + 1 : tableMap1.Columns.Count;
            var countOfColumnsInSecondTable = tableMap2.HasEncryptedColumns ? tableMap2.Columns.Count + 1 : tableMap2.Columns.Count;
            var countOfColumnsInThirdTable = tableMap3.HasEncryptedColumns ? tableMap3.Columns.Count + 1 : tableMap3.Columns.Count;

            try
            {
                var notNullValues = values.Where(v => v != null).ToArray();

                var queryable = notNullValues.Length == 0 ? _connection.Query(cmd) : _connection.Query(cmd, notNullValues);

                foreach (var row in queryable)
                {
                    var columnsFromFile = new Dictionary<string, List<SqlColumnInfo>>();
                    var colNumber = 0;
                    foreach (var column in row)
                    {
                        string tableName = string.Empty;

                        if (colNumber < countOfColumnsInFirstTable)
                        {
                            tableName = tableMap1.Name;
                        }
                        else if (countOfColumnsInFirstTable <= colNumber && colNumber < countOfColumnsInFirstTable + countOfColumnsInSecondTable)
                        {
                            tableName = tableMap2.Name;
                        }
                        else if (countOfColumnsInFirstTable + countOfColumnsInSecondTable <= colNumber && colNumber < countOfColumnsInFirstTable + countOfColumnsInSecondTable + countOfColumnsInThirdTable)
                        {
                            tableName = tableMap3.Name;
                        }
                        else if(countOfColumnsInFirstTable + countOfColumnsInSecondTable + countOfColumnsInThirdTable <= colNumber)
                        {
                            tableName = tableMap4.Name;
                        }

                        if (!columnsFromFile.ContainsKey(tableName))
                        {
                            columnsFromFile.Add(tableName, new List<SqlColumnInfo>());    // если для очередной таблицы еще не создан список со столбцами, то создаем его.
                        }
                        var tmp = new SqlColumnInfo { Name = column.ColumnInfo.Name };

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
                        columnsFromFile[tableName].Add(tmp);   // NULL will be NULL.
                        colNumber++;
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
